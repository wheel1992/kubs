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
        [SerializeField] private GameObject zonePrefab;
        //[SerializeField] private GameObject zoneStartDummyPrefab;
        //private KubsDebug _debugger;
        private List<GameObject> _zones;
        //private Dictionary<int, Stack<ShiftRecord>> _mapShifts;

        private const int INDEX_DEFAULT_CHILD_ZONE = 0;
        private const bool IS_DEBUG = true;

        private GameObject _defaultFirstZone;
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
            //_mapShifts = new Dictionary<int, Stack<ShiftRecord>>();
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
            string msg = "";
            foreach(var i in args.HoveredIndices) {
                msg += i + ", ";
            }
            Debug.Log(msg);

            var largestIndex = args.HoveredIndices.Max(index => index);

            if (args.HoveredIndices.Count == 1)
            {
                // One zone and is empty
                if (IsZoneEmpty(largestIndex))
                {
                    return;
                }

                // Hover only at 1 block
                AddZoneTail();
                return;
            }

            /*
            * Hover across more than 1 block
            * Excute code below
            */

            // largest zone index is empty
            if (IsZoneEmpty(largestIndex))
            {
                return;
            }

            StartCoroutine(Shift(largestIndex));
            AddZoneAt(largestIndex);
            UpdateZoneIndices();

            // PrintZones();
            // var smallestIndex = args.HoveredIndices.Min(index => index);
            // if (IsZoneEmpty(smallestIndex))
            // {
            //     //Debug.Log("HandleZonesHovered: smallest zone " + smallestIndex + " is empty");
            //     // If there's two collided zones
            //     // And the smallest index is not empty
            //     // Suggest to user that the smallest index is available
            //     // ...
            //     return;
            // }

            // var largestIndex = args.HoveredIndices.Max(index => index);
            // if (IsZoneEmpty(largestIndex))
            // {
            //     //Debug.Log("HandleZonesHovered: largest zone = " + largestIndex + " is empty");
            //     return;
            // }
            // StartCoroutine(Shift(largestIndex, largestIndex));
            // UpdateZoneIndices();
            // HasShiftTemporary = true;
        }
        private void HandleZonesUnhovered(object sender, ZoneHoverEventArgs args)
        {
            // Debug.Log("HandleZonesUnhovered: Unhover index " + args.UnhoveredIndex);
            // if (args.UnhoveredIndex != -1)
            // {
            //     StartCoroutine(Unshift(args.UnhoveredIndex));
            //     AddZoneTailIfEmpty();
            //     PrintZones();

            //     if (HasShiftTemporary)
            //         HasShiftTemporary = false;
            // }

            if (IsZoneEmpty(args.UnhoveredIndex))
            {
                /* 
                * Is a blank zone created during hover
                * Destroy the empty zone
                * Shift left for all other zones
                */
                Debug.Log("HandleZonesUnhovered: Unhover index " + args.UnhoveredIndex + " is empty");
                var removedZone = _zones[args.UnhoveredIndex];
                _zones.RemoveAt(args.UnhoveredIndex);
                Destroy(removedZone);
                UpdateZoneIndices();

                StartCoroutine(Unshift(args.UnhoveredIndex));
            }
            else
            {
                /* 
                * When the blocks on zone is shifted away due to hover
                * Remain untouched
                */
                Debug.Log("HandleZonesUnhovered: Unhover index " + args.UnhoveredIndex + " does not need to do anything");
            }


        }
        private void HandleZoneSnapped(object sender, ZoneEventArgs args)
        {
            Debug.Log("HandleZoneSnapped: at " + args.Index + ", is occupied = " + GetZoneControllerByGameObject(_zones[args.Index]).IsOccupied);
            // if (HasShiftTemporary)
            // {
            //     HasShiftTemporary = false;
            //     return;
            // }

            // AddZoneTailIfEmpty();
            // PrintZones();
            // // StartCoroutine(FlushLeft(args.Index));
        }
        private void HandleZoneUnsnapped(object sender, ZoneEventArgs args)
        {
            Debug.Log("HandleZoneUnsnapped: at " + args.Index);
        }
        #endregion

        #region Private Initization Methods
        private void Init()
        {
            // By default, there's one child Zone in ZoneGroup
            _defaultFirstZone = GetChildAt(INDEX_DEFAULT_CHILD_ZONE);
            var _defaultZoneCtrl = GetZoneControllerByGameObject(_defaultFirstZone);
            _defaultFirstZonePosition = _defaultFirstZone.transform.position;
            RegisterZoneEventHandler(_defaultZoneCtrl);

            _zones.Insert(INDEX_DEFAULT_CHILD_ZONE, _defaultFirstZone);
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
            Debug.Log("AddZoneTail");
            AddZoneNext(_zones.Count - 1);
        }
        private void AddZoneTailIfEmpty()
        {
            Debug.Log("AddZoneTailIfEmpty");
            if (!IsTailEmpty())
            {
                AddZoneNext(_zones.Count - 1);
            }
        }
        private void AddZoneAt(int index)
        {
            var pos = new Vector3(
                _defaultFirstZonePosition.x,
                _defaultFirstZonePosition.y,
                _defaultFirstZonePosition.z + (_defaultFirstZone.transform.localScale.z * index));

            var zone = CreateZoneGameObject(pos, index);
            var zoneCtrl = GetZoneControllerByGameObject(zone);
            zoneCtrl.IsOccupied = false;
            zoneCtrl.IsTemporary = false;
            RegisterZoneEventHandler(zoneCtrl);

            if (index == _zones.Count)
            {
                // New index is the list size
                // Use Add instead of Insert
                _zones.Add(zone);
            }
            else
            {
                _zones.Insert(index, zone);
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
            yield break;
            // if (index != 0 && IsPreviousZoneEmpty(index))
            // {
            //     yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[index]).IsOccupied == true);
            //     var block = GetZoneControllerByGameObject(_zones[index]).Detach(true);
            //     // ProgramBlock block = null;
            //     // while (block == null)
            //     // {
            //     //     block = GetZoneControllerByGameObject(_zones[index]).Detach(true);              
            //     // }
            //     yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[index]).IsProcessing == false);
            //     GetZoneControllerByGameObject(_zones[index - 1]).Attach(block);
            //     yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[index - 1]).IsProcessing == false);
            // }
            // else
            // {
            //     RemoveExtraTails();
            //     UpdateZoneIndices();
            //     AddZoneTailIfEmpty();
            // }
            // yield break;
        }
        private void MoveZoneToLeft(int index)
        {
            var zone = _zones[index];
            zone.transform.position = new Vector3(zone.transform.position.x, zone.transform.position.y, zone.transform.position.z - 1);
        }
        private void MoveZoneToRight(int index)
        {
            var zone = _zones[index];
            zone.transform.position = new Vector3(zone.transform.position.x, zone.transform.position.y, zone.transform.position.z + 1);
        }
        private void RemoveExtraTails()
        {
            Debug.Log("RemoveExtraTails");
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
        private IEnumerator Unshift(int index)
        {
            Debug.Log("Unshift: index = " + index);

            for (int i = index; i < _zones.Count; i++)
            {
                MoveZoneToLeft(i);
            }

            yield break;
            // while (!IsStackShiftsEmpty(hoveredIndex))
            // {
            //     var record = GetStackShiftsByHoveredIndex(hoveredIndex).Pop();
            //     // ProgramBlock block = null;
            //     // while (block == null)
            //     // {
            //     //     block = GetZoneControllerByGameObject(_zones[record.To]).Detach(true);              
            //     // }
            //     yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[record.To]).IsOccupied == true);
            //     var block = GetZoneControllerByGameObject(_zones[record.To]).Detach(true);
            //     yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[record.To]).IsProcessing == false);
            //     GetZoneControllerByGameObject(_zones[record.From]).Attach(block);
            //     yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[record.From]).IsProcessing == false);
            //     //yield return null;
            // }
            // yield break;
        }
        private IEnumerator Shift(int index)
        {
            Debug.Log("Shift: index = " + index);

            for (int i = index; i < _zones.Count; i++)
            {
                MoveZoneToRight(i);
            }

            yield break;

            // if (IsZoneEmpty(index))
            // {
            //     // Current index is not occupied
            //     // Does not need to shift
            //     //return;
            //     yield break;
            // }

            // if (!IsNextZoneEmpty(index))
            // {
            //     Shift(hoveredIndex, index + 1);
            // }

            // if (IsTail(index))
            // {
            //     //Debug.Log("Shift: index = " + index + " is the last zone. Not shifting anything");
            //     yield break;
            // }
            // // Execute shifting when...
            // // 1. Current index is not empty
            // // 2. Next index is empty
            // Debug.Log("Shift: From = " + index + ", To = " + (index + 1));
            // /* 
            // * Detach block from current index
            // * Attach block to next index
            // * Add a shift record in stack for keeping track
            // */
            // yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[index]).IsOccupied == true);
            // var block = GetZoneControllerByGameObject(_zones[index]).Detach(true);
            // // yield return null;
            // // ProgramBlock block = null;
            // // while (block == null)
            // // {
            // //     block = GetZoneControllerByGameObject(_zones[index]).Detach(true);
            // //     yield return null;
            // // }
            // yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[index]).IsProcessing == false);
            // GetZoneControllerByGameObject(_zones[index + 1]).Attach(block);
            // yield return new WaitUntil(() => GetZoneControllerByGameObject(_zones[index + 1]).IsProcessing == false);
            // //yield return null;

            // // Add to stack
            // InsertStackShifts(hoveredIndex,
            //     new ShiftRecord
            //     {
            //         From = index,
            //         To = index + 1
            //     });
            // yield break;
        }
        private void DestroyZone(int index)
        {
            if (index < 0 || index >= _zones.Count) { return; }
            var zone = _zones[index];
            _zones.RemoveAt(index);
            Destroy(zone);
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
        // private void InsertStackShifts(int index, ShiftRecord record)
        // {
        //     var stackShifts = GetStackShiftsByHoveredIndex(index);
        //     stackShifts.Push(record);
        //     _mapShifts[index] = stackShifts;
        // }
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
        // private Stack<ShiftRecord> GetStackShiftsByHoveredIndex(int index)
        // {
        //     Stack<ShiftRecord> stackShifts;
        //     if (_mapShifts.TryGetValue(index, out stackShifts))
        //     {
        //         return stackShifts;
        //     }
        //     return new Stack<ShiftRecord>();
        // }
        // private bool IsStackShiftsEmpty(int index)
        // {
        //     var stackShifts = GetStackShiftsByHoveredIndex(index);
        //     return stackShifts.Count == 0;
        // }
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

