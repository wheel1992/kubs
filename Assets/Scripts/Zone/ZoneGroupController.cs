using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kubs
{
    public struct ZoneGroupEventArgs
    {
        public int Index;
    }
    public class ShiftRecord
    {
        public int From { get; set; }
        public int To { get; set; }
    }
    public class ZoneGroupController : MonoBehaviour
    {
        //public delegate void ZoneGroupEventHandler(object sender, ZoneGroupEventArgs args);

        [SerializeField] private GameObject zonePrefab;

        //private KubsDebug _debugger;
        private List<GameObject> _zones;
        private Dictionary<int, Stack<ShiftRecord>> _mapShifts;
        //private Stack<ShiftRecord> _stackShifts;

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
            _mapShifts = new Dictionary<int, Stack<ShiftRecord>>();
            //_stackShifts = new Stack<ShiftRecord>();
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
            var smallestIndex = args.HoveredIndices.Min(index => index);
            if (IsZoneEmpty(smallestIndex))
            {
                // If there's two collided zones
                // And the smallest index is not empty
                // Suggest to user that the smallest index is available
                // ...
                return;
            }

            var largestIndex = args.HoveredIndices.Max(index => index);
            if ((args.HoveredIndices.Count == 1 && !IsZoneEmpty(largestIndex)) ||
                (args.HoveredIndices.Count > 1 && IsNextZoneEmpty(largestIndex)))
            {
                return;
            }
            Shift(largestIndex, largestIndex);
            UpdateZoneIndices();
        }
        private void HandleZonesUnhovered(object sender, ZoneHoverEventArgs args)
        {
            Debug.Log("HandleZonesUnhovered: Unhover index " + args.UnhoveredIndex);
            if (args.UnhoveredIndex != -1)
            {
                RemoveZoneAt(args.UnhoveredIndex);
                UpdateZoneIndices();
            }
        }
        private void HandleZoneSnapped(object sender, ZoneEventArgs args)
        {
            // if (IsNextZoneNull(args.Index))
            // {
            //     Debug.Log("HandleZoneSnapped: Add next zone from " + args.Index);
            //     AddZoneNext(args.Index);
            // }
            if (!IsTailEmpty())
            {
                AddZoneTail();
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

        private void AddZoneTail()
        {
            AddZoneNext(_zones.Count - 1);
        }
        private void AddZoneNext(int currentIndex)
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

            if (nextIndex == _zones.Count)
            {
                // New index is the list size
                // Use Add instead of Insert
                _zones.Add(nextZone);
            }
            else
            {
                _zones.Insert(nextIndex, nextZone);
            }

        }
        private void AddZoneAt(int index)
        {
            // var currentIndexPosition = _zones[index].transform.position;
            // var currentIndexLocalPosition = _zones[index].transform.localPosition;
            // var prevLocalPosition = currentIndexLocalPosition;
            // // Move position from index to last zones to right
            // for (int i = index; i < _zones.Count; i++)
            // {
            //     var newPos = new Vector3(
            //         prevLocalPosition.x,
            //         prevLocalPosition.y,
            //         prevLocalPosition.z + _zones[i].transform.localScale.z);
            //     _zones[i].transform.localPosition = newPos;

            //     GetZoneControllerByGameObject(_zones[i]).SetAttachedProgramBlockPosition(
            //         new Vector3(_zones[i].transform.position.x, 0.9f, _zones[i].transform.position.z));

            //     //Debug.Log("Move zone " + i + " from " + prevLocalPosition+ " to " + newPos);
            //     prevLocalPosition = newPos;
            // }

            // Debug.Log("AddZoneAt: insert temp zone at index " + index);

            // var tempZone = CreateZoneGameObject(currentIndexPosition, index);
            // var tempZoneCtrl = GetZoneControllerByGameObject(tempZone);
            // tempZoneCtrl.IsTemporary = true;
            // tempZoneCtrl.IsOccupied = false;

            // RegisterZoneEventHandler(tempZoneCtrl);
            // _zones.Insert(index, tempZone);
        }
        private void Unshift(int hoveredIndex)
        {
            while (!IsStackShiftsEmpty(hoveredIndex))
            {
                var record = GetStackShiftsByHoveredIndex(hoveredIndex).Pop();
                var block = GetZoneControllerByGameObject(_zones[record.To]).Detach();
                GetZoneControllerByGameObject(_zones[record.From]).Attach(block);
            }
        }
        private void Shift(int hoveredIndex, int index)
        {
            if (IsZoneEmpty(index))
            {
                // Current index is not occupied
                // Does not need to shift
                return;
            }

            if (!IsNextZoneEmpty(index))
            {
                Shift(hoveredIndex, index + 1);
            }

            // Execute shifting when...
            // 1. Current index is not empty
            // 2. Next index is empty

            /* 
            * Detach block from current index
            * Attach block to next index
            * Add a shift record in stack for keeping track
            */
            var block = GetZoneControllerByGameObject(_zones[index]).Detach();
            GetZoneControllerByGameObject(_zones[index + 1]).Attach(block);

            // Add to stack
            InsertStackShifts(hoveredIndex,
                new ShiftRecord
                {
                    From = index,
                    To = index + 1
                });
        }
        private void RemoveZoneAt(int index)
        {
            if (index < 0 || index >= _zones.Count)
            {
                throw new IndexOutOfRangeException("RemoveZone: index " + index + " is out of range");
            }

            var zoneCtrl = GetZoneControllerByGameObject(_zones[index]);
            // Debug.Log("RemoveZoneAt: index " + index + " IsTemporary = " + zoneCtrl.IsTemporary + " IsOccupied = " + zoneCtrl.IsOccupied);
            if (zoneCtrl.IsTemporary && !zoneCtrl.IsOccupied)
            {
                var prevLocalPosition = _zones[index].transform.localPosition;
                // Revert position from index to last zones to left
                for (int i = index + 1; i < _zones.Count; i++)
                {
                    var currentIndexLocalPosition = _zones[index].transform.localPosition;

                    var newPos = new Vector3(
                        prevLocalPosition.x,
                        prevLocalPosition.y,
                        prevLocalPosition.z);
                    _zones[i].transform.localPosition = newPos;

                    GetZoneControllerByGameObject(_zones[i]).SetAttachedProgramBlockPosition(
                        new Vector3(_zones[i].transform.position.x, 0.9f, _zones[i].transform.position.z));

                    // Debug.Log("Revert zone " + i + " from " + currentIndexLocalPosition + " to " + prevLocalPosition);

                    prevLocalPosition = newPos;
                }

                Debug.Log("RemoveZoneAt: remove temp zone at index " + index);

                var tempZoneCtrl = GetZoneControllerByGameObject(_zones[index]);
                Destroy(tempZoneCtrl);
                _zones.RemoveAt(index);

                // Left only 1 zone
                // Add one empty zone to right
                if (_zones.Count == 1)
                {
                    AddZoneNext(0);
                }

            }
        }
        private void UpdateZoneIndices()
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var zone = GetZoneControllerByGameObject(_zones[i]);
                zone.Index = i;
            }
        }

        #region Private Get Methods
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
        private GameObject GetChildAt(int index)
        {
            if (transform.childCount == 0) { return null; }
            return transform.GetChild(index).gameObject;
        }
        private ZoneController GetZoneControllerByGameObject(GameObject obj)
        {
            return obj.GetComponent<ZoneController>();
        }
        private bool IsTailEmpty()
        {
            if (_zones.Count == 0) { return false; }
            return IsZoneEmpty(_zones.Count - 1);
        }
        private void InsertStackShifts(int index, ShiftRecord record)
        {
            var stackShifts = GetStackShiftsByHoveredIndex(index);
            stackShifts.Push(record);
            _mapShifts[index] = stackShifts;
        }
        private Stack<ShiftRecord> GetStackShiftsByHoveredIndex(int index)
        {
            //var stackShifts = _mapShifts.get
            Stack<ShiftRecord> stackShifts;
            if (_mapShifts.TryGetValue(index, out stackShifts))
            {
                return stackShifts;
            }
            return new Stack<ShiftRecord>();
        }
        private bool IsStackShiftsEmpty(int index)
        {
            var stackShifts = GetStackShiftsByHoveredIndex(index);
            return stackShifts.Count == 0;
        }
        private bool IsNextZoneNull(int index)
        {
            return IsZoneNull(index + 1);
        }
        private bool IsNextZoneEmpty(int index)
        {
            return IsZoneEmpty(index + 1);
        }
        private bool IsZoneNull(int index)
        {
            if (index < 0 || index >= _zones.Count) { return false; }
            return _zones[index] == null;
        }
        private bool IsZoneEmpty(int index)
        {
            if (index < 0 || index >= _zones.Count) { return false; }
            return GetZoneControllerByGameObject(_zones[index]).IsOccupied;
        }

        #endregion
    }
}

