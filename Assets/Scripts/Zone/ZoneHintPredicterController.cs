using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public struct ZoneHintPredicterEventArgs
    {
        public GameObject CollidedObject;
    }
    public class ZoneHintPredicterController : MonoBehaviour
    {
        public delegate void ZoneHintPredicterEventHandler(object sender, ZoneHintPredicterEventArgs args);
        public event ZoneHintPredicterEventHandler OnPredicterTriggerEnter;
        public event ZoneHintPredicterEventHandler OnPredicterTriggerExit;

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
            OnPredicterTriggerEnter(this, new ZoneHintPredicterEventArgs { CollidedObject = other.gameObject });
        }
        void OnTriggerExit(Collider other)
        {
            OnPredicterTriggerExit(this, new ZoneHintPredicterEventArgs { CollidedObject = other.gameObject });
        }
    }

}
