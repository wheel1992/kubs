using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class ProgramBlock : Block
    {
        public delegate void HoverEventHandler(int targetZoneId);
        public delegate void SnapEventHandler(GameObject block, int zoneId);
        public event HoverEventHandler Hover;
        public event HoverEventHandler Unhover;
        public event SnapEventHandler Snap;
        public ProgramBlockType Type { get; set; }
        public State State { get; set; }
        public int ZoneId { get; set; }
        [HideInInspector]
        public int HoverZoneId = -1;
        [HideInInspector]
        //public bool isCloned = true;
        private Rigidbody _rb;
        private VRTK_InteractableObject _interactableObject;

        #region Lifecycle

        void Awake()
        {
            Category = BlockCategory.Program;
            ZoneId = -1;
            State = State.UnsnapIdle;
        }

        // Use this for initialization
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _interactableObject = GetComponentInteractableObject();

            // Sweep child block by default is not triggered
            // Only triggered when it is attached
            PauseSweepChildTrigger();

            GetSweepChild().OnEnter += new SweepChildBlock.TriggerEventHandler(DoChildTriggeredEnter);
            GetSweepChild().OnExit += new SweepChildBlock.TriggerEventHandler(DoChildTriggerExit);

            // Fire off event
            // To register block program events such as Hover, Unhover and Snap
            EventManager.TriggerEvent(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_HOVER_EVENT, gameObject);
            EventManager.TriggerEvent(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_SNAP_EVENT, gameObject);

            // Set its type again by checking its name
            GetProgramBlockByGameObject(gameObject).Type = GetProgramBlockTypeByName(gameObject.name);
        }
        private void OnTriggerExit(Collider other)
        {
            if (other == null) { return; }
            // I am an Unsnap BlockProgram
            if (State == State.UnsnapHover)
            {
                if (other.gameObject.tag.CompareTo(Constant.TAG_TEMPORARY_POSITION_OBJECT) == 0)
                {
                    // I am exiting from current zone!
                    Unhover(HoverZoneId);
                    HoverZoneId = -1;
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other == null) { return; }
            // I am an Unsnap BlockProgram
            if (State == State.UnsnapHover)
            {
                if (other.gameObject.tag.CompareTo(Constant.TAG_TEMPORARY_POSITION_OBJECT) == 0)
                {
                    // I am still colliding at the current zone!
                }
            }
        }
        private void Update()
        {
            // Only blocks with isCloned will check for SnapDropZone
            if (GetComponentInteractableObject().IsInSnapDropZone() && GetVRTKSnapDropZone().CompareTag(Constant.TAG_SNAP_DROP_ZONE_PLATE))
            {
                StartSweepChildTrigger();
                ZoneId = GetSnapDropZone().ZoneId;
            }
            else
            {
                PauseSweepChildTrigger();
                ZoneId = -1;
            }
        }
        private void DoChildTriggeredEnter(Collider other)
        {
            // My SweepTestChild collide with other.gameobject which is a ProgramBlock
            if (other != null && other.gameObject.tag.CompareTo(Constant.TAG_BLOCK_PROGRAM) == 0)
            {
                ProgramBlock otherBlock = other.gameObject.GetComponent<ProgramBlock>();

                if (otherBlock.IsGrabbed())
                {
                    Debug.Log("DoChildTriggeredEnter: other program block HoverZoneId " + otherBlock.HoverZoneId);
                    if (ZoneId > -1 && ZoneId != otherBlock.HoverZoneId)
                    {
                        if (otherBlock.HoverZoneId > -1)
                        {
                            // has hovered on other zone currently
                            Unhover(otherBlock.HoverZoneId);
                            otherBlock.HoverZoneId = -1;
                        }

                        Hover(ZoneId);
                        // Set my current state to SnapTempMove
                        State = State.SnapTempMove;

                        // Set other block HoverZoneId and state to UnsnapHover
                        otherBlock.HoverZoneId = ZoneId;
                        otherBlock.State = State.UnsnapHover;
                    }
                }
                else
                {
                    if (ZoneId > -1 && ZoneId == otherBlock.HoverZoneId)
                    {
                        Snap(other.gameObject, ZoneId);
                    }
                }
            }
        }
        private void DoChildTriggerExit(Collider other)
        {
            if (other != null && other.gameObject.tag.CompareTo(Constant.TAG_BLOCK_PROGRAM) == 0)
            {
                ProgramBlock otherBlock = other.gameObject.GetComponent<ProgramBlock>();
                if (otherBlock.IsGrabbed() && otherBlock.HoverZoneId == ZoneId && otherBlock.State == State.UnsnapHover && State == State.SnapIdle)
                {
                    otherBlock.HoverZoneId = -1;
                    Unhover(ZoneId);
                }
            }
        }

        #endregion

        #region Public methods

        public void PauseSweepChildTrigger()
        {
            GetSweepChild().PauseTrigger();
        }

        public void StartSweepChildTrigger()
        {
            GetSweepChild().StartTrigger();
        }

        public SnapDropZone GetSnapDropZone()
        {
            return GetComponentInteractableObject().GetStoredSnapDropZone().GetComponent<SnapDropZone>();
        }

        public VRTK_SnapDropZone GetVRTKSnapDropZone()
        {
            return GetComponentInteractableObject().GetStoredSnapDropZone();
        }

        public bool IsGrabbed()
        {
            return GetComponentInteractableObject().IsGrabbed();
        }

        public bool IsSnappedToZone()
        {
            return GetComponentInteractableObject().IsInSnapDropZone();
        }

        #endregion

        private VRTK_InteractableObject GetComponentInteractableObject()
        {
            return GetComponent<VRTK_InteractableObject>();
        }
        private ProgramBlockType GetProgramBlockTypeByName(string name)
        {
            if (name.Contains(Constant.NAME_PROGRAM_BLOCK_FORWARD))
            {
                return ProgramBlockType.Forward;
            }
            else if (name.Contains(Constant.NAME_PROGRAM_BLOCK_JUMP))
            {
                return ProgramBlockType.Jump;
            }
            else if (name.Contains(Constant.NAME_PROGRAM_BLOCK_ROTATELEFT))
            {
                return ProgramBlockType.RotateLeft;
            }
            return ProgramBlockType.RotateRight;
        }
        private ProgramBlock GetProgramBlockByGameObject(GameObject obj)
        {
            return obj.GetComponent<ProgramBlock>();
        }
        private SweepChildBlock GetSweepChild()
        {
            return transform.GetChild(0).GetComponent<SweepChildBlock>();
        }
    }

    public enum ProgramBlockType
    {
        Forward = 0,
        RotateLeft = 1,
        RotateRight = 2,
        Jump = 3,
        ForLoopStart = 4,
        ForLoopEnd = 5
    }

    public enum State
    {
        UnsnapIdle = 0,
        UnsnapHover = 1,
        SnapIdle = 2,
        SnapTempMove = 3
    }
}