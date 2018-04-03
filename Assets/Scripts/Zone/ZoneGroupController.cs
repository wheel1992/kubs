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
        public override string ToString()
        {
            return "ShiftRecord: From " + From + " To " + To;
        }
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

        private Vector3 _defaultFirstZonePosition;
        private bool HasShiftTemporary = false;

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
            Debug.Log("HandleZonesHovered");
            PrintZones();
            var smallestIndex = args.HoveredIndices.Min(index => index);
            if (IsZoneEmpty(smallestIndex))
            {
                //Debug.Log("HandleZonesHovered: smallest zone " + smallestIndex + " is empty");
                // If there's two collided zones
                // And the smallest index is not empty
                // Suggest to user that the smallest index is available
                // ...
                return;
            }

            var largestIndex = args.HoveredIndices.Max(index => index);
            if (IsZoneEmpty(largestIndex))
            {
                //Debug.Log("HandleZonesHovered: largest zone = " + largestIndex + " is empty");
                return;
            }
            StartCoroutine(Shift(largestIndex, largestIndex));
            UpdateZoneIndices();
            HasShiftTemporary = true;
        }
        private void HandleZonesUnhovered(object sender, ZoneHoverEventArgs args)
        {
            Debug.Log("HandleZonesUnhovered: Unhover index " + args.UnhoveredIndex);
            if (args.UnhoveredIndex != -1)
            {
                //StartCoroutine(Unshift(args.UnhoveredIndex));
                Reposition();
                AddZoneTailIfEmpty();
                PrintZones();
                //UpdateZoneIndices();

                if (HasShiftTemporary)
                    HasShiftTemporary = false;
            }
        }
        private void HandleZoneSnapped(object sender, ZoneEventArgs args)
        {
            Debug.Log("HandleZoneSnapped");
            if (HasShiftTemporary)
            {
                HasShiftTemporary = false;
                return;
            }

            Reposition();
            AddZoneTailIfEmpty();
            PrintZones();

            //StartCoroutine(FlushLeft(args.Index));
            //UpdateZoneIndices();
            // //Debug.Log("HandleZoneSnapped: IsTailEmpty = " + IsTailEmpty());
            // PrintZones();
            // if (!IsTailEmpty())
            // {
            //     AddZoneTail();
            // }
            // UpdateZoneIndices();
        }
        private void HandleZoneUnsnapped(object sender, ZoneEventArgs args)
        {
            Debug.Log("HandleZoneUnsnapped");
            PrintZones();
            UpdateZoneIndices();
            // if (!HasShiftTemporary)
            // {
            //     if (IsZoneEmpty(args.Index))
            //     {
            //         DestroyZone(args.Index);
            //     }
            // }
            
        }
        #endregion

        #region Private Initization Methods
        private void Init()
        {
            // By default, there's one child Zone in ZoneGroup
            var _defaultZoneObject = GetChildAt(INDEX_DEFAULT_CHILD_ZONE);
            var _defaultZoneCtrl = GetZoneControllerByGameObject(_defaultZoneObject);
            _defaultFirstZonePosition = _defaultZoneObject.transform.position;
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

        private void AddZoneTailIfEmpty()
        {
            Debug.Log("AddZoneTailIfEmpty");
            if(!IsTailEmpty()) {
                AddZoneNext(_zones.Count - 1);
            }   
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
            nextZoneCtrl.IsOccupied = false;
            nextZoneCtrl.IsTemporary = false;
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
        private IEnumerator FlushLeft(int index)
        {
            if (index != 0 && IsPreviousZoneEmpty(index))
            {
                var block = GetZoneControllerByGameObject(_zones[index]).Detach(true);
                //Debug.Log("FlushLeft: Detach block = " + block);
                yield return null;
                GetZoneControllerByGameObject(_zones[index - 1]).Attach(block);
                yield return null;
            }
            else
            {
                RemoveExtraTails();
                UpdateZoneIndices();
                AddZoneTailIfEmpty();
            }
            yield break;
        }
        private void RemoveExtraTails()
        {
            Debug.Log("RemoveExtraTails");
            PrintZones();
            bool isPreviousEmpty = false;
            int prev = 0;
            for (int b = _zones.Count - 1; b >= 0; b--)
            {
                if (isPreviousEmpty)
                {
                    DestroyZone(prev);
                    isPreviousEmpty = false;
                }
                if (IsZoneEmpty(b))
                {
                    isPreviousEmpty = true;
                }
                else
                {
                    break;
                }
                prev = b;
            }
        }
        private IEnumerator Unshift(int hoveredIndex)
        {
            while (!IsStackShiftsEmpty(hoveredIndex))
            {
                var record = GetStackShiftsByHoveredIndex(hoveredIndex).Pop();
                var block = GetZoneControllerByGameObject(_zones[record.To]).Detach(true);
                Debug.Log("Unshift: Detach block = " + block);
                yield return null;
                GetZoneControllerByGameObject(_zones[record.From]).Attach(block);
                yield return null;
            }
            yield break;
        }
        private IEnumerator Shift(int hoveredIndex, int index)
        {
            Debug.Log("Shift: hoveredIndex = " + hoveredIndex + ", index" + index);

            if (IsZoneEmpty(index))
            {
                // Current index is not occupied
                // Does not need to shift
                //return;
                yield break;
            }

            if (!IsNextZoneEmpty(index))
            {
                Shift(hoveredIndex, index + 1);
            }

            if (IsTail(index))
            {
                //Debug.Log("Shift: index = " + index + " is the last zone. Not shifting anything");
                yield break;
            }
            // Execute shifting when...
            // 1. Current index is not empty
            // 2. Next index is empty
            Debug.Log("Shift: From = " + index + ", To = " + (index + 1));
            /* 
            * Detach block from current index
            * Attach block to next index
            * Add a shift record in stack for keeping track
            */
            var block = GetZoneControllerByGameObject(_zones[index]).Detach(true);
            //Debug.Log("Shift: Detach block = " + block);
            yield return null;
            GetZoneControllerByGameObject(_zones[index + 1]).Attach(block);
            yield return null;
            // Add to stack
            InsertStackShifts(hoveredIndex,
                new ShiftRecord
                {
                    From = index,
                    To = index + 1
                });
            yield break;
        }
        private void DestroyZone(int index)
        {
            if (index < 0 || index >= _zones.Count) { return; }
            var zone = _zones[index];
            _zones.RemoveAt(index);
            Destroy(zone);
        }
        private void Reposition()
        {
            // _zones.RemoveAll(zone => !GetZoneControllerByGameObject(zone).IsOccupied);
            foreach (var zone in _zones.ToList())
            {
                if (!GetZoneControllerByGameObject(zone).IsOccupied)
                {
                    _zones.Remove(zone);
                    Destroy(zone);
                }
            }

            UpdateZoneIndices();

            if (_zones.Count > 0)
            {
                var i = 0;
                foreach (var zone in _zones)
                {
                    // Set new position
                    zone.transform.position = new Vector3(
                        _defaultFirstZonePosition.x,
                        _defaultFirstZonePosition.y,
                        _defaultFirstZonePosition.z + (zone.transform.localScale.z * i));
                    // Update zone index
                    GetZoneControllerByGameObject(zone).Index = i;
                    // Increment counter
                    i++;
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
        private bool IsTail(int index)
        {
            return index == _zones.Count - 1;
        }
        private void InsertStackShifts(int index, ShiftRecord record)
        {
            var stackShifts = GetStackShiftsByHoveredIndex(index);
            stackShifts.Push(record);
            _mapShifts[index] = stackShifts;
        }
        private void PrintZones()
        {
            string msg = "";
            foreach (var zone in _zones)
            {
                if (GetZoneControllerByGameObject(zone).IsOccupied)
                {
                    msg += " [x]";
                }
                else
                {
                    msg += " [_]";
                }
            }
            Debug.Log(msg);
        }
        private Stack<ShiftRecord> GetStackShiftsByHoveredIndex(int index)
        {
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
        private bool IsPreviousZoneEmpty(int index)
        {
            return IsZoneEmpty(index - 1);
        }
        private bool IsZoneNull(int index)
        {
            if (index < 0 || index >= _zones.Count) { return true; }
            return _zones[index] == null;
        }
        private bool IsZoneEmpty(int index)
        {
            if (index < 0 || index >= _zones.Count) { return true; }
            return !GetZoneControllerByGameObject(_zones[index]).IsOccupied;
        }

        #endregion
    }
}

