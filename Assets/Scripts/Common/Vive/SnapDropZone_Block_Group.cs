using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class SnapDropZone_Block_Group : MonoBehaviour
    {
        [SerializeField] private int maximumNumberOfSnapDropZones = 10;
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private GameObject snapDropZonePrefab;
        [SerializeField] private GameObject topColliderObjectPrefab;
        [SerializeField] private GameObject _proposedBlockPrefab;
        /// <summary>
        /// Holds a list of top collider object prefabs
        /// </summary>


        /// <summary>
        /// This holds the default first snap drop zone gameobject
        /// </summary>
        private GameObject _defaultSnapDropZonePrefab;
        private Vector3 _defaultSnapDropZonePosition;

        private IList<Block> _blocks;
        private IList<GameObject> _zones;
        private IList<GameObject> _proposedBlocks;

        private Stack<StackItemMoveZone> _stackMoveZones;
        //private IList<GameObject> _topColliderObjects;

        //private static int SnappedBlockIndex = 0;
        private static int _numOfSnapDropZone = 1;
        private static int _snappedBlockCount = 0;

        private void Start()
        {
            _blocks = new List<Block>();
            _zones = new List<GameObject>();
            _proposedBlocks = new List<GameObject>();
            _stackMoveZones = new Stack<StackItemMoveZone>();

            _defaultSnapDropZonePrefab = transform.Find(Constant.NAME_SNAP_DROP_ZONE_PROGRAM_BLOCK).gameObject;
            _defaultSnapDropZonePrefab.GetComponent<SnapDropZone>().ZoneId = 0;
            _defaultSnapDropZonePosition = _defaultSnapDropZonePrefab.transform.position;

            RegisterSnapDropZoneEventHandler(_defaultSnapDropZonePrefab);

            RegisterLevelSceneLoadEventHandler(levelPrefab.GetComponent<SceneLoad>());

            _zones.Add(_defaultSnapDropZonePrefab);
            for (int i = 1; i <= maximumNumberOfSnapDropZones; i++)
            {
                _zones.Add(AddSnapDropZone());
                _blocks.Add(null);
            } 
        }

        private void DoProgramBlockZoneEntered(object sender, SnapDropZoneEventArgs e)
        {
            Debug.Log("== SnapDropZone: ENTERING <<<<");
        }

        private void DoProgramBlockZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                ProgramBlock block = e.snappedObject.GetComponent<ProgramBlock>();

                int snappedZoneId = GetProgramBlockSnappedDropZone(block).ZoneId;
                Debug.Log("DoProgramBlockZoneSnapped to zone " + GetProgramBlockSnappedDropZone(block).ZoneId);

                block.ZoneId = snappedZoneId;
                block.StartSweepChildTrigger();

                // Zone id start index at 0
                // Can be used to replace at specific list index
                _blocks[snappedZoneId] = block;

                
            } else
            {
                Debug.Log("DoProgramBlockZoneSnapped snappedObject is null!!!");
            } 
        }

        private void DoProgramBlockZoneExited(object sender, SnapDropZoneEventArgs e)
        {
            Debug.Log("== SnapDropZone: EXITED >>>>");
            //if (e.snappedObject != null)
            //{
            //    Debug.Log("DoProgramBlockZoneExited object tag = " + e.snappedObject.tag);

            //    if (e.snappedObject.tag.CompareTo(Constant.TAG_BLOCK_PROGRAM) == 0)
            //    {
            //        ProgramBlock block = e.snappedObject.GetComponent<ProgramBlock>();
            //        int snappedZoneId = GetProgramBlockSnappedDropZone(block).ZoneId;
            //        //int snappedZoneId = _blocks.IndexOf(block);
            //        Debug.Log("DoProgramBlockZoneExited from zone " + block.ZoneId);

            //        block.ZoneId = -1;
            //        block.PauseSweepChildTrigger();

            //        _blocks[snappedZoneId] = block;
            //    }
            //}
            
        }

        private void DoProgramBlockZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                if (e.snappedObject.tag.CompareTo(Constant.TAG_BLOCK_PROGRAM) == 0)
                {
                    ProgramBlock block = e.snappedObject.GetComponent<ProgramBlock>();
                    int snappedZoneId = GetProgramBlockSnappedDropZone(block).ZoneId;
                    Debug.Log("DoProgramBlockZoneUnsnapped to zone " + snappedZoneId);

                    // Zone id start index at 0
                    // Can be used to replace at specific list index
                    _blocks[snappedZoneId] = null;
                } 
            }
        }

        private void RegisterLevelSceneLoadEventHandler(SceneLoad levelObj)
        {
            levelObj.ProgramBlockShiftRight += new SceneLoad.ProgramBlockShiftEventHandler(DoProgramBlockShiftRight);
            levelObj.ProgramBlockShiftRevert += new SceneLoad.ProgramBlockShiftEventHandler(DoProgramBlockShiftRevert);
            levelObj.ProgramBlockPlace += new SceneLoad.ProgramBlockPlaceEventHandler(DoProgramBlockPlace);
            levelObj.ProgramBlockSnap += new SceneLoad.ProgramBlockSnapEventHandler(DoProgramBlockSnap);
        }

        private void RegisterSnapDropZoneEventHandler(GameObject snapDropZone)
        {
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneEntered);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneSnapped);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneExited);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneUnsnapped);
        }

        private void DoProgramBlockSnap(GameObject block, int zoneId)
        {
            Debug.Log("DoProgramBlockSnap at zone " + zoneId);
            VRTK_SnapDropZone nextVrtkZone = _zones[zoneId].GetComponent<VRTK_SnapDropZone>();
            nextVrtkZone.ForceSnap(block);

            var pb = block.GetComponent<ProgramBlock>();
            pb.ZoneId = zoneId;
            _blocks[zoneId] = pb;

            DestroyAllProposedBlocks();
        }

        private void DoProgramBlockPlace(int startZoneIndex)
        {
            Debug.Log("DoProgramBlockPlace ");
            while (!IsStackMoveZoneEmpty())
            {
                StackItemMoveZone item = _stackMoveZones.Pop();
                MoveSnappedBlock(item.From, item.To);
            }
            DestroyAllProposedBlocks();
        }

        private void DoProgramBlockShiftRevert(int startZoneIndex)
        {
            Debug.Log("DoProgramBlockShiftRevert at index " + startZoneIndex);
            //RevertStackMoveZone();
            DestroyAllProposedBlocks();
        }

        private void DoProgramBlockShiftRight(int startZoneIndex)
        {
            //Debug.Log("DoProgramBlockShiftRight at index " + startZoneIndex);

            if (startZoneIndex < maximumNumberOfSnapDropZones - 1)
            {
                DoProgramBlockShiftRight(startZoneIndex + 1);
            }
            if (startZoneIndex == maximumNumberOfSnapDropZones - 1)
            {
                return;
            }

            bool isNextFilled = _blocks[startZoneIndex + 1] != null;
            bool isCurrentFilled = _blocks[startZoneIndex] != null;

            if (!isCurrentFilled)
            {
                // Since current zone is not filled
                // Can skip
                return;
            }
            if (isNextFilled)
            {
                // since next zone is filled
                // does not need to shift right
                return;
            }

            //Debug.Log("DoProgramBlockShiftRight start shift from " + startZoneIndex + " to " + (startZoneIndex + 1));
            if (!isStackMoveZoneContain(startZoneIndex, startZoneIndex + 1))
            {
                _stackMoveZones.Push(new StackItemMoveZone { From = startZoneIndex, To = startZoneIndex + 1 });
            }

            DisplayProposedBlock(startZoneIndex + 1);
        }

        private void MoveSnappedBlock(int fromIndex, int toIndex)
        {
            Debug.Log("MoveSnappedBlock: From " + fromIndex + " - to " + toIndex);
            var currentBlock = (ProgramBlock) _blocks[fromIndex];

            VRTK_SnapDropZone currentVrtkZone = currentBlock.GetVRTKSnapDropZone();
            currentVrtkZone.ForceUnsnap();

            VRTK_SnapDropZone nextVrtkZone = _zones[toIndex].GetComponent<VRTK_SnapDropZone>();
            nextVrtkZone.ForceSnap(currentBlock.gameObject);
        }

        private void DisplayProposedBlock(int zoneIndex)
        {
            var zonePosition = _zones[zoneIndex].GetComponent<VRTK_SnapDropZone>().transform.position;
            var proposedBlock = Instantiate(_proposedBlockPrefab, zonePosition,
               Quaternion.identity);
            _proposedBlocks.Add(proposedBlock);
        }

        private void DestroyAllProposedBlocks()
        {
            foreach (var block in _proposedBlocks)
            {
                DestoryProposedBlock(block);
            }
        }

        private void DestoryProposedBlock(GameObject obj)
        {
            Destroy(obj);
        }

        //private void RevertStackMoveZone()
        //{
        //    Debug.Log("RevertStackMoveZone");
        //    while (!IsStackMoveZoneEmpty())
        //    {
        //        StackItemMoveZone item = _stackMoveZones.Pop();
        //        MoveSnappedBlock(item.To, item.From);
        //    }
        //}

        //private void ClearStackMoveZone()
        //{
        //    Debug.Log("ClearStackMoveZone");
        //    _stackMoveZones.Clear();
        //}

        private GameObject AddSnapDropZone()
        {
            if (_numOfSnapDropZone >= maximumNumberOfSnapDropZones)
            {
                Debug.Log("AddSnapDropZone: Reach maximum number = " + maximumNumberOfSnapDropZones + " of snap drop zones!");
                return null;
            }

            var obj = CreateSnapDropZone(new Vector3(
                _defaultSnapDropZonePosition.x,
                _defaultSnapDropZonePosition.y,
                _defaultSnapDropZonePosition.z + (_defaultSnapDropZonePrefab.transform.localScale.z * _numOfSnapDropZone)));

            RegisterSnapDropZoneEventHandler(obj);
            // Set Zone Id
            // Since a default zone is setup of id=0,
            // New zone start from id=1
            obj.AddComponent<SnapDropZone>().ZoneId = _numOfSnapDropZone;

            _numOfSnapDropZone++;

            return obj;
        }

        private GameObject CreateSnapDropZone(Vector3 position)
        {
            if (snapDropZonePrefab == null)
            {
                Debug.Log("CreateSnapDropZone: SnapDropZone prefab is not defined!");
                return null;
            }
            var snapDropZone = Instantiate(
              snapDropZonePrefab,
              position,
              Quaternion.identity);
            snapDropZone.transform.SetParent(gameObject.transform);

            return snapDropZone;
        }

        private SnapDropZone GetProgramBlockSnappedDropZone(ProgramBlock block)
        {
            return block.GetSnapDropZone();
        }

        private bool isStackMoveZoneContain(int from, int to)
        {
            foreach(StackItemMoveZone item in _stackMoveZones)
            {
                if (item.From == from && item.To == to) { return true; }
            }
            return false;
        }

        private bool IsStackMoveZoneEmpty()
        {
            if (_stackMoveZones == null) { return true; }
            return _stackMoveZones.Count == 0;
        }

    }
}

