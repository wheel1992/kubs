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
        
        #region Public Methods

        public void Snap(GameObject obj)
        {
            Debug.Log("Snap: ");
            GetChildVRTKSnapDropZone().ForceSnap(obj);
        }
        public void Unsnap()
        {
            GetChildVRTKSnapDropZone().ForceUnsnap();
        }

        #endregion

        #region Private Lifecycle Methods

        void Start()
        {
            RegisterVRTKSnapDropZoneEventHandler(GetChildVRTKSnapDropZone());
        }
        void Update()
        {
        }

        #endregion

        #region Private Methods

        private void RegisterVRTKSnapDropZoneEventHandler(VRTK_SnapDropZone snapDropZone)
        {
            snapDropZone.ObjectEnteredSnapDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneEntered);
            snapDropZone.ObjectSnappedToDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneSnapped);
            snapDropZone.ObjectExitedSnapDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneExited);
            snapDropZone.ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(DoSnapDropZoneUnsnapped);
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

        #endregion

        #region Private Get methods
        private ProgramBlock GetProgramBlockByObject(GameObject obj)
        {
            return obj.GetComponent<ProgramBlock>();
        }
        private VRTK_SnapDropZone GetChildVRTKSnapDropZone()
        {
            return gameObject.transform.GetChild(CHILD_INDEX_VRTKSNAPDROPZONE).GetComponent<VRTK_SnapDropZone>();
        }

        #endregion
    }
}

