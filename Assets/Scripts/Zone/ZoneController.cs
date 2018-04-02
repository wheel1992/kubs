using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public struct ZoneEventArgs
    {
        public int Index;
    }

    public struct ZoneHoverEventArgs
    {
        public IList<int> HoveredIndices;
        public int UnhoveredIndex;
    }
    /// <summary>
    /// This contains both ZoneBaseController and ZoneHintController
    /// 
    /// This should holds its current position and its index
    /// 
    /// This should handles all events between ZoneHintController
    /// and ProgramBlock
    /// </summary>
    public class ZoneController : MonoBehaviour
    {
        public delegate void ZoneEventHandler(object sender, ZoneEventArgs args);
        public delegate void ZoneHoverEventHandler(object sender, ZoneHoverEventArgs args);
        public event ZoneEventHandler OnZoneSnapped;
        public event ZoneEventHandler OnZoneUnsnapped;
        public event ZoneHoverEventHandler OnZonesHovered;
        public event ZoneHoverEventHandler OnZonesUnhovered;
        [HideInInspector]
        public bool IsOccupied = false;
        [HideInInspector]
        public bool IsTemporary = false;
        public int Index { get; set; }
        private const string TAG_ZONE_BASE = "ZoneBase";
        private const string TAG_ZONE_HINT = "ZoneHint";
        private const string TAG_ZONE_SNAP = "ZoneSnap";
        private const string TAG_ZONE_CONTAINER = "ZoneContainer";
        private const bool IS_DEBUG = true;
        private KubsDebug _debugger;
        private ZoneBaseController _zoneBaseCtrl;
        //private GameObject _zoneContainerGameObject;
        private ZoneHintController _zoneHintCtrl;
        private ZoneSnapController _zoneSnapCtrl;
        private ProgramBlock _attachedProgramBlock;

        #region Public methods

        public void Attach(ProgramBlock block)
        {
            IsOccupied = true;
            // Return result in callback HandleZoneOnSnapped
            _zoneSnapCtrl.Snap(block.gameObject);
        }
        public ProgramBlock Detach(bool isAttachedMove)
        {  
            IsOccupied = false;
            var unattachedBlock = _attachedProgramBlock;
            unattachedBlock.IsAttachedMove = isAttachedMove;
            // Return result in callback HandleZoneOnUnsnapped
            _zoneSnapCtrl.Unsnap();
            return unattachedBlock;
        }

        #endregion

        #region Private Lifecycle Methods
        void Awake()
        {
            _debugger = new KubsDebug(IS_DEBUG);
        }
        // Use this for initialization
        void Start()
        {
            _zoneHintCtrl = GetChildZoneHint();
            _zoneHintCtrl.OnZoneHintTriggerEnter += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintOnTriggerEnter);
            _zoneHintCtrl.OnZoneHintTriggerExit += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintOnTriggerExit);

            _zoneSnapCtrl = GetChildZoneSnap();
            _zoneSnapCtrl.OnEntered += new ZoneSnapController.ZoneSnapEventHandler(HandleZoneOnEntered);
            _zoneSnapCtrl.OnExited += new ZoneSnapController.ZoneSnapEventHandler(HandleZoneOnExited);
            _zoneSnapCtrl.OnSnapped += new ZoneSnapController.ZoneSnapEventHandler(HandleZoneOnSnapped);
            _zoneSnapCtrl.OnUnsnapped += new ZoneSnapController.ZoneSnapEventHandler(HandleZoneOnUnsnapped);

            _zoneBaseCtrl = GetChildZoneBase();
        }

        // Update is called once per frame
        void Update()
        {
        }

        #endregion

        #region Private Event Handler Listener 

        private void HandleZoneOnEntered(object sender, ZoneSnapEventArgs args)
        {
            Debug.Log("HandleZoneOnEntered");
        }
        private void HandleZoneOnExited(object sender, ZoneSnapEventArgs args)
        {
            Debug.Log("HandleZoneOnExited");
            if (args.WhichBlock != null)
            {
                args.WhichBlock.GetVRTKInteractableObject().validDrop = VRTK_InteractableObject.ValidDropTypes.DropAnywhere;
                // if (args.WhichBlock.State == State.Attach) {
                //     args.WhichBlock.State = State.Detach;
                // }
            }
        }
        private void HandleZoneOnSnapped(object sender, ZoneSnapEventArgs args)
        {
            Debug.Log("HandleZoneOnSnapped at " + Index);
            IsOccupied = true;
            if (args.WhichBlock != null)
            {
                args.WhichBlock.ResetCollidedZoneIndices();
                args.WhichBlock.ZoneIndex = this.Index;
                _attachedProgramBlock = args.WhichBlock;

                OnZoneSnapped(this, new ZoneEventArgs { Index = this.Index });
            }
        }
        private void HandleZoneOnUnsnapped(object sender, ZoneSnapEventArgs args)
        {
            IsOccupied = false;
            if (args.WhichBlock != null)
            {
                args.WhichBlock.GetVRTKInteractableObject().validDrop = VRTK_InteractableObject.ValidDropTypes.NoDrop;
                _attachedProgramBlock = null;
                OnZoneUnsnapped(this, new ZoneEventArgs { Index = this.Index });
            }
        }
        private void HandleZoneHintOnTriggerEnter(object sender, ZoneHintEventArgs args)
        {
            if (args.CollidedObject != null)
            {
                var collidedBlock = GetProgramBlockByGameObject(args.CollidedObject);
                // if (collidedBlock != null && !collidedBlock.HasZoneIndex())
                if (collidedBlock != null && !collidedBlock.IsAttachedMove)
                {
                    Debug.Log("HandleZoneHintOnTriggerEnter:");
                    if (collidedBlock.HasCollidedZoneIndex(this.Index))
                    {
                        // ProgramBlock keeps hovering on top of this zone
                        // Do not repeat the code below
                        return;
                    }

                    //Debug.Log("HandleZoneHintOnTriggerEnter: collided is ProgramBlock!");
                    collidedBlock.AddCollidedZoneIndex(this.Index);
                    //collidedBlock.PrintCollidedZoneIndices();

                    OnZonesHovered(this,
                        new ZoneHoverEventArgs
                        {
                            HoveredIndices = collidedBlock.GetCollidedZoneIndices(),
                            UnhoveredIndex = -1
                        });
                }
            }
        }
        private void HandleZoneHintOnTriggerExit(object sender, ZoneHintEventArgs args)
        {
            if (args.CollidedObject != null)
            {
                var collidedBlock = GetProgramBlockByGameObject(args.CollidedObject);
                // if (collidedBlock != null && collidedBlock.HasZoneIndex())
                if (collidedBlock != null && !collidedBlock.IsAttachedMove)
                {
                    Debug.Log("HandleZoneHintOnTriggerExit:");
                    //Debug.Log("HandleZoneHintOnTriggerExit: collided is ProgramBlock!");
                    collidedBlock.RemoveCollidedZoneIndex(this.Index);
                    //collidedBlock.PrintCollidedZoneIndices();

                    OnZonesUnhovered(this,
                        new ZoneHoverEventArgs
                        {
                            HoveredIndices = collidedBlock.GetCollidedZoneIndices(),
                            UnhoveredIndex = this.Index
                        });
                }
            }
        }

        #endregion

        #region Private methods
        public void SetAttachedProgramBlockPosition(Vector3 newPos)
        {
            if (_attachedProgramBlock != null)
            {
                _attachedProgramBlock.transform.position = newPos;
            }
        }
        GameObject GetGameObjectObjectByTag(string tag)
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
        ProgramBlock GetProgramBlockByGameObject(GameObject obj)
        {
            return obj.GetComponent<ProgramBlock>();
        }
        VRTK_InteractableObject GetVRTKInteractableObject(GameObject obj)
        {
            return obj.GetComponent<VRTK_InteractableObject>();
        }
        ZoneBaseController GetChildZoneBase()
        {
            return GetChildByTagName(TAG_ZONE_BASE).GetComponent<ZoneBaseController>();
        }
        ZoneHintController GetChildZoneHint()
        {
            return GetChildByTagName(TAG_ZONE_HINT).GetComponent<ZoneHintController>();
        }
        ZoneSnapController GetChildZoneSnap()
        {
            return GetChildByTagName(TAG_ZONE_SNAP).GetComponent<ZoneSnapController>();
        }
        GameObject GetChildByTagName(string tagName)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag.CompareTo(tagName) == 0)
                {
                    return transform.GetChild(i).gameObject;
                }
            }
            return null;
        }

        #endregion
    }

}