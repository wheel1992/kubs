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

        const string TAG_ZONE_BASE = "ZoneBase"; const string TAG_ZONE_HINT = "ZoneHint";

        #region Public methods

        public bool Attach(GameObject obj)
        {
            var interactObj = GetVRTKInteractableObject(obj);

            return true;
        }

        public bool Detach()
        {
            return true;
        }

        #endregion

        #region Private methods

        // Use this for initialization
        void Start()
        {
            RegisterZoneHintEvents();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        private void HandleZoneHintDisplay(object sender, ZoneHintEventArgs otherObject)
        {

        }
        private void HandleZoneHintTriggerEnter(object sender, ZoneHintEventArgs otherObject)
        {

        }
        private void HandleZoneHintTriggerExit(object sender, ZoneHintEventArgs otherObject)
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
            return GetChildByName(TAG_ZONE_BASE).GetComponent<ZoneBaseController>();
        }
        ZoneHintController GetChildZoneHint()
        {
            return GetChildByName(TAG_ZONE_HINT).GetComponent<ZoneHintController>();
        }
        GameObject GetChildByName(string tagName)
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
    }
}