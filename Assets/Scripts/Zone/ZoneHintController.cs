using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public struct ZoneHintEventArgs
    {
        public GameObject CollidedObject;
    }

    /// <summary>
    /// This has a parent which is <see cref="ZoneController"></see>
    /// 
    /// This should be a transparent object which is not visible.
    /// 
    /// This should contain EventHandlers which can interact with its parent
    ///  
    /// </summary>
    public class ZoneHintController : MonoBehaviour
    {
        public delegate void ZoneHintEventHandler(object sender, ZoneHintEventArgs args);
        public event ZoneHintEventHandler OnZoneHintTriggerEnter;
        public event ZoneHintEventHandler OnZoneHintTriggerExit;
        private const string TAG_PREDICTER = "ZoneHintPredicter";
        private ZoneHintPredicterController _predicterCtrl;
        private BoxCollider _collider;

        #region Public methods

        #endregion

        #region Private methods

        void Start()
        {
            _predicterCtrl = GetZoneHintPredicterControllerByGameObject(GetZoneHintPredicterGameObject());
            _predicterCtrl.OnPredicterTriggerEnter += new ZoneHintPredicterController.ZoneHintPredicterEventHandler(HandlePredicterTriggerEnter);
            _predicterCtrl.OnPredicterTriggerExit += new ZoneHintPredicterController.ZoneHintPredicterEventHandler(HandlePredicterTriggerExit);
        }
        void Update()
        {

        }

        #endregion

        #region Private Event Handler Listener 
        private void HandlePredicterTriggerEnter(object sender, ZoneHintPredicterEventArgs args)
        {
            OnZoneHintTriggerEnter(this, new ZoneHintEventArgs { CollidedObject = args.CollidedObject });
        }
        private void HandlePredicterTriggerExit(object sender, ZoneHintPredicterEventArgs args)
        {
            OnZoneHintTriggerExit(this, new ZoneHintEventArgs { CollidedObject = args.CollidedObject });
        }

        #endregion

        #region Private Methods

        #endregion

        #region Private Get Methods
        private GameObject GetZoneHintPredicterGameObject()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag.CompareTo(TAG_PREDICTER) == 0)
                {
                    return transform.GetChild(i).gameObject;
                }
            }
            return null;
        }
        private ZoneHintPredicterController GetZoneHintPredicterControllerByGameObject(GameObject predicter)
        {
            return predicter.GetComponent<ZoneHintPredicterController>();
        }

        #endregion
    }
}