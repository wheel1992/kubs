using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public struct ZoneSnapEventArgs
    {
        public ProgramBlock WhichBlock;
    }

    /// <summary>
    /// Handles all captured events from VRTK_SnapDropZone
    /// </summary>
    public class ZoneSnapController : MonoBehaviour
    {
        public delegate void ZoneSnapEventHandler(object sender, ZoneSnapEventArgs args);
        public event ZoneSnapEventHandler OnEntered;
        public event ZoneSnapEventHandler OnExited;
        public event ZoneSnapEventHandler OnSnapped;
        public event ZoneSnapEventHandler OnUnsnapped;

        const string NAME_CHILD_VRTKSNAPDROPZONE = "VRTKSnapDropZone";
        const int CHILD_INDEX_VRTKSNAPDROPZONE = 0;

        private GameObject _child_Vrtk_SnapDropZone;

        #region Public Methods

        public void Snap(GameObject obj) {
            _child_Vrtk_SnapDropZone.GetComponent<VRTK_SnapDropZone>().ForceSnap(obj);
        }
        public void Unsnap() {
            _child_Vrtk_SnapDropZone.GetComponent<VRTK_SnapDropZone>().ForceUnsnap();
        }

        #endregion


        void Start()
        {
            _child_Vrtk_SnapDropZone = gameObject.transform.GetChild(CHILD_INDEX_VRTKSNAPDROPZONE).gameObject;
            RegisterVRTKSnapDropZoneEventHandler(_child_Vrtk_SnapDropZone);
        }
        void Update()
        {
        }
        private void RegisterVRTKSnapDropZoneEventHandler(GameObject snapDropZone)
        {
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneEntered);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneSnapped);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneExited);
            snapDropZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneUnsnapped);
        }
        private void DoSnapDropZoneEntered(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                var block = GetProgramBlockByObject(e.snappedObject);
                if (block != null)
                {
                    OnEntered(this, new ZoneSnapEventArgs { WhichBlock = block });
                }
            }
        }
        private void DoSnapDropZoneSnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                var block = GetProgramBlockByObject(e.snappedObject);
                if (block != null)
                {
                    OnSnapped(this, new ZoneSnapEventArgs { WhichBlock = block });
                }
            }
        }
        private void DoSnapDropZoneExited(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                var block = GetProgramBlockByObject(e.snappedObject);
                if (block != null)
                {
                    OnExited(this, new ZoneSnapEventArgs { WhichBlock = block });
                }
            }
        }
        private void DoSnapDropZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            if (e.snappedObject != null)
            {
                var block = GetProgramBlockByObject(e.snappedObject);
                if (block != null)
                {
                    OnUnsnapped(this, new ZoneSnapEventArgs { WhichBlock = block });
                }
            }
        }

        #region Private Get methods

        private ProgramBlock GetProgramBlockByObject(GameObject obj)
        {
            return obj.GetComponent<ProgramBlock>();
        }

        #endregion
    }
}

