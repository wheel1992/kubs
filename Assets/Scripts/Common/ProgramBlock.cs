using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class ProgramBlock : Block
    {
        public ProgramBlockType Type { get; set; }
        [HideInInspector]
        // public int ZoneIndex = -1;
        public int CollidedZoneIndex { get; set; }

        #region Public Methods
        public void SetActive()
        {
            gameObject.SetActive(true);
        }
        public void SetInactive()
        {
            gameObject.SetActive(false);
        }
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        public VRTK_InteractableObject GetVRTKInteractableObject()
        {
            return gameObject.GetComponent<VRTK_InteractableObject>();
        }
        public GameObject GetParent()
        {
            return transform.parent.gameObject;
        }
        public bool IsInZone()
        {
            // Debug.Log("HasZoneIndex: ZoneIndex = " + ZoneIndex);
            // return ZoneIndex != -1;
            var zoneIndex = GetZoneIndex();
            return zoneIndex != -1;
        }
        public int GetZoneIndex()
        {
            var parent = GetParent();
            if (parent == null) { return -1; }

            // Check instance is in ZoneController
            var zoneCtrl = parent.GetComponent<ZoneController>();
            if (zoneCtrl == null) { return -1; }

            return zoneCtrl.Index;
        }
        #endregion

        #region Private Lifecycle Methods
        void Awake()
        {
            Category = BlockCategory.Program;
        }
        void Start()
        {
            // _collidedZoneIndices = new List<int>();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag.CompareTo(Constant.TAG_BLOCK_SWEEP_TEST_CHILD) == 0)
                {
                    transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled = false;
                }
            }

            GetVRTKInteractableObject().InteractableObjectUngrabbed += new InteractableObjectEventHandler(HandleOnUngrabbed);

            CollidedZoneIndex = -1;
            // ZoneIndex = -1;
            DetermineType();
        }
        private void Update()
        {
            DetermineType();
        }

        #endregion

        #region Private Event Listeners
        private void HandleOnUngrabbed(object sender, InteractableObjectEventArgs args)
        {
            if (sender is VRTK_InteractableObject)
            {
                var block = ((VRTK_InteractableObject)sender).gameObject.GetComponent<ProgramBlock>();
                if (block != null && block.Type == ProgramBlockType.ForLoopStart)
                {
                    EventManager.TriggerEvent(Constant.EVENT_NAME_FOR_LOOP_START_UNGRAB, sender);
                }
            }
        }

        #endregion

        #region Private Methods

        private void DetermineType()
        {
            if (gameObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_FORWARD) == 0)
            {
                Type = ProgramBlockType.Forward;
            }
            else if (gameObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_JUMP) == 0)
            {
                Type = ProgramBlockType.Jump;
            }
            else if (gameObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_ROTATELEFT) == 0)
            {
                Type = ProgramBlockType.RotateLeft;
            }
            else if (gameObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_ROTATERIGHT) == 0)
            {
                Type = ProgramBlockType.RotateRight;
            }
            else if (gameObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_FORLOOPSTART) == 0)
            {
                Type = ProgramBlockType.ForLoopStart;
            }
            else if (gameObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_FORLOOPEND) == 0)
            {
                Type = ProgramBlockType.ForLoopEnd;
            }
        }

        #endregion
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