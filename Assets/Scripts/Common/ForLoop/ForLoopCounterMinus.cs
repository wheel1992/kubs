﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public struct CounterMinusEventArgs
    {
        public GameObject CollidedObject;
    }
    public class ForLoopCounterMinus : MonoBehaviour
    {
        public delegate void CounterMinusEventHandler(object sender, CounterMinusEventArgs args);
        public event CounterMinusEventHandler OnEnter;
        public event CounterMinusEventHandler OnExit;
        private bool IsTriggered = false;
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
        void OnTriggerEnter(Collider other)
        {
            if (!IsTriggered)
            {
                IsTriggered = true;
                OnEnter(this, new CounterMinusEventArgs { CollidedObject = other.gameObject });
            }
        }
        void OnTriggerExit(Collider other)
        {
            IsTriggered = false;
            OnExit(this, new CounterMinusEventArgs { CollidedObject = other.gameObject });
        }
    }
}

