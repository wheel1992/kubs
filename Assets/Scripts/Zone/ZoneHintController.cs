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
        [SerializeField] private Material materialValid;
        [SerializeField] private Material materialInvalid;

        public delegate void ZoneHintEventHandler(object sender, ZoneHintEventArgs args);
        public event ZoneHintEventHandler OnZoneHintTriggerEnter;
        public event ZoneHintEventHandler OnZoneHintTriggerExit;
        private const string TAG_PREDICTER = "ZoneHintPredicter";
        private const string TAG_HIGHLIGHTED_PREFAB_OBJECT = "ZoneHintHighlightedObject";
        private ZoneHintPredicterController _predicterCtrl;
        private BoxCollider _collider;

        #region Public methods

        public void DisplayHighlight(bool isValid)
        {
            Debug.Log("DisplayHighlight: isValid " + isValid);
            var prefab = GetZoneHintHighlightPrefabGameObject();
            prefab.SetActive(true);
            var prefabMeshRenderer = prefab.GetComponent<MeshRenderer>();
            if (prefabMeshRenderer != null)
            {
                if (isValid)
                {
                    prefabMeshRenderer.material = materialValid;
                }
                else
                {
                    prefabMeshRenderer.material = materialInvalid;
                }
            }
        }
        public void HideHighlight()
        {
            Debug.Log("HideHighlight: ");
            var prefab = GetZoneHintHighlightPrefabGameObject();
            prefab.SetActive(false);
        }

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
        private GameObject GetZoneHintHighlightPrefabGameObject()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag.CompareTo(TAG_HIGHLIGHTED_PREFAB_OBJECT) == 0)
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