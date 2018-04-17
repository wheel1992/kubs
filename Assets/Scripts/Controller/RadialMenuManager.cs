using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VRTK;
using UnityEngine.SceneManagement;

namespace Kubs
{
    public class RadialMenuManager : MonoBehaviour
    {
        private VRTK_RadialMenu _radialMenu;
        [SerializeField] private GameObject controller;
        [SerializeField] private Sprite iconPointer;
        [SerializeField] private Sprite iconTeleport;
        [SerializeField] private int IndexPointer;
        [SerializeField] private int IndexHelp;
        [SerializeField] private int IndexMenu;
        private GameObject controllerTooltips;
        private GameObject radialMenuTooltips;
        private bool isPointerEnabled = false;
        private bool isMenuEnabled = false;

        public void HandleMenuDisable()
        {
            Debug.Log("HandleMenuDisable");
            var menuPointer = GetRadialMenuButton(IndexPointer);
            menuPointer.ButtonIcon = iconTeleport;
            _radialMenu.RegenerateButtons();

            isMenuEnabled = false;
            isPointerEnabled = false;
        }
        public void HandleMenuEnable()
        {
            var menuPointer = GetRadialMenuButton(IndexPointer);
            menuPointer.ButtonIcon = iconPointer;
            _radialMenu.RegenerateButtons();
        }

        #region Pointer Events
        public void OnPointerClick()
        {

        }
        public void OnPointerHoverEnter()
        {
            isPointerEnabled = true;
        }
        public void OnPointerHoverExit()
        {
            isPointerEnabled = false;
        }

        #endregion

        #region Menu Events
        public void OnMenuClick()
        {
            if (isMenuEnabled)
            {
                // Current enable, click to disable
                isMenuEnabled = false;
                EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_DISABLE, null);
            }
            else
            {
                // Current disable, click to enable
                isMenuEnabled = true;
                EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_ENABLE, null);
            }
        }
        #endregion

        #region Help Events
        public void OnHelpClick()
        {
            EnableTooltips();
        }
        public void OnHelpHoverEnter()
        {
            EnableTooltips();
        }
        public void OnHelpHoverExit()
        {
            DisableTooltips();
        }
        #endregion 

        // Use this for initialization
        void Start()
        {
            _radialMenu = GetVRTKRadialMenu();
            controllerTooltips = GetVRTKControllerTooltips().gameObject;
            radialMenuTooltips = GetRadialMenuTooltips();
            DisableTooltips();
            DisablePointer();
        }

        // Update is called once per frame
        void Update()
        {
            if (isMenuEnabled)
            {

                if (isPointerEnabled)
                {
                    EnablePointer(false);
                }
                else
                {
                    DisablePointer();
                }
            }
            else
            {
                if (isPointerEnabled)
                {
                    EnablePointer(true);
                }
                else
                {
                    DisablePointer();
                }
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
        void EnableTooltips()
        {
            controllerTooltips.SetActive(true);
            radialMenuTooltips.SetActive(true);
        }
        void DisablePointer()
        {
            var controller = GetController();
            if (controller == null) { return; }
            var pointer = GetPointer(controller);
            if (pointer == null) { return; }

            pointer.Toggle(false);
        }
        void DisableTooltips()
        {
            controllerTooltips.SetActive(false);
            radialMenuTooltips.SetActive(false);
        }
        GameObject GetController()
        {
            if (transform.parent == null) { return null; }
            return transform.parent.gameObject;
        }
        VRTK_ControllerTooltips GetVRTKControllerTooltips()
        {
            var tooltip = controller.transform.Find("ControllerTooltips");
            if (tooltip == null) { return null; }
            return tooltip.GetComponent<VRTK_ControllerTooltips>();
        }
        GameObject GetRadialMenuTooltips()
        {
            var tooltip = controller.transform.Find("RadialMenuTooltips");
            if (tooltip == null) { return null; }
            return tooltip.gameObject;
        }
        VRTK_Pointer GetPointer(GameObject obj)
        {
            return controller.GetComponent<VRTK_Pointer>();
        }
        VRTK_RadialMenu GetVRTKRadialMenu()
        {
            return gameObject.GetComponent<VRTK_RadialMenu>();
        }
        VRTK_RadialMenu.RadialMenuButton GetRadialMenuButton(int index)
        {
            return _radialMenu.GetButton(index);
        }
    }
}

