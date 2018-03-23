using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kubs
{
    public class SweepChildBlock : MonoBehaviour
    {

        public delegate void TriggerEventHandler(Collider other);
        public event TriggerEventHandler OnEnter;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (OnEnter != null)
            {
                OnEnter(other);
            }
        }

        public void PauseTrigger()
        {
            GetBoxCollider().enabled = false;
        }

        public void StartTrigger()
        {
            GetBoxCollider().enabled = true;
        }

        private BoxCollider GetBoxCollider()
        {
            return gameObject.GetComponent<BoxCollider>();
        }
    }
}

