using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class ForLoopEnd : MonoBehaviour
    {
        public ForLoopStart ForLoopStart { get; set; }

        #region Public Methods
        public void Enable()
        {
            // gameObject.GetComponent<BoxCollider>().enabled = true;
            // gameObject.GetComponent<Rigidbody>().isKinematic = true;
            // EnableGrab();
            SetActive();
        }
        public void Disable()
        {
            // gameObject.GetComponent<BoxCollider>().enabled = false;
            // gameObject.GetComponent<Rigidbody>().isKinematic = true;
            // DisableGrab();
            SetInactive();
        }
        public void DisableGrab()
        {
            GetProgramBlock().GetVRTKInteractableObject().isGrabbable = false;
            GetProgramBlock().GetVRTKInteractableObject().useOnlyIfGrabbed = false;
        }
        public void EnableGrab()
        {
            GetProgramBlock().GetVRTKInteractableObject().isGrabbable = true;
            GetProgramBlock().GetVRTKInteractableObject().useOnlyIfGrabbed = true;
        }
        public void SetActive()
        {
            gameObject.SetActive(true);
        }
        public void SetInactive()
        {
            gameObject.SetActive(false);
        }
        public void SetParent(Transform parent)
        {
            GetProgramBlock().SetParent(parent);
            transform.rotation = parent.rotation;
        }
        // public ForLoopStart GetParentForLoopStart()
        // {
        //     var block = GetProgramBlock();
        //     if (block == null) { return null; }

        //     var parent = block.GetParent();
        //     if (parent == null) { return null; }

        //     // Update its reference for ForLoopStart
        //     if (parent.GetComponent<ForLoopStart>() == null)
        //     {
        //         return null;
        //     }

        //     this.ForLoopStart = parent.GetComponent<ForLoopStart>();
        //     return this.ForLoopStart;
        // }
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
            //this.ForLoopStart = GetParentForLoopStart();

            //GetProgramBlock().GetVRTKInteractableObject().InteractableObjectSnappedToDropZone += new InteractableObjectEventHandler(HandleOnSnappedToDropZone);
            // GetProgramBlock().GetVRTKInteractableObject().InteractableObjectSnappedToDropZone +=
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region  Private Event Handler Methods
        private void HandleOnSnappedToDropZone(object sender, InteractableObjectEventArgs args)
        {
            if (sender is VRTK_InteractableObject)
            {
                var interactableObject = (VRTK_InteractableObject)sender;
                // Ungrabbed and dropped not within the Zone (aka outside)
                var forEndBlock = interactableObject.GetComponent<ForLoopEnd>();
                if (interactableObject.IsInSnapDropZone() && !forEndBlock.GetProgramBlock().IsInSnapDropZoneClone())
                { 
                    if (forEndBlock != null)
                    {
                        //forEndBlock.SetParent(interactableObject.gameObject.transform);
                        //forEndBlock.Enable();
                    }
                }
            }
        }
        #endregion

        #region Private Get methods
        #endregion
    }
}

