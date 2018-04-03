using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class ProgramBlock : Block
    {
        // public delegate void HoverEventHandler(int targetZoneId);
        // public delegate void SnapEventHandler(GameObject block, int zoneId);
        // public event HoverEventHandler Hover;
        // public event HoverEventHandler Unhover;
        // public event SnapEventHandler Snap;
        public ProgramBlockType Type { get; set; }

        // public int ZoneId { get; set; }
        // [HideInInspector]
        // public int HoverZoneId = -1;

        // v2 here
        // public State State { get; set; }
        [HideInInspector]
        public int ZoneIndex = -1;
        [HideInInspector]
        public bool IsAttachedMove = false;
        private List<int> _collidedZoneIndices;

        #region Lifecycle
        void Awake()
        {
            Category = BlockCategory.Program;
        }
        void Start()
        {
            _collidedZoneIndices = new List<int>();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag.CompareTo(Constant.TAG_BLOCK_SWEEP_TEST_CHILD) == 0)
                {
                    transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = false;
                }
            }
            
            //State = State.Detach;
            IsAttachedMove = false;
        }
        private void Update()
        {
        }

        #endregion

        public void AddCollidedZoneIndex(int index)
        {
            foreach (int item in _collidedZoneIndices)
            {
                if (item == index)
                {
                    // Found there exists the similar index value
                    // Break the loop
                    return;
                }
            }
            Debug.Log("AddCollidedZoneIndex " + index);
            _collidedZoneIndices.Add(index);
        }
        public void RemoveCollidedZoneIndex(int index)
        {
            Debug.Log("RemoveCollidedZoneIndex " + index);
            foreach (int item in _collidedZoneIndices)
            {
                if (item == index)
                {
                    var a = _collidedZoneIndices.IndexOf(item);
                    _collidedZoneIndices.RemoveAt(a);
                    break;
                }
            }
        }
        public void ResetCollidedZoneIndices()
        {
            _collidedZoneIndices = new List<int>();
        }
        public void ResetZoneIndex()
        {
            ZoneIndex = -1;
        }
        public void PrintCollidedZoneIndices()
        {
            string msg = "";
            foreach (int item in _collidedZoneIndices)
            {
                msg += item + ", ";
            }
            Debug.Log(msg);
        }
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        public List<int> GetCollidedZoneIndices()
        {
            return _collidedZoneIndices;
        }
        public VRTK_InteractableObject GetVRTKInteractableObject()
        {
            return gameObject.GetComponent<VRTK_InteractableObject>();
        }
        public bool HasCollidedZoneIndex(int index)
        {
            foreach (int item in _collidedZoneIndices)
            {
                if (item == index)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasZoneIndex()
        {
            Debug.Log("HasZoneIndex: ZoneIndex = " + ZoneIndex);
            return ZoneIndex != -1;
        }

        // void Awake()
        // {
        //     Category = BlockCategory.Program;
        //     ZoneId = -1;
        //     State = State.UnsnapIdle;
        // }

        // // Use this for initialization
        // void Start()
        // {
        //     //Debug.Log("ProgramBlock Start");
        //     _rb = GetComponent<Rigidbody>();
        //     _interactableObject = GetComponentInteractableObject();

        //     // Sweep child block by default is not triggered
        //     // Only triggered when it is attached
        //     PauseSweepChildTrigger();

        //     GetSweepChild().OnEnter += new SweepChildBlock.TriggerEventHandler(DoChildTriggeredEnter);
        //     GetSweepChild().OnExit += new SweepChildBlock.TriggerEventHandler(DoChildTriggerExit);

        //     // Fire off event
        //     // To register block program events such as Hover, Unhover and Snap
        //     EventManager.TriggerEvent(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_HOVER_EVENT, gameObject);
        //     EventManager.TriggerEvent(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_SNAP_EVENT, gameObject);

        //     // Set its type again by checking its name
        //     GetProgramBlockByGameObject(gameObject).Type = GetProgramBlockTypeByName(gameObject.name);
        // }
        // private void OnTriggerExit(Collider other)
        // {
        //     if (other == null) { return; }
        //     // I am an Unsnap BlockProgram
        //     if (State == State.UnsnapHover)
        //     {
        //         if (other.gameObject.tag.CompareTo(Constant.TAG_TEMPORARY_POSITION_OBJECT) == 0)
        //         {
        //             // I am exiting from current zone!
        //             Unhover(HoverZoneId);
        //             HoverZoneId = -1;
        //         }
        //     }
        // }
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other == null) { return; }
        //     // I am an Unsnap BlockProgram
        //     if (State == State.UnsnapHover)
        //     {
        //         if (other.gameObject.tag.CompareTo(Constant.TAG_TEMPORARY_POSITION_OBJECT) == 0)
        //         {
        //             // I am still colliding at the current zone!
        //         }
        //     }
        // }
        // private void Update()
        // {
        //     // Only blocks with isCloned will check for SnapDropZone
        //     if (GetComponentInteractableObject().IsInSnapDropZone() && GetVRTKSnapDropZone().CompareTag(Constant.TAG_SNAP_DROP_ZONE_PLATE))
        //     {
        //         StartSweepChildTrigger();
        //         ZoneId = GetSnapDropZone().ZoneId;
        //     }
        //     else
        //     {
        //         PauseSweepChildTrigger();
        //         ZoneId = -1;
        //     }
        // }
        // private void DoChildTriggeredEnter(Collider other)
        // {
        //     // My SweepTestChild collide with other.gameobject which is a ProgramBlock
        //     if (other != null && other.gameObject.tag.CompareTo(Constant.TAG_BLOCK_PROGRAM) == 0)
        //     {
        //         ProgramBlock otherBlock = other.gameObject.GetComponent<ProgramBlock>();

        //         if (otherBlock.IsGrabbed())
        //         {
        //             Debug.Log("DoChildTriggeredEnter: other program block HoverZoneId " + otherBlock.HoverZoneId);
        //             if (ZoneId > -1 && ZoneId != otherBlock.HoverZoneId)
        //             {
        //                 if (otherBlock.HoverZoneId > -1)
        //                 {
        //                     // has hovered on other zone currently
        //                     Unhover(otherBlock.HoverZoneId);
        //                     otherBlock.HoverZoneId = -1;
        //                 }

        //                 Hover(ZoneId);
        //                 // Set my current state to SnapTempMove
        //                 State = State.SnapTempMove;

        //                 // Set other block HoverZoneId and state to UnsnapHover
        //                 otherBlock.HoverZoneId = ZoneId;
        //                 otherBlock.State = State.UnsnapHover;
        //             }
        //         }
        //         else
        //         {
        //             if (ZoneId > -1 && ZoneId == otherBlock.HoverZoneId)
        //             {
        //                 Snap(other.gameObject, ZoneId);
        //             }
        //         }
        //     }
        // }
        // private void DoChildTriggerExit(Collider other)
        // {
        //     if (other != null && other.gameObject.tag.CompareTo(Constant.TAG_BLOCK_PROGRAM) == 0)
        //     {
        //         ProgramBlock otherBlock = other.gameObject.GetComponent<ProgramBlock>();
        //         if (otherBlock.IsGrabbed() && otherBlock.HoverZoneId == ZoneId && otherBlock.State == State.UnsnapHover && State == State.SnapIdle)
        //         {
        //             otherBlock.HoverZoneId = -1;
        //             Unhover(ZoneId);
        //         }
        //     }
        // }

        // #endregion

        // #region Public methods

        // public void PauseSweepChildTrigger()
        // {
        //     var child = GetSweepChild();
        //     if (child != null)
        //     {
        //         child.PauseTrigger();
        //     }
        // }

        // public void StartSweepChildTrigger()
        // {
        //     var child = GetSweepChild();
        //     if (child != null)
        //     {
        //         child.StartTrigger();
        //     }
        // }

        // public SnapDropZone GetSnapDropZone()
        // {
        //     return GetComponentInteractableObject().GetStoredSnapDropZone().GetComponent<SnapDropZone>();
        // }

        // public VRTK_SnapDropZone GetVRTKSnapDropZone()
        // {
        //     return GetComponentInteractableObject().GetStoredSnapDropZone();
        // }

        // public bool IsGrabbed()
        // {
        //     return GetComponentInteractableObject().IsGrabbed();
        // }

        // public bool IsSnappedToZone()
        // {
        //     return GetComponentInteractableObject().IsInSnapDropZone();
        // }

        // #endregion


        // private ProgramBlockType GetProgramBlockTypeByName(string name)
        // {
        //     if (name.Contains(Constant.NAME_PROGRAM_BLOCK_FORWARD))
        //     {
        //         return ProgramBlockType.Forward;
        //     }
        //     else if (name.Contains(Constant.NAME_PROGRAM_BLOCK_JUMP))
        //     {
        //         return ProgramBlockType.Jump;
        //     }
        //     else if (name.Contains(Constant.NAME_PROGRAM_BLOCK_ROTATELEFT))
        //     {
        //         return ProgramBlockType.RotateLeft;
        //     }
        //     return ProgramBlockType.RotateRight;
        // }
        // private ProgramBlock GetProgramBlockByGameObject(GameObject obj)
        // {
        //     return obj.GetComponent<ProgramBlock>();
        // }
        // private SweepChildBlock GetSweepChild()
        // {
        //     for (int i = 0; i < transform.childCount; i++)
        //     {
        //         if (transform.GetChild(i).gameObject.tag.CompareTo(Constant.TAG_BLOCK_SWEEP_TEST_CHILD) == 0)
        //         {
        //             return transform.GetChild(i).gameObject.GetComponent<SweepChildBlock>();
        //         }

        //     }
        //     return null;
        // }
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
        // UnsnapIdle = 0,
        // UnsnapHover = 1,
        Detach = 1,
        Attach = 2,
        AttachMove = 3
    }
}