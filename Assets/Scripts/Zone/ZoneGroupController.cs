using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public struct ZoneGroupEventArgs
    {
        public int Index;
    }
    public class ZoneGroupController : MonoBehaviour
    {
        [SerializeField] private GameObject zonePrefab;
        //[SerializeField] private GameObject forLoopEndBlockPrefab;
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
        //private ProgramBlock _defaultForEndLoopBlock;
        private Vector3 _defaultFirstZonePosition;

        bool isCoroutineExecuting = false;
        IEnumerator ExecuteAfterTime(float time, Action task)
        {
            if (isCoroutineExecuting)
                yield break;
            isCoroutineExecuting = true;
            yield return new WaitForSeconds(time);
            task();
            isCoroutineExecuting = false;
        }

        #region Private Lifecycle Methods
        void Awake()
        {
            //_debugger = new KubsDebug(IS_DEBUG);
        }
        void OnEnable()
        {
            EventManager.StartListening(Constant.EVENT_NAME_FOR_LOOP_START_UNGRAB, HandleForLoopStartUngrab);
        }
        // Use this for initialization
        void Start()
        {
            _zones = new List<GameObject>();

            InitAudioClips();
            InitDefaultFirstZone();

            // Create a ForEndLoop block and set hidden
            // _defaultForEndLoopBlock = CreateForEndBlock(new Vector3(0, 0, 0));
            // // _defaultForEndLoopBlock.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            // _defaultForEndLoopBlock.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        }
        void OnDisable()
        {
            EventManager.StopListening(Constant.EVENT_NAME_FOR_LOOP_START_UNGRAB, HandleForLoopStartUngrab);
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
                msg += "[" + block.Type + "] ";

                if (block.Type == ProgramBlockType.ForLoopStart)
                {
                    block.Value = GetForLoopStart(block).loopCounter;
                }
            }
            Debug.Log("CompileProgramBlocks: " + msg);
            return list;
        }

        #endregion

        #region Private Event Handler Listener 
        private void HandleForLoopStartUngrab(object sender)
        {
            //Debug.Log("HandleForLoopStartUngrab");
            if (sender is VRTK_InteractableObject)
            {
                var interactableObject = (VRTK_InteractableObject)sender;
                //Debug.Log("HandleForLoopStartUngrab: in zone? = " + ();

                StartCoroutine(ExecuteAfterTime(3f, () =>
                {
                    if (!interactableObject.IsInSnapDropZone())
                    {
                        var forStartBlock = interactableObject.gameObject.GetComponent<ForLoopStart>();
                        if (forStartBlock != null)
                        {
                            if (forStartBlock.ForLoopEnd == null)
                            {
                                forStartBlock.ShowDummyForLoopEnd();
                            }
                            else
                            {
                                var forEndIndex = forStartBlock.ForLoopEnd.GetZoneIndex();
                                if (forEndIndex != -1)
                                {
                                    //forStartBlock.ForLoopEnd.SetParent(forStartBlock.transform);
                                    forStartBlock.ShowDummyForLoopEnd();
                                    forStartBlock.DeleteForLoopEnd();
                                    DestroyZone(forEndIndex);
                                    Unshift(forEndIndex);
                                    UpdateZoneIndices();
                                }
                            }

                        }
                    }
                }));


                // // Ungrabbed and dropped not within the Zone (aka outside)
                // if (!interactableObject.IsInSnapDropZone())
                // {
                //     // var block = interactableObject.gameObject.GetComponent<ProgramBlock>();
                //     // if (block == null) { return; }

                //     var forStartBlock = interactableObject.gameObject.GetComponent<ForLoopStart>();
                //     if (forStartBlock == null) { return; }

                //     var forEndIndex = forStartBlock.ForLoopEnd.GetZoneIndex();
                //     if (forEndIndex != -1)
                //     {
                //         // // var block = GetZoneControllerByGameObject(_zones[forEndIndex]).UnparentAttachedBlock();
                //         // //GetZoneControllerByGameObject(_zones[forEndIndex]).UnparentAttachedBlock();
                //         // // forStartBlock.ForLoopEnd.SetInactive();
                //         // // var forEndBlock = block.GetComponent<ForLoopEnd>();
                //         // forStartBlock.ForLoopEnd.SetParent(forStartBlock.transform);
                //         // forStartBlock.DisableForLoopEnd();
                //         // // var forStartBlock = GetForLoopProgramBlock(forEndIndex);
                //         // // forStartBlock.ForLoopEndIndex = -1;
                //         // // forStartBlock.ForLoopStartIndex = -1;

                //         DestroyZone(forEndIndex);
                //         Unshift(forEndIndex);
                //         UpdateZoneIndices();

                //     }
                // }
            }
        }
        private void HandleZonesHovered(object sender, ZoneHoverEventArgs args)
        {
            bool isValidMove = true;

            Debug.Log("Hover: at " + args.CollidedZoneIndex + " with " + args.CollidedObject);
            var currentZone = GetZoneControllerByGameObject(_zones[args.CollidedZoneIndex]);

            // Test For loop
            var testBlock = GetProgramBlockByGameObject(args.CollidedObject);
            if (testBlock != null)
            {
                switch (testBlock.Type)
                {
                    case ProgramBlockType.ForLoopStart:
                        isValidMove = IsForStartCorrectPlacement(testBlock.GetComponent<ForLoopStart>(), args.CollidedZoneIndex);
                        Debug.Log("Hover: ForLoopStart is valid?" + isValidMove);
                        //if (!isValidMove) { return; }
                        //Debug.Log("Hover: ForStart is valid? = " + isValidMove + " child ForEnd index = " + testBlock.GetComponent<ForLoopStart>().ForLoopEnd.GetZoneIndex());
                        break;
                    case ProgramBlockType.ForLoopEnd:
                        isValidMove = IsForEndCorrectPlacement(testBlock.GetComponent<ForLoopEnd>(), args.CollidedZoneIndex);
                        Debug.Log("Hover: ForLoopEnd is valid?" + isValidMove);
                        //if (!isValidMove) { return; }
                        break;
                    default: break;
                }
                // if (testBlock.Type == ProgramBlockType.ForLoopStart)
                // {
                //     var currentForEndIndex = GetZoneIndexWithForLoopEndBlock();
                //     //Debug.Log("Hover: ForStart " + args.ZoneIndex + " ForEnd " + currentForEndIndex);
                //     // Proposed ForStartLoop zone index is args.ZoneIndex
                //     if (currentForEndIndex != -1 && args.CollidedZoneIndex > currentForEndIndex)
                //     {
                //         //Debug.Log("Hover: ForStartLoop cannot be behind ForEndLoop");
                //         currentZone.DisableSnap();
                //         currentZone.ShowHint(false);

                //         // Start incorrect position audio
                //         _audioSourceIncorrectPosition.Play();

                //         return;
                //     }
                // }
                // else if (testBlock.Type == ProgramBlockType.ForLoopEnd)
                // {
                //     var currentForStartIndex = GetZoneIndexWithForLoopStartBlock();
                //     //Debug.Log("Hover: ForStart " + currentForStartIndex + " ForEnd " + args.ZoneIndex);
                //     // Proposed ForEndLoop zone index is args.ZoneIndex
                //     if (currentForStartIndex != -1 && args.CollidedZoneIndex < currentForStartIndex)
                //     {
                //         //Debug.Log("Hover: ForStartLoop cannot be behind ForEndLoop");
                //         currentZone.DisableSnap();
                //         currentZone.ShowHint(false);

                //         // Start incorrect position audio
                //         _audioSourceIncorrectPosition.Play();

                //         return;
                //     }
                // }
            }

            if (!isValidMove)
            {
                currentZone.DisableSnap();
                currentZone.ShowHint(false);

                // Start incorrect position audio
                _audioSourceIncorrectPosition.Play();
                return;
            }

            // Current zone is empty, do nothing
            if (IsZoneEmpty(args.CollidedZoneIndex))
            {
                Debug.Log("Hover: " + args.CollidedZoneIndex + " is empty");
                currentZone.ShowHint(true);
                return;
            }

            /* 
            * Current zone is occupied
            * Execute code below
            */

            // Left or right non-tail zone is empty, do nothing
            if (IsPreviousZoneEmpty(args.CollidedZoneIndex) || (IsNextZoneEmpty(args.CollidedZoneIndex) && !IsZoneTail(args.CollidedZoneIndex + 1)))
            {
                Debug.Log("Hover: " + args.CollidedZoneIndex + " is either left empty or right non-tail empty");
                return;
            }

            Shift(args.CollidedZoneIndex);
            AddZoneAt(args.CollidedZoneIndex);

            Debug.Log("Hover: " + args.CollidedZoneIndex + " showing hint...");
            GetZoneControllerByGameObject(_zones[args.CollidedZoneIndex]).ShowHint(true);

            UpdateZoneIndices();

            // // Do Test Here
            // int forStartIndex = GetNearestLeftForLoopStart(_zones.Count - 1);
            // ForLoopStart forLoopStart = GetZoneControllerByGameObject(_zones[forStartIndex]).GetAttachedProgramBlock().GetComponent<ForLoopStart>();
            // int forEndIndex = forLoopStart.ForLoopEnd.GetZoneIndex();
            // // int forLoopEndIndex = GetNearestRightForLoopEnd(0);
            // // if (forLoopStartIndex != -1 && forLoopEndIndex != -1)
            // // {
            // //     var forLoopBlock = GetForLoopProgramBlock(forLoopStartIndex);
            // //     //Debug.Log("Unhovered: ForStart " + forLoopStartIndex + " ForEnd " + forLoopEndIndex);
            // //     forLoopBlock.SetSideAreaTo(forLoopStartIndex, forLoopEndIndex);
            // // }
            // if (forStartIndex != -1 && forEndIndex != -1)
            // {
            //     forLoopStart.SetSideAreaTo(forStartIndex, forEndIndex);
            // }
            UpdateForLoopSideArea(args.CollidedZoneIndex);
        }
        private void HandleZonesUnhovered(object sender, ZoneHoverEventArgs args)
        {
            //Debug.Log("Unhovered: at " + args.ZoneIndex + "");

            // Test For loop
            // var testBlock = GetProgramBlockByGameObject(args.CollidedObject);
            // if (testBlock != null && (testBlock.Type == ProgramBlockType.ForLoopStart || testBlock.Type == ProgramBlockType.ForLoopEnd))
            // {
            //     GetZoneControllerByGameObject(_zones[args.CollidedZoneIndex]).EnableSnap();
            // }
            GetZoneControllerByGameObject(_zones[args.CollidedZoneIndex]).EnableSnap();

            GetZoneControllerByGameObject(_zones[args.CollidedZoneIndex]).HideHint();

            // Current unhovered zone is occupied, do nothing  
            if (!IsZoneEmpty(args.CollidedZoneIndex))
            {
                return;
            }

            // Current unhovered zone is empty and tail, do nothing
            if (IsZoneTail(args.CollidedZoneIndex))
            {
                return;
            }

            DestroyZone(args.CollidedZoneIndex);
            Unshift(args.CollidedZoneIndex);

            UpdateZoneIndices();


            // // // Do Test Here
            // int forStartIndex = GetNearestLeftForLoopStart(_zones.Count - 1);
            // ForLoopStart forLoopStart = GetZoneControllerByGameObject(_zones[forStartIndex]).GetAttachedProgramBlock().GetComponent<ForLoopStart>();
            // int forEndIndex = forLoopStart.ForLoopEnd.GetZoneIndex();
            // // int forLoopEndIndex = GetNearestRightForLoopEnd(0);
            // // if (forLoopStartIndex != -1 && forLoopEndIndex != -1)
            // // {
            // //     var forLoopBlock = GetForLoopProgramBlock(forLoopStartIndex);
            // //     //Debug.Log("Unhovered: ForStart " + forLoopStartIndex + " ForEnd " + forLoopEndIndex);
            // //     forLoopBlock.SetSideAreaTo(forLoopStartIndex, forLoopEndIndex);
            // // }
            // if (forStartIndex != -1 && forEndIndex != -1)
            // {
            //     forLoopStart.SetSideAreaTo(forStartIndex, forEndIndex);
            // }
            UpdateForLoopSideArea(args.CollidedZoneIndex);
        }
        private void HandleZoneSnapped(object sender, ZoneEventArgs args)
        {
            //Debug.Log("HandleZoneSnapped: at " + args.Index + ", is occupied = " + GetZoneControllerByGameObject(_zones[args.Index]).IsOccupied);
            GetZoneControllerByGameObject(_zones[args.Index]).HideHint();

            _audioSourceInsertBlock.Play();
            Debug.Log("HandleZoneSnapped:");
            var attachedBlock = GetZoneControllerByGameObject(_zones[args.Index]).GetAttachedProgramBlock();
            if (attachedBlock != null)
            {
                // int forLoopStartIndex = -1;
                // int forLoopEndIndex = -1;
                Debug.Log("HandleZoneSnapped: " + attachedBlock.gameObject.name);
                switch (attachedBlock.Type)
                {
                    case ProgramBlockType.ForLoopStart:
                        attachedBlock.transform.localPosition = new Vector3(
                                attachedBlock.transform.localPosition.x,
                                0.8f,
                                attachedBlock.transform.localPosition.z);

                        var forStart = attachedBlock.GetComponent<ForLoopStart>();
                        //Debug.Log("HandleZoneSnapped: ForStart's ForEnd zone index = " + forStart.ForLoopEnd.GetZoneIndex());
                        if (forStart.ForLoopEnd == null || forStart.ForLoopEnd.GetZoneIndex() == -1)
                        {
                            //forStart.ForLoopEnd.SetActive();
                            //AddZoneTail();
                            //GetZoneTail().AttachBlock(forStart.ForLoopEnd.GetProgramBlock());

                            forStart.HideDummyForLoopEnd();

                            Shift(args.Index + 1);
                            AddZoneAt(args.Index + 1);
                            UpdateZoneIndices();
                            // GetZoneAt(args.Index + 1).AttachBlock(forStart.ForLoopEnd.GetProgramBlock());
                            GetZoneAt(args.Index + 1).AttachBlock(forStart.CreateForLoopEnd().GetProgramBlock());
                        }

                        UpdateForLoopSideArea(args.Index);
                        // if (GetZoneIndexWithForLoopEndBlock() == -1)
                        // {
                        //     // A ForStartLoop block is attached
                        //     _defaultForEndLoopBlock.gameObject.SetActive(true);
                        //     // Display and attached ForEndLoop at the end of zones
                        //     AddZoneTail();
                        //     GetZoneTail().AttachBlock(_defaultForEndLoopBlock);

                        //     var forLoopBlock = GetForLoopProgramBlock(attachedBlock);
                        //     forLoopBlock.ForLoopStartIndex = args.Index;
                        //     forLoopBlock.ForLoopEndIndex = GetZoneTail().Index;
                        //     return;

                        // }
                        // else
                        // {
                        //     forLoopEndIndex = GetNearestRightForLoopEnd(args.Index);
                        //     if (forLoopEndIndex != -1)
                        //     {
                        //         var forLoopBlock = GetForLoopProgramBlock(args.Index);
                        //         forLoopBlock.ForLoopEndIndex = forLoopEndIndex;
                        //         //Debug.Log("HandleZoneSnapped: ForStart = " + args.Index + " ForEnd = " + forLoopEndIndex);

                        //         forLoopBlock.SetSideAreaTo(forLoopEndIndex, forLoopEndIndex);
                        //     }
                        // }

                        break;

                    case ProgramBlockType.ForLoopEnd:
                        attachedBlock.transform.localPosition = new Vector3(
                            attachedBlock.transform.localPosition.x,
                            0.55f, // 0.8f,
                            attachedBlock.transform.localPosition.z);

                        UpdateForLoopSideArea(args.Index);
                        // forLoopStartIndex = GetNearestLeftForLoopStart(args.Index);
                        // if (forLoopStartIndex != -1)
                        // {
                        //     var forLoopBlock = GetForLoopProgramBlock(forLoopStartIndex);
                        //     forLoopBlock.ForLoopEndIndex = args.Index;
                        //     //Debug.Log("HandleZoneSnapped: ForStart = " + forLoopStartIndex + " ForEnd = " + args.Index);

                        //     forLoopBlock.SetSideAreaTo(forLoopStartIndex, args.Index);
                        // }
                        break;

                    default:
                        attachedBlock.transform.localPosition = new Vector3(
                            attachedBlock.transform.localPosition.x,
                            0.8f,
                            attachedBlock.transform.localPosition.z);

                        // forLoopStartIndex = GetNearestLeftForLoopStart(args.Index);
                        // forLoopEndIndex = GetNearestRightForLoopEnd(args.Index);
                        // if (forLoopStartIndex != -1 && forLoopEndIndex != -1)
                        // {
                        //     var forLoopBlock = GetForLoopProgramBlock(forLoopStartIndex);
                        //     forLoopBlock.ForLoopEndIndex = forLoopEndIndex;
                        //     //Debug.Log("HandleZoneSnapped: ForStart = " + forLoopStartIndex + " ForEnd = " + forLoopEndIndex);

                        //     forLoopBlock.SetSideAreaTo(forLoopStartIndex, forLoopEndIndex);
                        // }
                        UpdateForLoopSideArea(args.Index);
                        break;
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

            // var attachedBlock = GetZoneControllerByGameObject(_zones[args.Index]).GetAttachedProgramBlock();
            // if (attachedBlock != null)
            // {
            //     int forLoopStartIndex = -1;
            //     int forLoopEndIndex = -1;
            //     //int numBetweenBlock = 0;
            //     switch (attachedBlock.Type)
            //     {
            //         case ProgramBlockType.ForLoopStart:
            //             break;
            //         case ProgramBlockType.ForLoopEnd:
            //             break;
            //         default:

            //             break;
            //     }
            // }
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
            // Debug.Log("position = " + _defaultFirstZone.transform.position + " localPosition = " + _defaultFirstZone.transform.localPosition);
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
            //Debug.Log("AddZoneTail");
            AddZoneNext(_zones.Count - 1);
        }
        private void AddZoneTailIfEmpty()
        {
            //Debug.Log("AddZoneTailIfEmpty");
            if (!IsZoneTailEmpty())
            {
                AddZoneNext(_zones.Count - 1);
            }
        }
        private void AddZoneAt(int index)
        {
            //Debug.Log("AddZoneAt: at " + index);
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
            //Debug.Log("Right: at " + index);
            var zone = _zones[index];
            zone.transform.position = new Vector3(zone.transform.position.x, zone.transform.position.y, zone.transform.position.z + 1);
        }
        private void Unshift(int index)
        {
            //Debug.Log("Unshift: at = " + index);

            for (int i = index; i < _zones.Count; i++)
            {
                MoveZoneToLeft(i);
            }
        }
        private void Shift(int index)
        {
            for (int i = index; i < _zones.Count; i++)
            {
                //Debug.Log("Shift: at = " + i);
                MoveZoneToRight(i);
            }
        }
        private void DestroyZone(int index)
        {
            //Debug.Log("DestroyZone: at " + index);
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
                var block = zone.GetAttachedProgramBlock();

                zone.Index = i;
            }
        }
        private void UpdateForLoopSideArea(int index)
        {
            int forStartIndex = GetNearestLeftForLoopStart(index);
            if (forStartIndex == -1) { return; }
            ForLoopStart forLoopStart = GetZoneControllerByGameObject(_zones[forStartIndex]).GetAttachedProgramBlock().GetComponent<ForLoopStart>();
            int forEndIndex = forLoopStart.ForLoopEnd.GetZoneIndex();

            if (forStartIndex != -1 && forEndIndex != -1)
            {
                forLoopStart.SetSideAreaTo(forStartIndex, forEndIndex);
            }
        }

        #region Private Get Methods
        // private ProgramBlock CreateForEndBlock(Vector3 pos)
        // {
        //     var blockObj = (GameObject)Instantiate(
        //         forLoopEndBlockPrefab,
        //         pos,
        //         Quaternion.identity);
        //     var block = blockObj.GetComponent<ProgramBlock>();
        //     block.Type = ProgramBlockType.ForLoopEnd;
        //     return block;
        // }
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
        public ForLoopStart GetForLoopStart(int zoneIndex)
        {
            var zone = GetZoneControllerByGameObject(_zones[zoneIndex]);
            if (zone == null) return null;

            var block = zone.GetAttachedProgramBlock();
            if (block == null) return null;

            return GetForLoopStart(block);
        }
        public ForLoopStart GetForLoopStart(ProgramBlock block)
        {
            if (block.Type == ProgramBlockType.ForLoopStart)
            {
                return block.gameObject.GetComponent<ForLoopStart>();
            }
            return null;
        }
        private ProgramBlock GetProgramBlockByGameObject(GameObject obj)
        {
            return obj.GetComponent<ProgramBlock>();
        }
        private ZoneController GetZoneControllerByGameObject(GameObject obj)
        {
            return obj.GetComponent<ZoneController>();
        }
        private ZoneController GetZoneAt(int index)
        {
            if (index < 0 || index >= _zones.Count) { return null; }
            return GetZoneControllerByGameObject(_zones[index]);
        }
        private ZoneController GetZoneTail()
        {
            return GetZoneControllerByGameObject(_zones[_zones.Count - 1]);
        }

        // Test ForLoop v2




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
        private bool IsForStartCorrectPlacement(ForLoopStart forStart, int targetZoneIndex)
        {

            if (forStart.ForLoopEnd != null)
            {
                if (forStart.ForLoopEnd.GetZoneIndex() < targetZoneIndex)
                {
                    Debug.Log("IsForStartCorrectPlacement: own forEnd is less than own ForStart");
                    return false;
                }
            }

            var leftForStartIndex = GetNearestLeftForLoopStart(targetZoneIndex);
            //Debug.Log("IsForStartCorrectPlacement: leftForStartIndex = " + leftForStartIndex + ", targetZoneIndex = " + targetZoneIndex);

            // Either there's no left ForStart (exist or not)
            // Or there's right ForStart
            if (leftForStartIndex == -1) { return true; }

            var leftForStart = GetZoneControllerByGameObject(_zones[leftForStartIndex]).GetAttachedProgramBlock().GetComponent<ForLoopStart>();
            // Test for invalid move
            // if (leftForStartIndex < targetZoneIndex && leftForStart.ForLoopEnd.GetZoneIndex() > targetZoneIndex)
            if (leftForStartIndex < targetZoneIndex)
            {
                if (leftForStart.ForLoopEnd == null) { return true; }
                if (leftForStart.ForLoopEnd.GetZoneIndex() > targetZoneIndex) { return false; }
                //Debug.Log("IsForStartCorrectPlacement: false");
                //return false;
            }
            //Debug.Log("IsForStartCorrectPlacement: true");
            return true;
            // var startIndex = GetZoneIndexWithForLoopStartBlock();
            // var endIndex = GetZoneIndexWithForLoopEndBlock();
            // Debug.Log("CorrectPlacement: ForStart " + startIndex + " ForEnd " + endIndex);
            // return startIndex < endIndex;
        }
        private bool IsForEndCorrectPlacement(ForLoopEnd forEnd, int targetZoneIndex)
        {
            // Current forEnd's parent - ForStart is the nearest left
            if (forEnd.ForLoopStart.GetZoneIndex() >= targetZoneIndex)
            {
                Debug.Log("IsForEndCorrectPlacement: forEnd target is less than its ForStart");
                return false;
            }

            var leftForStartIndex = GetNearestLeftForLoopStart(targetZoneIndex);
            // Debug.Log("IsForEndCorrectPlacement: targetZoneIndex = " + targetZoneIndex);
            // Debug.Log("IsForEndCorrectPlacement: leftForStartIndex = " + leftForStartIndex);

            // Either there's no left ForStart (exist or not)
            // Or there's right ForStart
            if (leftForStartIndex == -1) { return true; }

            var leftForStart = GetZoneControllerByGameObject(_zones[leftForStartIndex]).GetAttachedProgramBlock().GetComponent<ForLoopStart>();

            // Debug.Log("IsForEndCorrectPlacement: forEnd.ForLoopStart.GetZoneIndex() = " + forEnd.ForLoopStart.GetZoneIndex());

            // Test for invalid move
            // if (forEnd.ForLoopStart.GetZoneIndex() != leftForStartIndex &&
            //     leftForStartIndex < targetZoneIndex &&
            //     (leftForStart.ForLoopEnd.GetZoneIndex() > targetZoneIndex || leftForStart.ForLoopEnd.GetZoneIndex() < targetZoneIndex))
            if (forEnd.ForLoopStart.GetZoneIndex() != leftForStartIndex &&
                leftForStartIndex < targetZoneIndex
                )
            {
                if (leftForStart.ForLoopEnd == null)
                {
                    return true;
                }
                else if (leftForStart.ForLoopEnd.GetZoneIndex() > targetZoneIndex || leftForStart.ForLoopEnd.GetZoneIndex() < targetZoneIndex)
                {
                    Debug.Log("IsForEndCorrectPlacement: false");
                    return false;
                }

            }
            Debug.Log("IsForEndCorrectPlacement: true");
            return true;
        }
        private int GetNearestLeftForLoopStart(int index)
        {
            for (int i = index; i >= 0; i--)
            {
                var block = GetZoneControllerByGameObject(_zones[i]).GetAttachedProgramBlock();
                // Get first ForLoopStart
                if (block != null && block.Type == ProgramBlockType.ForLoopStart)
                {
                    return i;
                }
            }
            return -1;
        }
        // private int GetNearestRightForLoopEnd(int index)
        // {
        //     for (int i = index; i < _zones.Count; i++)
        //     {
        //         var block = GetZoneControllerByGameObject(_zones[i]).GetAttachedProgramBlock();
        //         // Get first ForLoopEnd
        //         if (block != null && block.Type == ProgramBlockType.ForLoopEnd)
        //         {
        //             return i;
        //         }
        //     }
        //     return -1;
        // }
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

