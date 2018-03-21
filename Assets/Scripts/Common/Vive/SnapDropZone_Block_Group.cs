using UnityEngine;
using VRTK;

namespace Kubs
{
    public class SnapDropZone_Block_Group : MonoBehaviour
    {
        [SerializeField] private int maximumNumberOfSnapDropZones = 10;
        [SerializeField] private GameObject snapDropZonePrefab;

        /// <summary>
        /// This holds the default first snap drop zone gameobject
        /// </summary>
        private GameObject _defaultSnapDropZonePrefab;
        private Vector3 _defaultSnapDropZonePosition;

        //private static int SnappedBlockIndex = 0; 
        private static int _numOfSnapDropZone = 1;
        private static int _snappedBlockCount = 0;

        //private GameObject _programBlockZone;
        //private GameObject sphereZone;

        private void Start()
        {
            _defaultSnapDropZonePrefab = transform.Find(Constant.NAME_SNAP_DROP_ZONE_PROGRAM_BLOCK).gameObject;
            _defaultSnapDropZonePosition = _defaultSnapDropZonePrefab.transform.position;

            RegisterSnapDropZoneEventHandler(_defaultSnapDropZonePrefab);

            for(int i = 0; i <= maximumNumberOfSnapDropZones; i++)
            {
                AddSnapDropZone();
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
            }

            _snappedBlockCount++;
            Debug.Log("== SnapDropZone: SNAPPED count = " + _snappedBlockCount);
        }

        private void DoProgramBlockZoneExited(object sender, SnapDropZoneEventArgs e)
        {
            Debug.Log("== SnapDropZone: EXITED >>>>");
        }

        private void DoProgramBlockZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                ProgramBlock block = e.snappedObject.GetComponent<ProgramBlock>();
            }
            _snappedBlockCount--;
            Debug.Log("== SnapDropZone: UNSNAPPED count = " + _snappedBlockCount);
        }

        private void RegisterSnapDropZoneEventHandler(GameObject snapDropZone)
        {
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneEntered);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneSnapped);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneExited);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneUnsnapped);
        }

        private void AddSnapDropZone()
        {
            if (_numOfSnapDropZone >= maximumNumberOfSnapDropZones)
            {
                // ...
                Debug.Log("AddSnapDropZone: Reach maximum number = " + maximumNumberOfSnapDropZones + " of snap drop zones!");
                return;
            }

            CreateSnapDropZone(new Vector3(
                _defaultSnapDropZonePosition.x, 
                _defaultSnapDropZonePosition.y, 
                _defaultSnapDropZonePosition.z + (_defaultSnapDropZonePrefab.transform.localScale.z * _numOfSnapDropZone)));

            _numOfSnapDropZone++;
        }

        private GameObject CreateSnapDropZone(Vector3 position)
        {
            if (snapDropZonePrefab == null) {
                Debug.Log("CreateSnapDropZone: SnapDropZone prefab is not defined!");
                return null;
            }
            var snapDropZone = (GameObject)Instantiate(
              snapDropZonePrefab,
              position,
              Quaternion.identity);
            snapDropZone.transform.SetParent(gameObject.transform);
            return snapDropZone;
        }

        private void RemoveSnapDropZone()
        {
            if (_numOfSnapDropZone == 1)
            {
                // ...
                Debug.Log("RemoveSnapDropZone: Cannot go lower than 1 snap drop zone!");
                return;
            }

            // ...

            _numOfSnapDropZone--;
        }

    }
}

