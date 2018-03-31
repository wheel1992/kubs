using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class ZoneController : MonoBehaviour
    {
        /// <summary>
        /// This contains both ZoneBaseController and 
		/// ZoneHintController
		/// 
		/// This should contain attached ProgramBlock as a child
		///
		/// This should holds its current position and its index
		/// 
		/// This should handles all events between ZoneHintController
		/// and ProgramBlock
        /// </summary>

        //public delegate void ZoneEventHandler(object sender, ZoneGroupEventArgs args);

        const string TAG_ZONE_BASE = "ZoneBase"; const string TAG_ZONE_HINT = "ZoneHint";
        const bool IS_DEBUG = true;

        public int Index { get; set; }

        private KubsDebug _debugger;
        private GameObject _attachedObject;
        private bool isOccupied = false;

        #region Public methods

        public bool Attach(GameObject obj)
        {
            _debugger.Log("Attach: ");
            if (obj == null)
            {
                isOccupied = false;
                throw new ArgumentException("Attach: GameObject parameter is null");
            }

            var interactableObject = GetVRTKInteractableObject(obj);
            if (interactableObject == null)
            {
                throw new NullReferenceException("Attach: GameObject does not have VRTK_InteractableObject script");
            }

            obj.transform.position = GetChildZoneHint().transform.position;
            isOccupied = true;
            return true;
        }
        public bool Detach()
        {
            // Unparent attached object from here
            // ...
            _attachedObject = null;
            isOccupied = false;
            return true;
        }
        public GameObject GetAttachedObject()
        {
            return _attachedObject;
        }

        #endregion

        #region Private Lifecycle methods
        void Awake()
        {
            _debugger = new KubsDebug(IS_DEBUG);
        }
        // Use this for initialization
        void Start()
        {
            RegisterZoneHintEvents();
            CheckRigidBody();
        }

        // Update is called once per frame
        void Update()
        {
        }

        #endregion

        #region Private methods

        private IEnumerator AttemptAttach(GameObject objToAttach)
        {
            _debugger.Log("AttemptAttach");
            yield return new WaitForEndOfFrame();
            // Do the attach block to zone here
            // ...
        }
        private void CheckRigidBody()
        {
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
            }
        }
        private void HandleZoneHintDisplay(object sender, ZoneHintEventArgs args)
        {

        }
        private void HandleZoneHintTriggerEnter(object sender, ZoneHintEventArgs args)
        {
            _debugger.Log("HandleZoneHintTriggerEnter");
            if (sender != null && sender is ZoneHintController)
            {
                //var zoneHint = ((GameObject)sender).GetComponent<ZoneHintController>();
                _debugger.Log("HandleZoneHintTriggerEnter: other object = " + args.OtherObject.name);
                if (!isOccupied &&
                    args.OtherObject != null &&
                    args.OtherObject.name.CompareTo(Constant.NAME_PROGRAM_BLOCK_FORWARD) == 0)
                {
                    Attach(args.OtherObject);
                }
            }
        }
        private void HandleZoneHintTriggerExit(object sender, ZoneHintEventArgs args)
        {

        }
        private void RegisterZoneHintEvents()
        {
            GetChildZoneHint().OnZoneHintDisplay += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintDisplay);
            GetChildZoneHint().OnZoneHintTriggerEnter += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintTriggerEnter);
            GetChildZoneHint().OnZoneHintTriggerExit += new ZoneHintController.ZoneHintEventHandler(HandleZoneHintTriggerExit);
        }

        GameObject GetGameObjectObjectByTag(string tag)
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
        ProgramBlock GetAttachedProgramBlock(GameObject obj)
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
        private Vector3 GetPosition()
        {
            return transform.position;
        }

        #endregion
    }
}