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

        void OnTriggerEnter(Collider other)
        {
            if (other != null)
            {
                if (OnPredicterTriggerEnter != null)
                {
                    OnPredicterTriggerEnter(this, new ZoneHintPredicterEventArgs { CollidedObject = other.gameObject });
                }
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other != null)
            {
                if (OnPredicterTriggerExit != null)
                {
                    OnPredicterTriggerExit(this, new ZoneHintPredicterEventArgs { CollidedObject = other.gameObject });
                }
            }
        }
    }

}
