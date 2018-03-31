using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        //private KubsDebug _debugger;
        private List<GameObject> _zones;

        private const int INDEX_DEFAULT_CHILD_ZONE = 0;
        private const bool IS_DEBUG = true;

        #region Private Lifecycle Methods
        void Awake()
        {
            //_debugger = new KubsDebug(IS_DEBUG);
        }

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

        #endregion

        #region Public Methods


        #endregion

        #region Private Event Handler Listener 
        private void HandleZonesHovered(object sender, ZoneHoverEventArgs args)
        {
            var largestIndex = args.HoveredIndices.Max(index => index);
            if (args.HoveredIndices.Count == 1 && 
                !GetZoneControllerByGameObject(_zones[largestIndex]).IsOccupied)
            {
                return;
            }
            AddZoneAt(largestIndex);
        }
        private void HandleZonesUnhovered(object sender, ZoneHoverEventArgs args)
        {
            if (args.HoveredIndices.Count > 0) {
                var largestIndex = args.HoveredIndices.Max(index => index);
            }
            
            UpdateZoneIndices();
        }
        private void HandleZoneSnapped(object sender, ZoneEventArgs args)
        {
            if (isNextZoneNull(args.Index))
            {
                Debug.Log("HandleZoneSnapped: Add next zone from " + args.Index);
                AddZone(args.Index);
            }
        }
        private void HandleZoneUnsnapped(object sender, ZoneEventArgs args)
        {

        }
        #endregion

        #region Private Initization Methods
        private void Init()
        {
            // By default, there's one child Zone in ZoneGroup
            var _defaultZoneObject = GetChildAt(INDEX_DEFAULT_CHILD_ZONE);
            var _defaultZoneCtrl = GetZoneControllerByGameObject(_defaultZoneObject);
            RegisterZoneEventHandler(_defaultZoneCtrl);

            _zones.Insert(INDEX_DEFAULT_CHILD_ZONE, _defaultZoneObject);
        }

        private void RegisterZoneEventHandler(ZoneController zone)
        {
            //Debug.Log("RegisterZoneEventHandler: zone index " + zone.Index);

            zone.OnZonesHovered += new ZoneController.ZoneHoverEventHandler(HandleZonesHovered);
            zone.OnZonesUnhovered += new ZoneController.ZoneHoverEventHandler(HandleZonesUnhovered);
            zone.OnZoneSnapped += new ZoneController.ZoneEventHandler(HandleZoneSnapped);
            zone.OnZoneUnsnapped += new ZoneController.ZoneEventHandler(HandleZoneUnsnapped);
        }

        #endregion


        private void AddZone(int currentIndex)
        {
            var nextIndex = currentIndex + 1;
            var currentZone = _zones[currentIndex];
            var currentZonePosition = currentZone.transform.position;
            var nextPosition = new Vector3(
                currentZonePosition.x,
                currentZonePosition.y,
                currentZonePosition.z + currentZone.transform.localScale.z);

            var nextZone = CreateZoneGameObject(nextPosition, nextIndex);
            var nextZoneCtrl = GetZoneControllerByGameObject(nextZone);
            //Debug.Log("AddNextZone nextZoneCtrl index = " + nextZoneCtrl.Index);
            RegisterZoneEventHandler(nextZoneCtrl);

            _zones.Insert(nextIndex, nextZone);
        }
        private void AddZoneAt(int index)
        {
            Debug.Log("AddZoneAt: index " + index);
            if (index < 0 || index >= _zones.Count)
            {
                throw new IndexOutOfRangeException("AddZoneAt: index " + index + " is out of range");
            }

            var currentIndexPosition = _zones[index].transform.position;
            var prevPosition = currentIndexPosition;
            // Move position from index to last zones to right
            for (int i = index; i < _zones.Count; i++)
            {
                var newPos = new Vector3(
                    prevPosition.x,
                    prevPosition.y,
                    prevPosition.z + _zones[i].transform.localScale.z);
                _zones[i].transform.position = newPos;
                Debug.Log("Move zone " + i + " from " + prevPosition+ " to " + newPos);
                prevPosition = _zones[i].transform.position;
            }

            Debug.Log("AddZoneAt: insert temp zone at index " + index);

            var tempZone = CreateZoneGameObject(currentIndexPosition, index);
            var tempZoneCtrl = GetZoneControllerByGameObject(tempZone);
            tempZoneCtrl.IsTemporary = true;
            //Debug.Log("AddNextZone nextZoneCtrl index = " + nextZoneCtrl.Index);
            RegisterZoneEventHandler(tempZoneCtrl);
            _zones.Insert(index, tempZone);

            UpdateZoneIndices();
        }
        private void UpdateZoneIndices()
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var zone = GetZoneControllerByGameObject(_zones[i]);
                zone.Index = i;
            }
        }
        private GameObject CreateZoneGameObject(Vector3 availablePosition, int availableIndex)
        {
            //Debug.Log("CreateZoneGameObject: new zone index " + availableIndex);

            var zone = (GameObject)Instantiate(
             zonePrefab,
             availablePosition,
             Quaternion.identity);
            zone.GetComponent<ZoneController>().Index = availableIndex;

            // Put new zone under this gameobject (parent)
            zone.transform.SetParent(gameObject.transform);

            return zone;
        }
        private void RemoveZoneAt(int index)
        {
            if (index < 0 || index >= _zones.Count)
            {
                throw new IndexOutOfRangeException("RemoveZone: index " + index + " is out of range");
            }

            var zoneCtrl = GetZoneControllerByGameObject(_zones[index]);
            if (zoneCtrl.IsTemporary && !zoneCtrl.IsOccupied)
            {

            }
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
        private bool IsNextZoneEmpty(int index)
        {
            if (index + 1 >= _zones.Count) { return false; }
            return GetZoneControllerByGameObject(_zones[index + 1]).IsOccupied;
        }

        #endregion
    }
}

