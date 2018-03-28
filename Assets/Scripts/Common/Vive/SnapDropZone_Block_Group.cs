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
        [SerializeField] private GameObject tempPositionObjectPrefab;
        [SerializeField] private GameObject _proposedBlockPrefab;


        /// <summary>
        /// This holds the default first snap drop zone gameobject
        /// </summary>
        private GameObject _defaultSnapDropZonePrefab;
        private Vector3 _defaultSnapDropZonePosition;
        private IList<GameObject> _zones;
        private IList<GameObject> _tempPositionObjects;
        private Stack<StackItemMoveZone> _stackMoveZones;
        private static int _numOfSnapDropZone = 1;
        private Vector3 _prevPos;

        public List<ProgramBlock> GetListOfSnappedProgramBlocks()
        {
            List<ProgramBlock> blocks = new List<ProgramBlock>();
            foreach (var zone in _zones)
            {
                var obj = GetGameObjectBySnapIndex(zone.GetComponent<VRTK_SnapDropZone>().GetComponent<SnapDropZone>().ZoneId);
                if (obj != null)
                {
                    blocks.Add(GetProgramBlockByObject(obj));
                }
            }
            return blocks;
        }

        private void Start()
        {
            _zones = new List<GameObject>();
            _tempPositionObjects = new List<GameObject>();

            _stackMoveZones = new Stack<StackItemMoveZone>();

            _defaultSnapDropZonePrefab = transform.Find(Constant.NAME_SNAP_DROP_ZONE_PROGRAM_BLOCK).gameObject;
            _defaultSnapDropZonePrefab.GetComponent<SnapDropZone>().ZoneId = 0;
            _defaultSnapDropZonePosition = _defaultSnapDropZonePrefab.transform.position;

            _prevPos = _defaultSnapDropZonePosition;

            RegisterSnapDropZoneEventHandler(_defaultSnapDropZonePrefab);
            RegisterLevelSceneLoadEventHandler(levelPrefab.GetComponent<SceneLoad>());

            _zones.Add(_defaultSnapDropZonePrefab);
            _tempPositionObjects.Add(null);
            for (int i = 1; i < maximumNumberOfSnapDropZones; i++)
            {
                _zones.Add(AddSnapDropZone());
                _tempPositionObjects.Add(null);
            }
        }

        private void DoProgramBlockZoneEntered(object sender, SnapDropZoneEventArgs e)
        {
            //Debug.Log("== SnapDropZone: ENTERING <<<<");
        }

        private void DoProgramBlockZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            //Debug.Log("== SnapDropZone: SNAPPED >>>> [ ] <<<<");
            if (sender is VRTK_SnapDropZone)
            {
                VRTK_SnapDropZone originZone = (VRTK_SnapDropZone)sender;
                int zoneId = originZone.GetComponent<SnapDropZone>().ZoneId;
                DecreaseZoneHeight(zoneId);
                Destroy(GetTemporaryPositionObjectByZoneId(zoneId));

                ProgramBlock block = GetProgramBlockByObject(GetGameObjectBySnapIndex(zoneId));
                //Debug.Log("== SnapDropZone: SNAPPED block state = " + block.State.ToString());
                Debug.Log("BEFORE PRINT ");
                PrintBlockTypes();

                if (block.State != State.SnapTempMove)
                {
                    if (IsPreviousZoneEmpty(zoneId))
                    {
                        MoveSnappedBlock(zoneId, zoneId - 1, false);
                    }
                }
                Debug.Log("AFTER PRINT ");
                PrintBlockTypes();
            }
        }

        private void DoProgramBlockZoneExited(object sender, SnapDropZoneEventArgs e)
        {
            //Debug.Log("== SnapDropZone: EXITED >>>>");
            if (sender is VRTK_SnapDropZone)
            {
                VRTK_SnapDropZone originZone = (VRTK_SnapDropZone)sender;
                DecreaseZoneHeight(originZone.GetComponent<SnapDropZone>().ZoneId);
            }

            if (e.snappedObject != null)
            {
                e.snappedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }

        private void DoProgramBlockZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            Debug.Log("== SnapDropZone: UNSNAPPED >>>>");
            if (e.snappedObject != null)
            {
                e.snappedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
        }

        private void RegisterLevelSceneLoadEventHandler(SceneLoad levelObj)
        {
            levelObj.ProgramBlockShiftRightWhenHover += new SceneLoad.ProgramBlockShiftEventHandler(DoProgramBlockShiftRightWhenHover);
            levelObj.ProgramBlockShiftRevert += new SceneLoad.ProgramBlockShiftEventHandler(DoProgramBlockShiftRevert);
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
            DecreaseZoneHeight(zoneId);
            DestroyAllTemporaryPositionObjects();

            // By right all items in stack are still alive
            foreach (StackItemMoveZone item in _stackMoveZones)
            {
                GetProgramBlockByObject(GetGameObjectBySnapIndex(item.To)).State = State.SnapIdle;
            }

            ClearStackMoveZone();

            SnapAt(GetProgramBlockByObject(block), zoneId);

            PrintBlockStates();
        }
        private void DoProgramBlockShiftRevert(int startZoneIndex)
        {
            //Debug.Log("DoProgramBlockShiftRevert");

            // Remove all temporary position objects
            DestroyAllTemporaryPositionObjects();

            // Set all VRTK_SnapDropZone to default height
            for (var i = 0; i < _zones.Count; i++)
            {
                DecreaseZoneHeight(i);
            }

            // Move all snapped blocks back to original position
            while (!IsStackMoveZoneEmpty())
            {
                StackItemMoveZone item = _stackMoveZones.Pop();
                MoveSnappedBlock(item.To, item.From, false);
            }

            //PrintBlockStates();
        }
        private void DoProgramBlockShiftRightWhenHover(int startZoneIndex)
        {
            if (!IsStackMoveZoneEmpty())
            {
                StackItemMoveZone item = _stackMoveZones.Pop();
                MoveSnappedBlock(item.To, item.From, false);
            }

            IncreaseZoneHeight(startZoneIndex);
            _tempPositionObjects.Insert(startZoneIndex, CreateTemporaryPositionObject(GetGameObjectBySnapIndex(startZoneIndex).transform.position));
            ClearStackMoveZone();
            ShiftRight(startZoneIndex);

            //PrintBlockStates();
        }
        private void ShiftRight(int startZoneIndex)
        {
            //Debug.Log("ShiftRight at " + startZoneIndex);

            bool isCurrentFilled = GetGameObjectBySnapIndex(startZoneIndex) != null;
            if (!isCurrentFilled)
            {
                // Since current zone is not filled
                // Can skip
                //Debug.Log("ShiftRight index at current " + startZoneIndex + " is filled!");
                return;
            }

            if (startZoneIndex < maximumNumberOfSnapDropZones - 1)
            {
                ShiftRight(startZoneIndex + 1);
            }

            if (startZoneIndex == maximumNumberOfSnapDropZones - 1)
            {
                return;
            }

            bool isNextFilled = GetGameObjectBySnapIndex(startZoneIndex + 1) != null;
            if (isNextFilled)
            {
                // since next zone is filled
                // does not need to shift right
                //Debug.Log("ShiftRight index at next " + (startZoneIndex + 1) + " is not filled!");
                return;
            }

            Debug.Log("ShiftRight start shift from " + startZoneIndex + " to " + (startZoneIndex + 1));
            if (!isStackMoveZoneContain(startZoneIndex, startZoneIndex + 1))
            {
                _stackMoveZones.Push(new StackItemMoveZone { From = startZoneIndex, To = startZoneIndex + 1 });
                MoveSnappedBlock(startZoneIndex, startZoneIndex + 1, true);
            }
        }
        private void SnapRemove(ProgramBlock block)
        {
            VRTK_SnapDropZone currentVrtkZone = block.GetVRTKSnapDropZone();
            currentVrtkZone.ForceUnsnap();
        }
        private void SnapAt(ProgramBlock block, int newZoneIndex)
        {
            VRTK_SnapDropZone nextVrtkZone = _zones[newZoneIndex].GetComponent<VRTK_SnapDropZone>();
            nextVrtkZone.ForceSnap(block.gameObject);

            block.ZoneId = nextVrtkZone.GetComponent<SnapDropZone>().ZoneId;
        }
        private void MoveSnappedBlock(int fromIndex, int toIndex, bool isTemporary)
        {
            ProgramBlock block = GetProgramBlockByObject(GetGameObjectBySnapIndex(fromIndex));
            SnapRemove(block);
            SnapAt(block, toIndex);

            if (isTemporary)
            {
                block.State = State.SnapTempMove;
            }
            else
            {
                block.State = State.SnapIdle;
            }
        }
        private void ClearStackMoveZone()
        {
            //Debug.Log("ClearStackMoveZone");
            _stackMoveZones.Clear();
        }
        private void DecreaseZoneHeight(int zoneId)
        {
            _zones[zoneId].transform.localScale = new Vector3(Constant.DEFAULT_SNAP_DROP_ZONE_SCALE, 1, Constant.DEFAULT_SNAP_DROP_ZONE_SCALE);
        }
        private void DestroyAllTemporaryPositionObjects()
        {
            foreach (GameObject obj in _tempPositionObjects)
            {
                Destroy(obj);
            }
        }
        private GameObject GetTemporaryPositionObjectByZoneId(int zoneId)
        {
            return _tempPositionObjects[zoneId];
        }
        private void IncreaseZoneHeight(int zoneId)
        {
            _zones[zoneId].transform.localScale = new Vector3(Constant.DEFAULT_SNAP_DROP_ZONE_SCALE, 2, Constant.DEFAULT_SNAP_DROP_ZONE_SCALE);
        }
        private GameObject AddSnapDropZone()
        {
            if (_numOfSnapDropZone >= maximumNumberOfSnapDropZones)
            {
                //Debug.Log("AddSnapDropZone: Reach maximum number = " + maximumNumberOfSnapDropZones + " of snap drop zones!");
                return null;
            }

            var obj = CreateSnapDropZone(new Vector3(
                _defaultSnapDropZonePosition.x,
                _defaultSnapDropZonePosition.y,
               _prevPos.z + Constant.DEFAULT_BLOCK_SPACING + Constant.DEFAULT_BLOCK_SIZE));

            // _defaultSnapDropZonePosition.z + (Constant.DEFAULT_BLOCK_SIZE * _numOfSnapDropZone)
            _prevPos = obj.transform.position;

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
        private GameObject CreateTemporaryPositionObject(Vector3 position)
        {
            return Instantiate(
              tempPositionObjectPrefab,
              position,
              Quaternion.identity);
        }
        private SnapDropZone GetProgramBlockSnappedDropZone(ProgramBlock block)
        {
            return block.GetSnapDropZone();
        }

        private VRTK_SnapDropZone GetProgramBlockVRTKSnappedDropZone(ProgramBlock block)
        {
            return block.GetVRTKSnapDropZone();
        }
        private GameObject GetGameObjectBySnapIndex(int snapIndex)
        {
            VRTK_SnapDropZone zone = _zones[snapIndex].GetComponent<VRTK_SnapDropZone>();
            return zone.GetCurrentSnappedObject();
        }
        private ProgramBlock GetProgramBlockByObject(GameObject obj)
        {
            return obj.GetComponent<ProgramBlock>();
        }
        private bool IsPreviousZoneEmpty(int currentZoneId)
        {
            if (currentZoneId == 0) { return false; }
            if (currentZoneId >= _zones.Count) { return false; }
            return GetGameObjectBySnapIndex(currentZoneId - 1) == null;
        }
        private bool isStackMoveZoneContain(int from, int to)
        {
            foreach (StackItemMoveZone item in _stackMoveZones)
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
        private void PrintStack()
        {
            Debug.Log("===== PRINT STACK ====");
            foreach (StackItemMoveZone item in _stackMoveZones)
            {
                Debug.Log("From " + item.From + " to " + item.To);
            }
            Debug.Log("======================");
        }

        private void PrintBlockStates()
        {
            string msg = "";
            Debug.Log("===== PRINT BLOCK STATES ====");
            foreach (GameObject zone in _zones)
            {
                GameObject obj = GetGameObjectBySnapIndex(zone.GetComponent<VRTK_SnapDropZone>().GetComponent<SnapDropZone>().ZoneId);
                if (obj == null)
                {
                    msg += "[x] ";
                }
                else
                {
                    msg += "[" + (int)GetProgramBlockByObject(obj).State + "] ";
                }

            }
            Debug.Log(msg);
            Debug.Log("======================");
        }

        private void PrintBlockTypes()
        {
            string msg = "";
            Debug.Log("===== PRINT BLOCK TYPES ====");
            foreach (GameObject zone in _zones)
            {
                GameObject obj = GetGameObjectBySnapIndex(zone.GetComponent<VRTK_SnapDropZone>().GetComponent<SnapDropZone>().ZoneId);
                if (obj == null)
                {
                    msg += "[x] ";
                }
                else
                {
                    msg += "[" + (int)GetProgramBlockByObject(obj).Type + "] ";
                }

            }
            Debug.Log(msg);
            Debug.Log("======================");
        }
    }
}

