using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class ProgramBlock : Block
    {
        public delegate void HoverEventHandler(int targetZoneId);
        public delegate void PlaceEventHandler(int targetZoneId);
        public delegate void SnapEventHandler(GameObject block, int zoneId);
        public event HoverEventHandler Hover;
        public event HoverEventHandler Unhover;
        public event PlaceEventHandler Place;
        public event SnapEventHandler Snap;

        public ProgramBlockType Type { get; set; }
        public int ZoneId { get; set; }
        private Rigidbody _rb;
        private bool hasHover = false;
        private int hoverZoneId = -1;
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


            // Sweep child block by default is not triggered
            // Only triggered when it is attached
            PauseSweepChildTrigger();

            GetSweepChild().OnEnter += new SweepChildBlock.TriggerEventHandler(DoChildTriggeredEnter);
        }

        private void OnTriggerExit(Collider other)
        {
            if (isGrabbed())
            {
                if (other != null && other.gameObject.layer == LayerMask.NameToLayer(Constant.LAYER_NAME_SWEEP_TEST))
                {
                    if (hasHover)
                    {
                        hasHover = false;
                        Unhover(hoverZoneId);
                        hoverZoneId = -1;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isSnappedToZone()) { return; }
            if (other != null && other.gameObject.layer == LayerMask.NameToLayer(Constant.LAYER_NAME_SWEEP_TEST))
            {
                Debug.Log("OnTriggerEnter: isGrabbed=" + isGrabbed() + ", hasHover = " + hasHover);
                // hit collider game object is SweepTestChild
                // Get parent of the SweepTestChild 
                GameObject sweepChildParent = other.gameObject.transform.parent.gameObject;
                // Get zoneId from parent of the SweepTestChild 
                int targetZoneId = sweepChildParent.GetComponent<ProgramBlock>().ZoneId;

                if (isGrabbed())
                {
                    if (targetZoneId > -1)
                    {
                        //Debug.Log("ProgramBlock Update: hit on gameobject parent zone id " + targetZoneId);
                        if (hoverZoneId != targetZoneId)
                        {
                            Hover(sweepChildParent.GetComponent<ProgramBlock>().ZoneId);
                            hoverZoneId = targetZoneId;
                        }
                        hasHover = true;
                    }
                }
                else // Not Grab
                {
                    if (targetZoneId > -1 && hasHover)
                    {
                        Place(targetZoneId);
                        Snap(gameObject, targetZoneId);
                        hoverZoneId = -1;
                        hasHover = false;
                    }
                }

            }

            //if (isGrabbed())
            //{
            //    if (other != null)
            //    {
            //        if (other.gameObject.layer == LayerMask.NameToLayer(Constant.LAYER_NAME_SWEEP_TEST))
            //        {
            //            // hit collider game object is SweepTestChild
            //            // Get parent 
            //            GameObject sweepChildParent = other.gameObject.transform.parent.gameObject;
            //            int targetZoneId = sweepChildParent.GetComponent<ProgramBlock>().ZoneId;
            //            if (targetZoneId > -1)
            //            {
            //                Debug.Log("ProgramBlock Update: hit on gameobject parent zone id " + targetZoneId);
            //                if (hoverZoneId != targetZoneId)
            //                {
            //                    Hover(sweepChildParent.GetComponent<ProgramBlock>().ZoneId);
            //                    hoverZoneId = targetZoneId;
            //                }
            //                hasHover = true;
            //            }
            //        }
            //        //else
            //        //{

            //        //}
            //    }
            //} else
            //{
            //    // Ungrab but still collide
            //    //if (other != null)
            //    //{
            //    //    if (other.gameObject.layer == LayerMask.NameToLayer(Constant.LAYER_NAME_SWEEP_TEST))
            //    //    {
            //    //        GameObject sweepChildParent = other.gameObject.transform.parent.gameObject;
            //    //        int targetZoneId = sweepChildParent.GetComponent<ProgramBlock>().ZoneId;
            //    //        if (targetZoneId > -1)
            //    //        {
            //    //            Place(targetZoneId);
            //    //            Snap(gameObject, targetZoneId);
            //    //        }
            //    //    }
            //    //}
            //}
        }

        // Update is called once per frame
        void FixedUpdate()
        {

            //if (isGrabbed())
            //{
            //    RaycastHit hit;
            //    // Test raycast on z-axis (right) and y-axis (down) 
            //    // _rb.SweepTest(transform.forward, out hit, 2f) || 
            //    if (_rb.SweepTest(-transform.up, out hit, 2f))
            //    {
            //        if (hit.collider != null)
            //        {
            //            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Constant.LAYER_NAME_SWEEP_TEST))
            //            {
            //                // hit collider game object is SweepTestChild
            //                // Get parent 
            //                GameObject sweepChildParent = hit.collider.gameObject.transform.parent.gameObject;
            //                int targetZoneId = sweepChildParent.GetComponent<ProgramBlock>().ZoneId;
            //                if (targetZoneId > -1)
            //                {
            //                    Debug.Log("ProgramBlock Update: hit on gameobject parent zone id " + targetZoneId);
            //                    if (hoverZoneId != targetZoneId)
            //                    {
            //                        Hover(sweepChildParent.GetComponent<ProgramBlock>().ZoneId);
            //                        hoverZoneId = targetZoneId;
            //                    }
            //                    hasHover = true;
            //                }
            //            }
            //            else
            //            {
            //                if (hasHover)
            //                {
            //                    hasHover = false;
            //                    Unhover(hoverZoneId);
            //                    hoverZoneId = -1;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void DoChildTriggeredEnter(Collider other)
        {
           
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
}