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
        public int ZoneIndex = -1;
        public int CollidedZoneIndex { get; set; }

        #region Public Methods

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        public VRTK_InteractableObject GetVRTKInteractableObject()
        {
            return gameObject.GetComponent<VRTK_InteractableObject>();
        }
        public bool HasZoneIndex()
        {
            Debug.Log("HasZoneIndex: ZoneIndex = " + ZoneIndex);
            return ZoneIndex != -1;
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

            //GetVRTKInteractableObject().InteractableObjectGrabbed += new InteractableObjectEventHandler(HandleOnGrabbed);

            CollidedZoneIndex = -1;
        }
        private void Update()
        {
        }

        // private void HandleOnGrabbed(object sender, InteractableObjectEventArgs args)
        // {
        //     //ResetCollidedZoneIndices();
        // }

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