using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class ForLoopEnd : MonoBehaviour
    {
        public ForLoopStart ForLoopStart { get; set; }

        #region Public Methods
        public void SetActive()
        {
            gameObject.SetActive(true);
        }
        public void SetInactive()
        {
            gameObject.SetActive(false);
        }
        public ForLoopStart GetParentForLoopStart()
        {
            var block = GetProgramBlock();
            if (block == null) { return null; }

            var parent = block.GetParent();
            if (parent == null) { return null; }

            // Update its reference for ForLoopStart
            if (parent.GetComponent<ForLoopStart>() == null)
            {
                return null;
            }

            this.ForLoopStart = parent.GetComponent<ForLoopStart>();
            return this.ForLoopStart;
        }
        public bool IsInZone()
        {
            // Debug.Log("HasZoneIndex: ZoneIndex = " + ZoneIndex);
            // return ZoneIndex != -1;
            var block = GetProgramBlock();
            if (block == null) { return false; }
            return block.IsInZone();
        }
        public ProgramBlock GetProgramBlock()
        {
            var block = gameObject.GetComponent<ProgramBlock>();
            return block;
        }
        public int GetZoneIndex()
        {
            var block = GetProgramBlock();
            if (block == null) { return -1; }

            return block.GetZoneIndex();
        }

        #endregion
        // Use this for initialization
        void Start()
        {
            this.ForLoopStart = GetParentForLoopStart();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Private Get methods
        #endregion
    }
}

