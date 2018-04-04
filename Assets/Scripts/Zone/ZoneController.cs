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
        public GameObject CollidedObject;
        public int ZoneIndex;
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

        #region Public methods

        public void AttachBlock(ProgramBlock block)
        {
            Debug.Log("AttachBlock: " + block.name);
            GetChildZoneSnap().Snap(block.gameObject);
        }
        public void DisableSnap()
        {
            GetChildZoneSnap().Disable();
        }
        public void EnableSnap()
        {
            GetChildZoneSnap().Enable();
        }
        public ProgramBlock GetAttachedProgramBlock()
        {
            var blockObj = GetChildByTagName(Constant.TAG_BLOCK_PROGRAM);
            if (blockObj == null) { return null; }
            return GetProgramBlockByGameObject(blockObj);
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
            //Debug.Log("HandleZoneOnEntered");
        }
        private void HandleZoneOnExited(object sender, ZoneSnapEventArgs args)
        {
            //Debug.Log("HandleZoneOnExited");
            if (args.WhichBlock != null)
            {
                args.WhichBlock.GetVRTKInteractableObject().validDrop = VRTK_InteractableObject.ValidDropTypes.DropAnywhere;
            }
        }
        private void HandleZoneOnSnapped(object sender, ZoneSnapEventArgs args)
        {
            IsOccupied = true;
            if (args.WhichBlock != null)
            {
                args.WhichBlock.CollidedZoneIndex = -1;
                args.WhichBlock.ZoneIndex = this.Index;
                // Attach block (child) to this zone (parent)
                args.WhichBlock.SetParent(this.transform);

                OnZoneSnapped(this,
                    new ZoneEventArgs
                    {
                        Index = this.Index
                    });
            }
        }
        private void HandleZoneOnUnsnapped(object sender, ZoneSnapEventArgs args)
        {
            IsOccupied = false;
            if (args.WhichBlock != null)
            {
                args.WhichBlock.GetVRTKInteractableObject().validDrop = VRTK_InteractableObject.ValidDropTypes.NoDrop;
                args.WhichBlock.SetParent(null);

                OnZoneUnsnapped(this,
                    new ZoneEventArgs
                    {
                        Index = this.Index
                    });
            }
        }
        private void HandleZoneHintOnTriggerEnter(object sender, ZoneHintEventArgs args)
        {
            if (args.CollidedObject != null)
            {
                var collidedBlock = GetProgramBlockByGameObject(args.CollidedObject);
                if (collidedBlock != null)
                {
                    if (collidedBlock.CollidedZoneIndex == Index)
                    {
                        return;
                    }

                    collidedBlock.CollidedZoneIndex = Index;

                    OnZonesHovered(this,
                        new ZoneHoverEventArgs
                        {
                            CollidedObject = args.CollidedObject,
                            ZoneIndex = collidedBlock.CollidedZoneIndex
                        });
                }
            }
        }
        private void HandleZoneHintOnTriggerExit(object sender, ZoneHintEventArgs args)
        {
            if (args.CollidedObject != null)
            {
                var collidedBlock = GetProgramBlockByGameObject(args.CollidedObject);
                if (collidedBlock != null)
                {
                    collidedBlock.CollidedZoneIndex = -1;
                    OnZonesUnhovered(this,
                        new ZoneHoverEventArgs
                        {
                            CollidedObject = args.CollidedObject,
                            ZoneIndex = Index
                        });
                }
            }
        }

        #endregion

        #region Private methods
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