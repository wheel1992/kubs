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
    public class ZoneGroupController : MonoBehaviour
    {
        [SerializeField] private GameObject zonePrefab;
        [SerializeField] private GameObject forLoopEndBlockPrefab;
        [SerializeField] private AudioClip audioClipIncorrectPosition;
        [SerializeField] private AudioClip audioClipInsertBlock;

        //private KubsDebug _debugger;
        private List<GameObject> _zones;
        private const int DEFAULT_INDEX_ZONE = 0;
        private const float DEFAULT_ZONE_SIZE = 1.0f;
        private const bool IS_DEBUG = true;
        private AudioSource _audioSourceIncorrectPosition;
        private AudioSource _audioSourceInsertBlock;
        private GameObject _defaultFirstZone;
        private ProgramBlock _defaultForEndLoopBlock;
        private Vector3 _defaultFirstZonePosition;

        #region Private Lifecycle Methods
        void Awake()
        {
            //_debugger = new KubsDebug(IS_DEBUG);
        }

        // Use this for initialization
        void Start()
        {
            _zones = new List<GameObject>();

            InitAudioClips();
            InitDefaultFirstZone();

            // Create a ForEndLoop block and set hidden
            _defaultForEndLoopBlock = CreateForEndBlock(new Vector3(0, 0, 0));
            // _defaultForEndLoopBlock.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            _defaultForEndLoopBlock.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        }

        #endregion

        #region Public Methods

        public List<ProgramBlock> CompileProgramBlocks()
        {
            string msg = "";
            var list = new List<ProgramBlock>();
            foreach (var zone in _zones)
            {
                var block = GetZoneControllerByGameObject(zone).GetAttachedProgramBlock();
                if (block == null)
                {
                    msg += "[], ";
                    continue;
                }
                list.Add(block);
                msg += "[" + GetZoneControllerByGameObject(zone).GetAttachedProgramBlock().Type + "] ";
            }
            Debug.Log("CompileProgramBlocks: " + msg);
            return list;
        }

        #endregion

        #region Private Event Handler Listener 
        private void HandleZonesHovered(object sender, ZoneHoverEventArgs args)
        {
            Debug.Log("Hover: at " + args.ZoneIndex + "");
            var currentZone = GetZoneControllerByGameObject(_zones[args.ZoneIndex]);

            // Test For loop
            var testBlock = GetProgramBlockByGameObject(args.CollidedObject);
            if (testBlock != null)
            {
                if (testBlock.Type == ProgramBlockType.ForLoopStart)
                {
                    var currentForEndIndex = GetZoneIndexWithForLoopEndBlock();
                    Debug.Log("Hover: ForStart " + args.ZoneIndex + " ForEnd " + currentForEndIndex);
                    // Proposed ForStartLoop zone index is args.ZoneIndex
                    if (currentForEndIndex != -1 && args.ZoneIndex > currentForEndIndex)
                    {
                        Debug.Log("Hover: ForStartLoop cannot be behind ForEndLoop");
                        currentZone.DisableSnap();
                        currentZone.ShowHint(false);

                        // Start incorrect position audio
                        _audioSourceIncorrectPosition.Play();

                        return;
                    }
                }
                else if (testBlock.Type == ProgramBlockType.ForLoopEnd)
                {
                    var currentForStartIndex = GetZoneIndexWithForLoopStartBlock();
                    Debug.Log("Hover: ForStart " + currentForStartIndex + " ForEnd " + args.ZoneIndex);
                    // Proposed ForEndLoop zone index is args.ZoneIndex
                    if (currentForStartIndex != -1 && args.ZoneIndex < currentForStartIndex)
                    {
                        Debug.Log("Hover: ForStartLoop cannot be behind ForEndLoop");
                        currentZone.DisableSnap();
                        currentZone.ShowHint(false);

                        // Start incorrect position audio
                        _audioSourceIncorrectPosition.Play();

                        return;
                    }
                }
            }

            currentZone.ShowHint(true);

            // Current zone is empty, do nothing
            if (IsZoneEmpty(args.ZoneIndex))
            {
                Debug.Log("Hover: " + args.ZoneIndex + " is empty");
                return;
            }

            /* 
            * Current zone is occupied
            * Execute code below
            */

            // Left or right non-tail zone is empty, do nothing
            if (IsPreviousZoneEmpty(args.ZoneIndex) || (IsNextZoneEmpty(args.ZoneIndex) && !IsZoneTail(args.ZoneIndex + 1)))
            {
                Debug.Log("Hover: " + args.ZoneIndex + " is either left empty or right non-tail empty");
                return;
            }

            Shift(args.ZoneIndex);
            AddZoneAt(args.ZoneIndex);

            UpdateZoneIndices();
        }
        private void HandleZonesUnhovered(object sender, ZoneHoverEventArgs args)
        {
            Debug.Log("Unhovered: at " + args.ZoneIndex + "");

            // Test For loop
            var testBlock = GetProgramBlockByGameObject(args.CollidedObject);
            if (testBlock != null && (testBlock.Type == ProgramBlockType.ForLoopStart || testBlock.Type == ProgramBlockType.ForLoopEnd))
            {
                GetZoneControllerByGameObject(_zones[args.ZoneIndex]).EnableSnap();
            }

            GetZoneControllerByGameObject(_zones[args.ZoneIndex]).HideHint();

            // Current unhovered zone is occupied, do nothing  
            if (!IsZoneEmpty(args.ZoneIndex))
            {
                return;
            }

            // Current unhovered zone is empty and tail, do nothing
            if (IsZoneTail(args.ZoneIndex))
            {
                return;
            }

            DestroyZone(args.ZoneIndex);
            Unshift(args.ZoneIndex);

            UpdateZoneIndices();
        }
        private void HandleZoneSnapped(object sender, ZoneEventArgs args)
        {
            Debug.Log("HandleZoneSnapped: at " + args.Index + ", is occupied = " + GetZoneControllerByGameObject(_zones[args.Index]).IsOccupied);

            GetZoneControllerByGameObject(_zones[args.Index]).HideHint();

            _audioSourceInsertBlock.Play();

            var attachedBlock = GetZoneControllerByGameObject(_zones[args.Index]).GetAttachedProgramBlock();
            if (attachedBlock != null && attachedBlock.Type == ProgramBlockType.ForLoopStart)
            {
                if (GetZoneIndexWithForLoopEndBlock() == -1)
                {
                    // A ForStartLoop block is attached
                    // Display and attached ForEndLoop at the end of zones
                    _defaultForEndLoopBlock.gameObject.SetActive(true);
                    AddZoneTail();
                    GetZoneTail().AttachBlock(_defaultForEndLoopBlock);
                    return;
                }
            }

            if (IsZoneTail(args.Index))
            {
                AddZoneTail();
                UpdateZoneIndices();
            }

        }
        private void HandleZoneUnsnapped(object sender, ZoneEventArgs args)
        {
            Debug.Log("HandleZoneUnsnapped: at " + args.Index);
        }

        #endregion

        #region Private Initization Methods
        private void InitAudioClips()
        {
            _audioSourceIncorrectPosition = gameObject.AddComponent<AudioSource>();
            _audioSourceIncorrectPosition.clip = audioClipIncorrectPosition;
            _audioSourceIncorrectPosition.loop = false;
            _audioSourceIncorrectPosition.playOnAwake = false;
            _audioSourceIncorrectPosition.volume = 1.0f;

            _audioSourceInsertBlock = gameObject.AddComponent<AudioSource>();
            _audioSourceInsertBlock.clip = audioClipInsertBlock;
            _audioSourceInsertBlock.loop = false;
            _audioSourceInsertBlock.playOnAwake = false;
            _audioSourceInsertBlock.volume = 1.0f;
        }
        private void InitDefaultFirstZone()
        {
            // By default, there's one child Zone in ZoneGroup
            _defaultFirstZone = GetChildAt(DEFAULT_INDEX_ZONE);
            var _defaultZoneCtrl = GetZoneControllerByGameObject(_defaultFirstZone);
            _defaultFirstZonePosition = _defaultFirstZone.transform.position;
            RegisterZoneEventHandler(_defaultZoneCtrl);

            _zones.Insert(DEFAULT_INDEX_ZONE, _defaultFirstZone);
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
            if (!IsZoneTailEmpty())
            {
                AddZoneNext(_zones.Count - 1);
            }
        }
        private void AddZoneAt(int index)
        {
            Debug.Log("AddZoneAt: at " + index);
            var pos = new Vector3(
                _defaultFirstZonePosition.x,
                _defaultFirstZonePosition.y,
                _defaultFirstZonePosition.z + (DEFAULT_ZONE_SIZE * index));

            var zone = CreateZoneGameObject(pos, index);
            var zoneCtrl = GetZoneControllerByGameObject(zone);
            zoneCtrl.IsOccupied = false;
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
        private void MoveZoneToLeft(int index)
        {
            var zone = _zones[index];
            zone.transform.position = new Vector3(zone.transform.position.x, zone.transform.position.y, zone.transform.position.z - 1);
        }
        private void MoveZoneToRight(int index)
        {
            Debug.Log("Right: at " + index);
            var zone = _zones[index];
            zone.transform.position = new Vector3(zone.transform.position.x, zone.transform.position.y, zone.transform.position.z + 1);
        }
        private void Unshift(int index)
        {
            Debug.Log("Unshift: at = " + index);

            for (int i = index; i < _zones.Count; i++)
            {
                MoveZoneToLeft(i);
            }
        }
        private void Shift(int index)
        {
            for (int i = index; i < _zones.Count; i++)
            {
                Debug.Log("Shift: at = " + i);
                MoveZoneToRight(i);
            }
        }
        private void DestroyZone(int index)
        {
            Debug.Log("DestroyZone: at " + index);
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
        private ProgramBlock CreateForEndBlock(Vector3 pos)
        {
            var blockObj = (GameObject)Instantiate(
                forLoopEndBlockPrefab,
                pos,
                Quaternion.identity);
            var block = blockObj.GetComponent<ProgramBlock>();
            block.Type = ProgramBlockType.ForLoopEnd;
            return block;
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
        private GameObject GetChildAt(int index)
        {
            if (transform.childCount == 0) { return null; }
            return transform.GetChild(index).gameObject;
        }
        private ProgramBlock GetProgramBlockByGameObject(GameObject obj)
        {
            return obj.GetComponent<ProgramBlock>();
        }
        private ZoneController GetZoneControllerByGameObject(GameObject obj)
        {
            return obj.GetComponent<ZoneController>();
        }
        private ZoneController GetZoneTail()
        {
            return GetZoneControllerByGameObject(_zones[_zones.Count - 1]);
        }
        private int GetZoneIndexWithForLoopEndBlock()
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var block = GetZoneControllerByGameObject(_zones[i]).GetAttachedProgramBlock();
                if (block != null && block.Type == ProgramBlockType.ForLoopEnd)
                {
                    return i;
                }
            }
            return -1;
        }
        private int GetZoneIndexWithForLoopStartBlock()
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var block = GetZoneControllerByGameObject(_zones[i]).GetAttachedProgramBlock();
                if (block != null && block.Type == ProgramBlockType.ForLoopStart)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Checks whether both ForStartLoop and ForEndLoop are in correct placement
        /// ForStartLoop must be before ForEndLoop 
        /// </summary>
        /// <returns>Returns True or False</returns>
        private bool IsForStartEndLoopCorrectPlacement()
        {
            var startIndex = GetZoneIndexWithForLoopStartBlock();
            var endIndex = GetZoneIndexWithForLoopEndBlock();
            Debug.Log("CorrectPlacement: ForStart " + startIndex + " ForEnd " + endIndex);
            return startIndex < endIndex;
        }
        private bool IsZoneTailEmpty()
        {
            if (_zones.Count == 0) { return false; }
            return IsZoneEmpty(_zones.Count - 1);
        }
        private bool IsZoneTail(int index)
        {
            //Debug.Log("IsZoneTail: at " + index + " is " + (index == _zones.Count - 1));
            return index == _zones.Count - 1;
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
        private bool IsNextZoneNull(int index)
        {
            return IsZoneNull(index + 1);
        }
        private bool IsNextZoneEmpty(int index)
        {
            //Debug.Log("IsNextZoneEmpty: at " + index + " is " + IsZoneEmpty(index + 1));
            return IsZoneEmpty(index + 1);
        }
        private bool IsPreviousZoneEmpty(int index)
        {
            //Debug.Log("IsPreviousZoneEmpty: at " + index + " is " + IsZoneEmpty(index - 1));
            return IsZoneEmpty(index - 1);
        }
        private bool IsZoneNull(int index)
        {
            if (index < 0 || index >= _zones.Count) { return true; }
            return _zones[index] == null;
        }
        private bool IsZoneEmpty(int index)
        {
            if (index < 0 || index >= _zones.Count) { return false; }
            return !GetZoneControllerByGameObject(_zones[index]).IsOccupied;
        }

        #endregion
    }
}

