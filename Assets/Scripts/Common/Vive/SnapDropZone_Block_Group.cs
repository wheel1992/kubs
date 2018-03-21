using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SnapDropZone_Block_Group : MonoBehaviour {

    private GameObject _programBlockZone;
    private GameObject sphereZone;

    private void Start()
    {
        _programBlockZone = transform.Find("Block_SnapDropZone").gameObject;
        //sphereZone = transform.Find("Sphere_SnapDropZone").gameObject;

        _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoCubeZoneSnapped);
        _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoCubeZoneSnapped);
        _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoCubeZoneUnsnapped);
        _programBlockZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoCubeZoneUnsnapped);

        //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
        //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoSphereZoneSnapped);
        //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
        //sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoSphereZoneUnsnapped);
    }

    private void DoCubeZoneSnapped(object sender, SnapDropZoneEventArgs e)
    {
        //sphereZone.SetActive(false);
    }

    private void DoCubeZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
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
