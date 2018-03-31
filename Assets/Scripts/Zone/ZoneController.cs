using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public struct ZoneEventArgs
    {
        //public GameObject OtherObject;
        public int Index;
    }

    public struct ZoneHoverEventArgs
    {
        //public GameObject OtherObject;
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

        const string TAG_ZONE_BASE = "ZoneBase";
        const string TAG_ZONE_HINT = "ZoneHint";
        const string TAG_ZONE_SNAP = "ZoneSnap";
        const string TAG_ZONE_CONTAINER = "ZoneContainer";

        const bool IS_DEBUG = true;

        public int Index { get; set; }

        private KubsDebug _debugger;

        private ZoneBaseController _zoneBaseCtrl;
        //private GameObject _zoneContainerGameObject;
        private ZoneHintController _zoneHintCtrl;
        private ZoneSnapController _zoneSnapCtrl;
        private ProgramBlock _attachedProgramBlock;

        #region Public methods

        public void Attach(ProgramBlock block)
        {
            _zoneSnapCtrl.Snap(block.gameObject);
        }
        public ProgramBlock Detach()
        {
            var unattachedBlock = _attachedProgramBlock;
            _zoneSnapCtrl.Unsnap();
            return unattachedBlock;
        }


        // public bool Attach(GameObject obj)
        // {
        //     _debugger.Log("Attach: ");
        //     if (obj == null)
        //     {
        //         isOccupied = false;
        //         throw new ArgumentException("Attach: GameObject parameter is null");
        //     }

        //     var interactableObject = GetVRTKInteractableObject(obj);
        //     if (interactableObject == null)
        //     {
        //         throw new NullReferenceException("Attach: GameObject does not have VRTK_InteractableObject script");
        //     }

        //     obj.transform.position = GetChildZoneHint().transform.position;
        //     isOccupied = true;
        //     return true;
        // }
        // public bool Detach()
        // {
        //     // Unparent attached object from here
        //     // ...
        //     // _attachedObject = null;
        //     // isOccupied = false;
        //     return true;
        // }
        // public GameObject GetAttachedObject()
        // {
        //     return _attachedObject;
        // }

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

            //_zoneContainerGameObject = GetChildByTagName(TAG_ZONE_CONTAINER);
        }

        // Update is called once per frame
        void Update()
        {
        }

        #endregion

        #region Private Event Handler Listener 

        private void HandleZoneOnEntered(object sender, ZoneSnapEventArgs args)
        {

        }
        private void HandleZoneOnExited(object sender, ZoneSnapEventArgs args)
        {
            if (args.WhichBlock != null)
            {
                args.WhichBlock.GetVRTKInteractableObject().validDrop = VRTK_InteractableObject.ValidDropTypes.DropAnywhere;
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
            }
            OnZoneSnapped(this, new ZoneEventArgs { Index = this.Index });
        }
        private void HandleZoneOnUnsnapped(object sender, ZoneSnapEventArgs args)
        {
            IsOccupied = false;
            if (args.WhichBlock != null)
            {
                args.WhichBlock.GetVRTKInteractableObject().validDrop = VRTK_InteractableObject.ValidDropTypes.NoDrop;
            }
            _attachedProgramBlock = null;
            OnZoneUnsnapped(this, new ZoneEventArgs { Index = this.Index });
        }
        private void HandleZoneHintOnTriggerEnter(object sender, ZoneHintEventArgs args)
        {
            if (args.CollidedObject != null)
            {
                var collidedBlock = GetProgramBlockByGameObject(args.CollidedObject);
                if (collidedBlock != null && !collidedBlock.HasZoneIndex())
                {
                    if (collidedBlock.HasCollidedZoneIndex(this.Index))
                    {
                        // ProgramBlock keeps hovering on top of this zone
                        // Do not repeat the code below
                        return;
                    }

                    //Debug.Log("HandleZoneHintOnTriggerEnter: collided is ProgramBlock!");
                    collidedBlock.AddCollidedZoneIndex(this.Index);
                    collidedBlock.PrintCollidedZoneIndices();

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
                if (collidedBlock != null)
                {
                    //Debug.Log("HandleZoneHintOnTriggerExit: collided is ProgramBlock!");
                    collidedBlock.RemoveCollidedZoneIndex(this.Index);
                    collidedBlock.PrintCollidedZoneIndices();

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

        // private IEnumerator AttemptAttach(GameObject objToAttach)
        // {
        //     _debugger.Log("AttemptAttach");
        //     yield return new WaitForEndOfFrame();
        //     // Do the attach block to zone here
        //     // ...
        // }
        // private void CheckRigidBody()
        // {
        //     if (GetComponent<Rigidbody>() == null)
        //     {
        //         gameObject.AddComponent<Rigidbody>();
        //     }
        // }
        // private void HandleZoneHintDisplay(object sender, ZoneHintEventArgs args)
        // {
        // }
        // private void HandleZoneHintTriggerEnter(object sender, ZoneHintEventArgs args)
        // {
        //     _debugger.Log("HandleZoneHintTriggerEnter");
        //     if (sender != null && sender is ZoneHintController)
        //     {
        //         //var zoneHint = ((GameObject)sender).GetComponent<ZoneHintController>();
        //         _debugger.Log("HandleZoneHintTriggerEnter: other object = " + args.OtherObject.name);
        //         if (!isOccupied &&
        //             args.OtherObject != null &&
        //             args.OtherObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_FORWARD) == 0)
        //         {
        //             Attach(args.OtherObject);
        //         }
        //     }
        // }
        // private void HandleZoneHintTriggerExit(object sender, ZoneHintEventArgs args)
        // {

        // }
        // private void RegisterZoneHintEvents()
        // {
        //     GetChildZoneHint().OnZoneHintDisplay += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintDisplay);
        //     GetChildZoneHint().OnZoneHintTriggerEnter += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintTriggerEnter);
        //     GetChildZoneHint().OnZoneHintTriggerExit += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintTriggerExit);
        // }

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
        // private Vector3 GetPosition()
        // {
        //     return transform.position;
        // }

        #endregion
    }

}