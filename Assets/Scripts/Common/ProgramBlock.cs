using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class ProgramBlock : Block
    {
        public delegate void HoverEventHandler(ProgramBlock block);
        public event HoverEventHandler Hover;

        public ProgramBlockType Type { get; set; }
        public int ZoneId { get; set; }
        private SweepChildBlock _sweepChild;
        private Rigidbody _rb;

        #region Lifecycle

        void Awake()
        {
            Category = BlockCategory.Program;
            ZoneId = -1;
        }

        // Use this for initialization
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            //_sweepChild = transform.GetChild(0).GetComponent<SweepChildBlock>();
            //_sweepChild.OnEnter += new SweepChildBlock.TriggerEventHandler(DoChildTriggeredEnter);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isGrabbed())
            {
                RaycastHit hit;
                // Test raycast on z-axis (right) and y-axis (down) 
                if (_rb.SweepTest(transform.forward, out hit, 2f) || _rb.SweepTest(-transform.up, out hit, 5f))
                {
                    if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer(Constant.LAYER_NAME_SWEEP_TEST))
                    {
                        // hit collider game object is SweepTestChild
                        // Get parent 
                        //Debug.Log("ProgramBlock Update: hit on gameobject " + hit.collider.gameObject.tag);
                        //Debug.Log("ProgramBlock Update: hit on gameobject parent zone id " + hit.collider.gameObject.transform.parent.GetComponent<ProgramBlock>().ZoneId);

                        GameObject sweepChildParent = hit.collider.gameObject.transform.parent.gameObject;
                        int targetZoneId = sweepChildParent.GetComponent<ProgramBlock>().ZoneId;
                        if (targetZoneId > -1)
                        {
                            Debug.Log("ProgramBlock Update: hit on gameobject parent zone id " + targetZoneId);
                            Hover(sweepChildParent.GetComponent<ProgramBlock>());
                        }

                    }
                }
            }
        }

        private void DoChildTriggeredEnter(Collider other)
        {
            //Debug.Log("ProgramBlock OnChildTriggeredEnter: collider at layer = " + other.gameObject.layer); 
        }

        #endregion

        #region Public methods

        public SnapDropZone GetSnapDropZone()
        {
            return getComponentInteractableObject().GetStoredSnapDropZone().GetComponent<SnapDropZone>();
        }

        public VRTK_SnapDropZone GetVRTKSnapDropZone()
        {
            return getComponentInteractableObject().GetStoredSnapDropZone();
        }

        public bool isGrabbed()
        {
            return getComponentInteractableObject().IsGrabbed();
        }

        public bool isSnappedToZone()
        {
            return getComponentInteractableObject().IsInSnapDropZone();
        }

        #endregion

        private VRTK_InteractableObject getComponentInteractableObject()
        {
            return GetComponent<VRTK_InteractableObject>();
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
}