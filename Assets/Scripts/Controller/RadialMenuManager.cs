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


        // public void OnOptionMenuClick()
        // {
        //     // Debug.Log("RadialMenuItems OnOptionMenuClick:");
        //     if (isMenuEnabled)
        //     {
        //         // Current enable, click to disable
        //         EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_DISABLE, null);
        //     }
        //     else
        //     {
        //         // Current disable, click to enable
        //         EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_ENABLE, null);
        //     }

        //     isMenuEnabled = !isMenuEnabled;
        // }

        #region Pointer Events
        public void OnPointerClick()
        {

        }
        public void OnPointerHoverEnter()
        {
            Debug.Log("OnPointerHoverEnter:");
            isPointerEnabled = true;
        }
        public void OnPointerHoverExit()
        {
            Debug.Log("OnPointerHoverExit:");
            isPointerEnabled = false;
        }

        #endregion

        #region Menu Events
        public void OnMenuClick()
        {
            Debug.Log("OnMenuClick:");
            // Debug.Log("RadialMenuItems OnMenuClick:");
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

            var menuPointer = GetRadialMenuButton(IndexPointer);
            if (isMenuEnabled)
            {
                menuPointer.ButtonIcon = iconPointer;
            }
            else
            {
                menuPointer.ButtonIcon = iconTeleport;
            }
            _radialMenu.RegenerateButtons();
        }
        #endregion

        #region Help Events
        public void OnHelpClick()
        {
            Debug.Log("OnHelpClick:");
            EnableTooltips();
        }
        public void OnHelpHoverEnter()
        {
            Debug.Log("OnHelpHoverEnter:");
            EnableTooltips();
        }
        public void OnHelpHoverExit()
        {
            Debug.Log("OnHelpHoverExit:");
            DisableTooltips();
        }
        #endregion 


        // public void OnOptionSelectClick()
        // {
        //     // Debug.Log("RadialMenuItems OnOptionSelectClick:");
        // }
        // public void OnOptionSelectHoverEnter()
        // {
        //     // Debug.Log("RadialMenuItems OnOptionSelectHoverEnter:");
        //     isPointerEnabled = true;
        //     isPointerAllowTeleport = false;
        // }
        // public void OnOptionSelectHoverExit()
        // {
        //     // Debug.Log("RadialMenuItems OnOptionSelectHoverExit:");
        //     isPointerEnabled = false;
        // }
        // public void OnOptionTeleportClick()
        // {
        //     // Debug.Log("RadialMenuItems OnOptionTeleportClick:");
        // }
        // public void OnOptionTeleportHoverEnter()
        // {
        //     // Debug.Log("RadialMenuItems OnOptionTeleportHoverEnter:");
        //     isPointerEnabled = true;
        //     isPointerAllowTeleport = true;
        // }
        // public void OnOptionTeleportHoverExit()
        // {
        //     // Debug.Log("RadialMenuItems OnOptionSelectHoverExit:");
        //     isPointerEnabled = false;
        // }

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
            // if (isPointerEnabled)
            // {
            //     EnablePointer(isPointerAllowTeleport);
            // }
            // else
            // {
            //     DisablePointer();
            // }


            if (isMenuEnabled)
            {

                if (isPointerEnabled)
                {
                    EnablePointer(false);
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
        void OnEnable()
        {
            EventManager.StartListening(Constant.EVENT_NAME_MENU_DISABLE, HandleMenuDisable);
            EventManager.StartListening(Constant.EVENT_NAME_MENU_ENABLE, HandleMenuEnable);
        }

        void OnDisable()
        {
            EventManager.StopListening(Constant.EVENT_NAME_MENU_DISABLE, HandleMenuDisable);
            EventManager.StopListening(Constant.EVENT_NAME_MENU_ENABLE, HandleMenuEnable);
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
        void HandleMenuDisable(object sender)
        {
            Debug.Log("HandleMenuDisable");
            var menuPointer = GetRadialMenuButton(IndexPointer);
            menuPointer.ButtonIcon = iconTeleport;
            _radialMenu.RegenerateButtons();
        }
        void HandleMenuEnable(object sender)
        {

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

