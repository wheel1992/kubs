using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class RadialMenuItems : MonoBehaviour
    {
        private bool isPointerEnabled = false;
        private bool isPointerAllowTeleport = false;

        public void OnOptionMenuClick()
        {
            Debug.Log("RadialMenuItems OnOptionMenuClick:");
        }
        public void OnOptionSelectClick()
        {
            Debug.Log("RadialMenuItems OnOptionSelectClick:");
            // Debug.Log("RadialMenuItems OnOptionSelectClick: " + transform.parent.name);
            // EnablePointer(false);
            isPointerEnabled = true;
            isPointerAllowTeleport = false;
        }
        public void OnOptionSelectHoverExit()
        {
            Debug.Log("RadialMenuItems OnOptionSelectHoverExit:");
            // DisablePointer();
            isPointerEnabled = false;
        }
        public void OnOptionTeleportClick()
        {
            Debug.Log("RadialMenuItems OnOptionTeleportClick:");
        }

        // Use this for initialization
        void Start()
        {
            DisablePointer();
        }

        // Update is called once per frame
        void Update()
        {
            if (isPointerEnabled)
            {
                EnablePointer(isPointerAllowTeleport);
            }
            else
            {
                DisablePointer();
            }
        }
        void EnablePointer(bool enableTeleport)
        {
            var controller = GetController();
            if (controller == null) { return; }
            var pointer = GetPointer(controller);
            if (pointer == null) { return; }

            pointer.enabled = true;
            pointer.enableTeleport = enableTeleport;

            pointer.activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
            pointer.holdButtonToActivate = true;

            pointer.selectionButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
            pointer.selectOnPress = true;
        }
        void DisablePointer()
        {
            var controller = GetController();
            if (controller == null) { return; }
            var pointer = GetPointer(controller);
            if (pointer == null) { return; }

            pointer.enabled = false;
            pointer.enableTeleport = false;

            pointer.activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            pointer.holdButtonToActivate = false;

            pointer.selectionButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            pointer.selectOnPress = false;
        }
        GameObject GetController()
        {
            if (transform.parent == null) { return null; }
            return transform.parent.gameObject;
        }
        VRTK_Pointer GetPointer(GameObject obj)
        {
            return obj.GetComponent<VRTK_Pointer>();
        }
    }
}

