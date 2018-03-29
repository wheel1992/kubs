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

        const string TAG_ZONE_BASE = "ZoneBase";
        const string TAG_ZONE_HINT = "ZoneHint";

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

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

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
        ZoneBaseController GetZoneBase(GameObject obj)
        {
            return obj.GetComponent<ZoneBaseController>();
        }
        ZoneHintController GetZoneHint(GameObject obj)
        {
            return obj.GetComponent<ZoneHintController>();
        }
    }
}