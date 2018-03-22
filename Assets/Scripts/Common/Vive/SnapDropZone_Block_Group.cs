﻿using System.Collections;
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
        [SerializeField] private GameObject _topColliderGroup;
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
        //private IList<GameObject> _topColliderObjects;

        //private static int SnappedBlockIndex = 0;
        private static int _numOfSnapDropZone = 1;
        private static int _snappedBlockCount = 0;

        private void Start()
        {
            _blocks = new List<Block>();
            _zones = new List<GameObject>();
            //_topColliderObjects = new List<GameObject>();

            _defaultSnapDropZonePrefab = transform.Find(Constant.NAME_SNAP_DROP_ZONE_PROGRAM_BLOCK).gameObject;
            _defaultSnapDropZonePrefab.GetComponent<SnapDropZone>().ZoneId = 0;
            _defaultSnapDropZonePosition = _defaultSnapDropZonePrefab.transform.position;

            RegisterSnapDropZoneEventHandler(_defaultSnapDropZonePrefab);

            RegisterLevelSceneLoadEventHandler(levelPrefab.GetComponent<SceneLoad>());


            for (int i = 0; i <= maximumNumberOfSnapDropZones; i++)
            {
                _zones.Add(AddSnapDropZone());
                _blocks.Add(null);
                //_topColliderObjects.Add(null);
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

                // Zone id start index at 0
                // Can be used to replace at specific list index
                _blocks[snappedZoneId] = block;

                block.ZoneId = snappedZoneId;

            } else
            {
                Debug.Log("DoProgramBlockZoneSnapped snappedObject is null!!!");
            } 
        }

        private void DoProgramBlockZoneExited(object sender, SnapDropZoneEventArgs e)
        {
            Debug.Log("== SnapDropZone: EXITED >>>>");
            if (e.snappedObject != null)
            {
                Debug.Log("DoProgramBlockZoneExited object tag = " + e.snappedObject.tag);

                if (e.snappedObject.tag.CompareTo(Constant.TAG_BLOCK_PROGRAM) == 0)
                {
                    ProgramBlock block = e.snappedObject.GetComponent<ProgramBlock>();
                    //int snappedZoneId = GetProgramBlockSnappedDropZone(block).ZoneId;
                    int snappedZoneId = _blocks.IndexOf(block);
                    Debug.Log("DoProgramBlockZoneExited to zone " + snappedZoneId);

                    block.ZoneId = -1;
                }
            }
            
        }

        private void DoProgramBlockZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                Debug.Log("DoProgramBlockZoneUnsnapped object tag = " + e.snappedObject.tag);

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
            levelObj.ProgramBlockShiftRight += new SceneLoad.ProgramBlockShiftRightEventHandler(DoProgramBlockShiftRight);
        }

        private void RegisterSnapDropZoneEventHandler(GameObject snapDropZone)
        {
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneEntered);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneSnapped);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneExited);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneUnsnapped);
        }

        

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

        //private void AddTopColliderObject(int zoneId, Block block)
        //{
        //    var blockPosition = block.transform.position;
        //    var blockHeight = block.transform.localScale.y;
        //    var blockWidth = block.transform.localScale.z;
        //    var colliderObject = CreateTopColliderObject(
        //        new Vector3(
        //            blockPosition.x,
        //            blockPosition.y + blockHeight,
        //            blockPosition.z - (blockWidth / 2)));

        //    _topColliderObjects[zoneId] = colliderObject;
        //}

        private GameObject CreateSnapDropZone(Vector3 position)
        {
            if (snapDropZonePrefab == null) {
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

        //private GameObject CreateTopColliderObject(Vector3 position)
        //{
        //    if (topColliderObjectPrefab == null)
        //    {
        //        Debug.Log("CreateTopColliderObject: topColliderObject prefab is not defined!");
        //        return null;
        //    }

        //    var colliderObject = Instantiate(
        //     topColliderObjectPrefab,
        //     position,
        //     Quaternion.identity);

        //    colliderObject.AddComponent<TopColliderObject_Controller>();
        //    colliderObject.transform.SetParent(_topColliderGroup.transform);

        //    return colliderObject;
        //}

        private void DoProgramBlockShiftRight(int startZoneIndex)
        {
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
                Debug.Log("ShiftRight: Current " + startZoneIndex + " is filled. End shift.");
                return;
            }

            if (isNextFilled)
            {
                // since next zone is filled
                // dont need to shift right already
                Debug.Log("ShiftRight: Current " + startZoneIndex + " Next " + (startZoneIndex + 1) + " is filled. End shift.");
                return;
            }

            var currentBlock = (ProgramBlock) _blocks[startZoneIndex];
            
            VRTK_SnapDropZone currentVrtkZone = currentBlock.GetVRTKSnapDropZone();
            currentVrtkZone.ForceUnsnap();

            VRTK_SnapDropZone nextVrtkZone = _zones[startZoneIndex + 1].GetComponent<VRTK_SnapDropZone>();
            nextVrtkZone.ForceSnap(currentBlock.gameObject);

        }


        private SnapDropZone GetProgramBlockSnappedDropZone(ProgramBlock block)
        {
            return block.GetSnapDropZone();
        }

        private GameObject GetTopColliderGroup()
        {
            return transform.Find(Constant.NAME_TOP_COLLIDER_GROUP).gameObject;
        }

        //private void RemoveTopColliderObject(int zoneId)
        //{
        //    var obj = _topColliderObjects[zoneId];
        //    _topColliderObjects[zoneId] = null;
        //    Destroy(obj);
        //}
    }
}

