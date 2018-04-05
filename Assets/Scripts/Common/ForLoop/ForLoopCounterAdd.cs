using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public struct CounterAddEventArgs
    {
        public GameObject CollidedObject;
    }
    public class ForLoopCounterAdd : MonoBehaviour
    {
        public delegate void CounterAddEventHandler(object sender, CounterAddEventArgs args);
        public event CounterAddEventHandler OnEnter;
        public event CounterAddEventHandler OnExit;
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
            if (!IsController(other.gameObject)) return;
            if (!IsTriggered)
            {
                IsTriggered = true;
                OnEnter(this, new CounterAddEventArgs { CollidedObject = other.gameObject });
            }
        }
        void OnTriggerExit(Collider other)
        {
            IsTriggered = false;
            OnExit(this, new CounterAddEventArgs { CollidedObject = other.gameObject });
        }
        bool IsController(GameObject obj)
        {
            return obj.name.CompareTo("Head") == 0 || obj.name.CompareTo("SideA") == 0 || obj.name.CompareTo("SideB") == 0;
        }
    }

}

