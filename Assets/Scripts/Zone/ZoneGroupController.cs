using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public struct ZoneGroupEventArgs
    {
        //public GameObject OtherObject;
        public int Index;
    }
    public class ZoneGroupController : MonoBehaviour
    {
        //public delegate void ZoneGroupEventHandler(object sender, ZoneGroupEventArgs args);

        [SerializeField] private GameObject zonePrefab;

        private List<GameObject> _zones;

        private const int INDEX_DEFAULT_CHILD_ZONE = 0;

        // Use this for initialization
        void Start()
        {
            _zones = new List<GameObject>();
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Private Event Handler Listener 
        private void HandleZoneSnapped(object sender, ZoneEventArgs args)
        {
            // if (isNextZoneNull(args.Index)) {
            //     AddNextZone(args.Index);
            // }
        }
        private void HandleZoneUnsnapped(object sender, ZoneEventArgs args)
        {

        }
        #endregion

        #region Private Initization Methods
        private void Init()
        {
            // By default, there's one child Zone in ZoneGroup
            var _defaultZone = GetZoneControllerByGameObject(GetChildAt(INDEX_DEFAULT_CHILD_ZONE));
            RegisterZoneEventHandler(_defaultZone);
        }

        private void RegisterZoneEventHandler(ZoneController zone)
        {
            zone.OnZoneSnapped += new ZoneController.ZoneEventHandler(HandleZoneSnapped);
            zone.OnZoneUnsnapped += new ZoneController.ZoneEventHandler(HandleZoneUnsnapped);
        }

        #endregion

        // void AddZone(GameObject zoneObject)
        // {
        //     if (zoneObject != null)
        //     {
        //         _zones.Add(zoneObject);
        //         GetZoneByGameObject(zoneObject).Index = _zones.IndexOf(zoneObject);
        //     }
        // }
        private void AddNextZone(int currentIndex)
        {
            var nextIndex = currentIndex + 1;
            var currentZone = _zones[currentIndex];
            var currentZonePosition = currentZone.transform.position;
            var nextPosition = new Vector3(
                currentZonePosition.x,
                currentZonePosition.y,
                currentZonePosition.z + currentZone.transform.localScale.z);

            var nextZone = CreateZoneGameObject(nextPosition, nextIndex);
            RegisterZoneEventHandler(GetZoneControllerByGameObject(nextZone));

            _zones.Insert(nextIndex, nextZone);
        }
        private void UpdateZoneIndices() {
            for(int i = 0; i < _zones.Count; i++) {
                var zone = GetZoneControllerByGameObject(_zones[i]);
                zone.Index = i;
            }
        }
        private GameObject CreateZoneGameObject(Vector3 availablePosition, int availableIndex)
        {
            var zone = (GameObject)Instantiate(
             zonePrefab,
             availablePosition,
             Quaternion.identity);

            var controller = zone.AddComponent<ZoneController>();
            controller.Index = availableIndex;

            return zone;
        }
        private bool RemoveZone(int index)
        {
            return false;
        }


        #region Private Get Methods

        private GameObject GetChildAt(int index)
        {
            if (transform.childCount == 0) { return null; }
            return transform.GetChild(index).gameObject;
        }

        private ZoneController GetZoneControllerByGameObject(GameObject obj)
        {
            return obj.GetComponent<ZoneController>();
        }

        private bool isNextZoneNull(int index)
        {
            if (index + 1 >= _zones.Count) { return true; }
            return _zones[index + 1] == null;
        }
        private bool IsNextZoneEmpty(int index) {
            if (index + 1 >= _zones.Count) { return false; }
            return GetZoneControllerByGameObject(_zones[index + 1]).IsOccupied;
        }

        #endregion
    }
}

