using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

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
        private bool IsTouched = false;
        // Use this for initialization
        void Awake()
        {
            DisableHalo();
        }
        void Start()
        {
            GetVRTKInteractableObject().InteractableObjectTouched += new InteractableObjectEventHandler(HandleOnTouched);
            GetVRTKInteractableObject().InteractableObjectUntouched += new InteractableObjectEventHandler(HandleOnUntouched);
            DisableHalo();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsTouched)
            {
                EnableHalo();
            }
            else
            {
                DisableHalo();
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (other == null) { return; }
            if (!IsController(other.gameObject)) return;
            if (!IsTriggered)
            {
                IsTriggered = true;
                OnEnter(this, new CounterAddEventArgs { CollidedObject = other.gameObject });
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other == null) { return; }
            IsTriggered = false;
            OnExit(this, new CounterAddEventArgs { CollidedObject = other.gameObject });
        }
        private void HandleOnTouched(object sender, InteractableObjectEventArgs args)
        {
            if (!IsController(args.interactingObject)) return;
            IsTouched = true;
        }
        private void HandleOnUntouched(object sender, InteractableObjectEventArgs args)
        {
            if (!IsController(args.interactingObject)) return;
            IsTouched = false;
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
            return obj.name.CompareTo("Head") == 0 || obj.name.CompareTo("SideA") == 0 || obj.name.CompareTo("SideB") == 0 || 
            obj.name.CompareTo("RightController") == 0 || obj.name.CompareTo("LeftController") == 0;
        }

    }

}

