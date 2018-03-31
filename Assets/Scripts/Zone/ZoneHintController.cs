using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public struct ZoneHintEventArgs
    {
        public GameObject OtherObject;
    }
    public class ZoneHintController : MonoBehaviour
    {
        /// <summary>
        /// This has a parent which is <see cref="ZoneController"></see>
        /// This should contain a box collider which detects
        /// a ProgramBlock when the block hovers over this hint.
        /// 
        /// This should be a transparent object which is not visible.
        /// 
        /// This should contain EventHandlers which can interact with its parent
        ///  
        /// </summary>



        public delegate void ZoneHintEventHandler(object sender, ZoneHintEventArgs args);
        public event ZoneHintEventHandler OnZoneHintTriggerEnter;
        public event ZoneHintEventHandler OnZoneHintTriggerExit;
        public event ZoneHintEventHandler OnZoneHintDisplay;

        private BoxCollider _collider;


        #region Public methods

        #endregion

        #region Private methods
        // Use this for initialization
        void Start()
        {
            //CheckRigidBody();
            CheckBoxCollider();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerExit(Collider other)
        {
            OnZoneHintTriggerExit(this, new ZoneHintEventArgs { OtherObject = other.gameObject });
        }
        void OnTriggerEnter(Collider other)
        {
            OnZoneHintTriggerEnter(this, new ZoneHintEventArgs { OtherObject = other.gameObject });
        }

        #endregion

        #region Private Methods
        private void CheckRigidBody()
        {
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
            }
        }
        private void CheckBoxCollider()
        {
            _collider = GetComponent<BoxCollider>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<BoxCollider>();
                _collider.isTrigger = true;
            }
        }

        #endregion

    }
}