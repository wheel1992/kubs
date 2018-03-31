using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public struct ZoneGroupEventArgs
    {
        public GameObject OtherObject;
    }
    public class ZoneGroupController : MonoBehaviour
    {
        public delegate void ZoneGroupEventHandler(object sender, ZoneGroupEventArgs args);
        [SerializeField] private GameObject zonePrefab;

        private List<GameObject> _zones;

        // Use this for initialization
        void Start()
        {
            _zones = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void HandleAddZone(GameObject zoneObject)
        {
            if (zoneObject != null)
            {
                _zones.Add(zoneObject);
                GetZoneByGameObject(zoneObject).Index = _zones.IndexOf(zoneObject);
            }
        }
        private void AddNextZone()
        {
            var nextIndex = _zones.Count;
            var currentZone = _zones[_zones.Count - 1];
            var currentZonePosition = currentZone.transform.position;
            var nextPosition = new Vector3(
                currentZonePosition.x,
                currentZonePosition.y,
                currentZonePosition.z + currentZone.transform.localScale.z);

            CreateZoneGameObject(nextPosition, nextIndex);
        }
        private GameObject CreateZoneGameObject(Vector3 availablePosition, int availableIndex)
        {
            var zone = (GameObject)Instantiate(
             zonePrefab,
             availablePosition,
             Quaternion.identity);

            var controller = zone.AddComponent<ZoneController>();
            controller.Index = availableIndex;

            return zone;
        }
        private bool RemoveZone(int index)
        {
			return false;
        }
        private ZoneController GetZoneByGameObject(GameObject obj)
        {
            return obj.GetComponent<ZoneController>();
        }
    }
}

