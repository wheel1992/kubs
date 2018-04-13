using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VRTK;
using UnityEngine.SceneManagement;

namespace Kubs
{
    public class RadialMenuItems : MonoBehaviour
    {
        private bool isPointerEnabled = false;
        private bool isPointerAllowTeleport = false;
        private bool isMenuEnabled = false;

        public void OnOptionMenuClick()
        {
            // Debug.Log("RadialMenuItems OnOptionMenuClick:");
            if (isMenuEnabled)
            {
                // Current enable, click to disable
                EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_DISABLE, null);
            }
            else
            {
                // Current disable, click to enable
                EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_ENABLE, null);
            }

            isMenuEnabled = !isMenuEnabled;
        }
        public void OnOptionSelectClick()
        {
            // Debug.Log("RadialMenuItems OnOptionSelectClick:");
        }
        public void OnOptionSelectHoverEnter()
        {
            // Debug.Log("RadialMenuItems OnOptionSelectHoverEnter:");
            isPointerEnabled = true;
            isPointerAllowTeleport = false;
        }
        public void OnOptionSelectHoverExit()
        {
            // Debug.Log("RadialMenuItems OnOptionSelectHoverExit:");
            isPointerEnabled = false;
        }
        public void OnOptionTeleportClick()
        {
            // Debug.Log("RadialMenuItems OnOptionTeleportClick:");
        }
        public void OnOptionTeleportHoverEnter()
        {
            // Debug.Log("RadialMenuItems OnOptionTeleportHoverEnter:");
            isPointerEnabled = true;
            isPointerAllowTeleport = true;
        }
        public void OnOptionTeleportHoverExit()
        {
            // Debug.Log("RadialMenuItems OnOptionSelectHoverExit:");
            isPointerEnabled = false;
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

            pointer.enableTeleport = enableTeleport;
            pointer.Toggle(true);
        }
        void DisablePointer()
        {
            var controller = GetController();
            if (controller == null) { return; }
            var pointer = GetPointer(controller);
            if (pointer == null) { return; }

            pointer.Toggle(false);
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

