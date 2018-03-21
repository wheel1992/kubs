using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class SnapDropZone_Block_Group : MonoBehaviour
    {

        private GameObject _programBlockZone;
        //private GameObject sphereZone;

        private void Start()
        {
            _programBlockZone = transform.Find(Constant.NAME_SNAP_DROP_ZONE_PROGRAM_BLOCK).gameObject;
            //sphereZone = transform.Find("Sphere_SnapDropZone").gameObject;

            _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneSnapped);
            _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneSnapped);
            _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneUnsnapped);
            _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoProgramBlockZoneUnsnapped);

            //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
            //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
            //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
            //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
        }

        private void DoProgramBlockZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            //sphereZone.SetActive(false);
        }

        private void DoProgramBlockZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            //sphereZone.SetActive(true);
        }

        //private void DoSphereZoneSnapped(object sender, SnapDropZoneEventArgs e)
        //{
        //    cubeZone.SetActive(false);
        //}

        //private void DoSphereZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        //{
        //    cubeZone.SetActive(true);
        //}
    }
}

