using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

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
            GetVRTKInteractableObject().InteractableObjectTouched += new InteractableObjectEventHandler(HandleOnTouched);
            GetVRTKInteractableObject().InteractableObjectUntouched += new InteractableObjectEventHandler(HandleOnUntouched);
            DisableHalo();
        }

        // Update is called once per frame
        void Update()
        {
        }
        void OnTriggerEnter(Collider other)
        {
            if (other == null) { return; }
            if (!IsController(other.gameObject)) return;
            if (!IsTriggered)
            {
                IsTriggered = true;
                OnEnter(this, new CounterMinusEventArgs { CollidedObject = other.gameObject });
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other == null) { return; }
            IsTriggered = false;
            OnExit(this, new CounterMinusEventArgs { CollidedObject = other.gameObject });
        }
        private void HandleOnTouched(object sender, InteractableObjectEventArgs args)
        {
            EnableHalo();
        }
        private void HandleOnUntouched(object sender, InteractableObjectEventArgs args)
        {
            DisableHalo();
        }
        void DisableHalo()
        {
            var halo = GetHalo();
            halo.enabled = false;
        }
        void EnableHalo()
        {
            var halo = GetHalo();
            halo.enabled = true;
        }
        private Behaviour GetHalo()
        {
            return (Behaviour)gameObject.GetComponent("Halo");
        }
        public VRTK_InteractableObject GetVRTKInteractableObject()
        {
            return gameObject.GetComponent<VRTK_InteractableObject>();
        }
        bool IsController(GameObject obj)
        {
            return obj.name.CompareTo("Head") == 0 || obj.name.CompareTo("SideA") == 0 || obj.name.CompareTo("SideB") == 0;
        }
    }
}

