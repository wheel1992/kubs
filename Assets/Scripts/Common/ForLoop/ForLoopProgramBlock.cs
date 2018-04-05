using System;
using System.Collections;
using System.Collections.Generic;
using Kubs;
using UnityEngine;

namespace Kubs
{
    public class ForLoopProgramBlock : MonoBehaviour
    {
        [SerializeField] private GameObject sideAreaPrefab;

        public int ForLoopStartIndex { get; set; }
        public int ForLoopEndIndex { get; set; }
        private GameObject _counterAdd;
        private GameObject _counterMinus;
        private GameObject _defaultSideArea;
        private Vector3 _defaultSideAreaPosition;
        private List<GameObject> sideAreas;

        public int loopCounter = 2;

        public void UpdateUi()
        {
            var numberBlocks = ForLoopEndIndex - ForLoopStartIndex - 1;
            Debug.Log("UpdateUi: sideAreas = " + sideAreas.Count + " numberBlocks = " + numberBlocks);
            if (numberBlocks < 0) return;
            if (sideAreas.Count != numberBlocks)
            {
                if (sideAreas.Count > numberBlocks)
                {
                    Remove(numberBlocks, sideAreas.Count - numberBlocks);
                    sideAreas.RemoveRange(numberBlocks, sideAreas.Count - numberBlocks);
                    Reposition();
                }
                else
                {
                    // current sideAreas count is less than NumberBlocks
                    Add(numberBlocks, numberBlocks - sideAreas.Count);
                    Reposition();
                }
            }
        }
        void Start()
        {
            _defaultSideArea = GetSideAreaGameObject();
            _defaultSideAreaPosition = _defaultSideArea.transform.localPosition;
            sideAreas = new List<GameObject>();
            sideAreas.Add(_defaultSideArea);

            _counterAdd = GetCounterAddGameObject();
            _counterAdd.GetComponent<ForLoopCounterAdd>().OnEnter += new ForLoopCounterAdd.CounterAddEventHandler(HandleOnCounterAddTriggerEnter);
            _counterAdd.GetComponent<ForLoopCounterAdd>().OnExit += new ForLoopCounterAdd.CounterAddEventHandler(HandleOnCounterAddTriggerExit);

            _counterMinus = GetCounterMinusGameObject();
            _counterMinus.GetComponent<ForLoopCounterMinus>().OnEnter += new ForLoopCounterMinus.CounterMinusEventHandler(HandleOnCounterMinusTriggerEnter);
            _counterMinus.GetComponent<ForLoopCounterMinus>().OnExit += new ForLoopCounterMinus.CounterMinusEventHandler(HandleOnCounterMinusTriggerExit);

            GetCounterNumberTextMesh().text = Convert.ToString(loopCounter);
        }
        // void Update()
        // {
        // }
        void Reposition()
        {
            for (int i = 0; i < sideAreas.Count; i++)
            {
                var side = sideAreas[i];
                side.transform.position = new Vector3(_defaultSideAreaPosition.x, _defaultSideAreaPosition.y, _defaultSideAreaPosition.z + (1f * i));
                Debug.Log("Reposition: " + i + " at " + side.transform.position);
            }
        }
        void Remove(int start, int count)
        {
            Debug.Log("Remove: start = " + start + " count = " + count);
            for (int i = start; i < count; i++)
            {
                Destroy(sideAreas[i]);
            }
        }
        void Add(int start, int count)
        {
            Debug.Log("Add: start = " + start + " count = " + count);
            for (int i = start; i < count; i++)
            {
                sideAreas.Insert(start, CreateSideArea(new Vector3(0, 0, 0)));
            }
        }
        void HandleOnCounterAddTriggerEnter(object sender, CounterAddEventArgs args)
        {
            Debug.Log("HandleOnCounterAddTriggerEnter");
            GetCounterNumberTextMesh().text = Convert.ToString(++loopCounter);
        }
        void HandleOnCounterAddTriggerExit(object sender, CounterAddEventArgs args)
        {
            Debug.Log("HandleOnCounterAddTriggerExit");
        }
        void HandleOnCounterMinusTriggerEnter(object sender, CounterMinusEventArgs args)
        {
            Debug.Log("HandleOnCounterMinusTriggerEnter");
            if (loopCounter != 1) {
                 GetCounterNumberTextMesh().text = Convert.ToString(--loopCounter);
            }
        }
        void HandleOnCounterMinusTriggerExit(object sender, CounterMinusEventArgs args)
        {
            Debug.Log("HandleOnCounterMinusTriggerExit");
        }
        GameObject CreateSideArea(Vector3 pos)
        {
            return Instantiate(sideAreaPrefab, pos, Quaternion.identity);
        }
        GameObject GetCounterAreaGameObject()
        {
            return transform.Find("ForStartCounterArea").gameObject;
        }
        GameObject GetCounterAddGameObject()
        {
            return GetCounterAreaGameObject().transform.Find("CounterAddArea").gameObject;
        }
        GameObject GetCounterMinusGameObject()
        {
            return GetCounterAreaGameObject().transform.Find("CounterMinusArea").gameObject;
        }
        GameObject GetCounterNumberAreaGameObject()
        {
            return GetCounterAreaGameObject().transform.Find("CounterNumberArea").gameObject;
        }
        GameObject GetSideAreaGameObject()
        {
            return transform.Find("SideArea").gameObject;
        }
        TextMesh GetCounterNumberTextMesh()
        {
            return GetCounterNumberAreaGameObject().transform.Find("CounterNumberText").gameObject.GetComponent<TextMesh>();
        }
    }

}
