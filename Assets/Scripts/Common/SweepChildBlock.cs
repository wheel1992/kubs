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
            //Debug.Log("SweepChildBlock OnTriggerEnter: collider at layer = " + other.gameObject.layer);
            if (OnEnter != null)
            {
                OnEnter(other);
            }
        }
    }
}

