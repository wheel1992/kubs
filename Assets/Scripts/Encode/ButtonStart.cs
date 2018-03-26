﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

namespace Kubs
{
    public class ButtonStart : MonoBehaviour
    {

        private VRTK_Button_UnityEvents buttonEvents;

        // Use this for initialization
        void Start()
        {
            buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
            if (buttonEvents == null)
            {
                buttonEvents = gameObject.AddComponent<VRTK_Button_UnityEvents>();
            }
            buttonEvents.OnPushed.AddListener(HandlePush);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void HandlePush(object sender, Control3DEventArgs e)
        {
            VRTK_Logger.Info("Pushed");
        }
    }
}
