using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class ForLoopStart : MonoBehaviour
    {
        public ForLoopEnd ForLoopEnd { get; set; }

        public int loopCounter = 2;
        private GameObject _counterAdd;
        private GameObject _counterMinus;
        private GameObject _defaultSideArea;
        private Vector3 _defaultSideAreaPosition;
        // private List<GameObject> sideAreas;
        private float increaseSideSize = 1f;
        private float increasePosZ = 0.5f;

        #region Public Methods
        public void SetSideAreaTo(int start, int end)
        {
            int count = end - start - 1;
            ResetSideArea();
            if (count <= 0) { return; }

            _defaultSideArea.SetActive(true);
            for (int i = 0; i < count; i++)
            {
                _defaultSideArea.transform.localScale = new Vector3(
                    _defaultSideArea.transform.localScale.x,
                    _defaultSideArea.transform.localScale.y,
                    _defaultSideArea.transform.localScale.z + increaseSideSize);
                _defaultSideArea.transform.localPosition = new Vector3(_defaultSideAreaPosition.x, _defaultSideAreaPosition.y, _defaultSideArea.transform.localPosition.z + increasePosZ);
            }
        }
        public void ResetSideArea()
        {
            _defaultSideArea.transform.localScale = new Vector3(
                    _defaultSideArea.transform.localScale.x,
                    _defaultSideArea.transform.localScale.y, 0);
            _defaultSideArea.transform.localPosition = new Vector3(_defaultSideAreaPosition.x, _defaultSideAreaPosition.y, 0.5f);
            _defaultSideArea.SetActive(false);
        }
        public void SetActive()
        {
            gameObject.SetActive(true);
        }
        public void SetInactive()
        {
            gameObject.SetActive(false);
        }
        public ForLoopEnd GetChildForLoopEnd()
        {
            var childEnd = transform.Find(Constant.NAME_PROGRAM_BLOCK_FORLOOPEND);
            if (childEnd == null) { return null; }

            if (childEnd.gameObject.GetComponent<ForLoopEnd>() == null)
            {
                return null;
            }

            this.ForLoopEnd = childEnd.gameObject.GetComponent<ForLoopEnd>();
            return this.ForLoopEnd;
        }
        public bool IsInZone()
        {
            // Debug.Log("HasZoneIndex: ZoneIndex = " + ZoneIndex);
            // return ZoneIndex != -1;
            var block = GetProgramBlock();
            if (block == null) { return false; }
            return block.IsInZone();
        }
        public ProgramBlock GetProgramBlock()
        {
            var block = gameObject.GetComponent<ProgramBlock>();
            return block;
        }
        public int GetZoneIndex()
        {
            var block = GetProgramBlock();
            if (block == null) { return -1; }

            return block.GetZoneIndex();
        }

        #endregion

        // Use this for initialization
        void Start()
        {
            this.ForLoopEnd = GetChildForLoopEnd();
            this.ForLoopEnd.SetInactive();

            _defaultSideArea = GetSideAreaGameObject();
            _defaultSideAreaPosition = _defaultSideArea.transform.localPosition;
            // sideAreas = new List<GameObject>();
            // sideAreas.Add(_defaultSideArea);

            _counterAdd = GetCounterAddGameObject();
            _counterAdd.GetComponent<ForLoopCounterAdd>().OnEnter += new ForLoopCounterAdd.CounterAddEventHandler(HandleOnCounterAddTriggerEnter);
            _counterAdd.GetComponent<ForLoopCounterAdd>().OnExit += new ForLoopCounterAdd.CounterAddEventHandler(HandleOnCounterAddTriggerExit);

            _counterMinus = GetCounterMinusGameObject();
            _counterMinus.GetComponent<ForLoopCounterMinus>().OnEnter += new ForLoopCounterMinus.CounterMinusEventHandler(HandleOnCounterMinusTriggerEnter);
            _counterMinus.GetComponent<ForLoopCounterMinus>().OnExit += new ForLoopCounterMinus.CounterMinusEventHandler(HandleOnCounterMinusTriggerExit);

            GetCounterNumberTextMesh().text = Convert.ToString(loopCounter);
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Private Methods

        void HandleOnCounterAddTriggerEnter(object sender, CounterAddEventArgs args)
        {
            //Debug.Log("HandleOnCounterAddTriggerEnter");
            GetCounterNumberTextMesh().text = Convert.ToString(++loopCounter);
        }
        void HandleOnCounterAddTriggerExit(object sender, CounterAddEventArgs args)
        {
            //Debug.Log("HandleOnCounterAddTriggerExit");
        }
        void HandleOnCounterMinusTriggerEnter(object sender, CounterMinusEventArgs args)
        {
            //Debug.Log("HandleOnCounterMinusTriggerEnter");
            if (loopCounter != 1)
            {
                GetCounterNumberTextMesh().text = Convert.ToString(--loopCounter);
            }
        }
        void HandleOnCounterMinusTriggerExit(object sender, CounterMinusEventArgs args)
        {
            //Debug.Log("HandleOnCounterMinusTriggerExit");
        }

        #endregion

        #region Private Get Methods

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

        #endregion
    }
}
