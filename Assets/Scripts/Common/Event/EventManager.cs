using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace Kubs
{
    public class EventManager : MonoBehaviour
    {

        private Dictionary<string, UnityAction<object>> eventDictionary;

        private static EventManager eventManager;

        public static EventManager instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();
                    }
                }

                return eventManager;
            }
        }

        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, UnityAction<object>>();
            }
        }

        public static void StartListening(string eventName, UnityAction<object> listener)
        {
            UnityAction<object> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
            }
            else
            {
                thisEvent += listener;
                instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction<object> listener)
        {
            if (eventManager == null) return;
            UnityAction<object> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
            }
        }

        public static void TriggerEvent(string eventName, object h)
        {
            UnityAction<object> thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(h);
            }
        }
    }
}
