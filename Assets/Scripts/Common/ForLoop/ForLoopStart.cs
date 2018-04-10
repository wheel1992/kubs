using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class ForLoopStart : MonoBehaviour
    {
        [SerializeField] private GameObject forLoopEndPrefab;
        public ForLoopEnd ForLoopEnd { get; set; }
        [HideInInspector]
        public int loopCounter = 2;
        private GameObject _dummyForLoopEnd;
        private GameObject _counterAdd;
        private GameObject _counterMinus;
        private GameObject _defaultSideArea;
        private Vector3 _defaultSideAreaPosition;
        private float increaseSideSize = 1f;
        private float increasePosZ = 0.5f;

        #region Public Methods
        public void HideDummyForLoopEnd()
        {
            GetDummyForLoopEnd().SetActive(false);
        }
        public void ShowDummyForLoopEnd()
        {
            GetDummyForLoopEnd().SetActive(true);
        }
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
        public ForLoopEnd CreateForLoopEnd()
        {
            ForLoopEnd = Instantiate(forLoopEndPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<ForLoopEnd>();
            ForLoopEnd.ForLoopStart = this;
            return ForLoopEnd;
        }
        public void DeleteForLoopEnd()
        {
            Destroy(ForLoopEnd);
        }
        public bool IsInZone()
        {
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
            GetProgramBlock().GetVRTKInteractableObject().InteractableObjectGrabbed += new InteractableObjectEventHandler(HandleOnGrabbed);
            GetProgramBlock().GetVRTKInteractableObject().InteractableObjectUngrabbed += new InteractableObjectEventHandler(HandleOnUngrabbed);
            GetProgramBlock().GetVRTKInteractableObject().InteractableObjectSnappedToDropZone += new InteractableObjectEventHandler(HandleOnSnappedToDropZone);

            _defaultSideArea = GetSideAreaGameObject();
            _defaultSideAreaPosition = _defaultSideArea.transform.localPosition;

            _counterAdd = GetCounterAddGameObject();
            _counterAdd.GetComponent<ForLoopCounterAdd>().OnEnter += new ForLoopCounterAdd.CounterAddEventHandler(HandleOnCounterAddTriggerEnter);
            _counterAdd.GetComponent<ForLoopCounterAdd>().OnExit += new ForLoopCounterAdd.CounterAddEventHandler(HandleOnCounterAddTriggerExit);

            _counterMinus = GetCounterMinusGameObject();
            _counterMinus.GetComponent<ForLoopCounterMinus>().OnEnter += new ForLoopCounterMinus.CounterMinusEventHandler(HandleOnCounterMinusTriggerEnter);
            _counterMinus.GetComponent<ForLoopCounterMinus>().OnExit += new ForLoopCounterMinus.CounterMinusEventHandler(HandleOnCounterMinusTriggerExit);

            GetCounterNumberTextMesh().text = Convert.ToString(loopCounter);

            ResetSideArea();
            ShowDummyForLoopEnd();

            if (GetProgramBlock().IsInSnapDropZoneClone())
            {
                ExpandCollider();
                DisableCounter();
            }
            else
            {
                ResetCollider();
            }

        }

        // Update is called once per frame
        void Update()
        {
        }

        #region Private Methods
        private void HandleOnGrabbed(object sender, InteractableObjectEventArgs args)
        {
            if (sender is VRTK_InteractableObject)
            {
                var interactableObject = (VRTK_InteractableObject)sender;
                var forStartBlock = interactableObject.gameObject.GetComponent<ForLoopStart>();
                if (forStartBlock != null)
                {
                    forStartBlock.DisableCounter();
                }
            }
        }
        private void HandleOnUngrabbed(object sender, InteractableObjectEventArgs args)
        {
            Debug.Log("HandleOnUngrabbed:");
            if (sender is VRTK_InteractableObject)
            {
                EventManager.TriggerEvent(Constant.EVENT_NAME_FOR_LOOP_START_UNGRAB, sender);
            }
        }
        private void HandleOnSnappedToDropZone(object sender, InteractableObjectEventArgs args)
        {
            Debug.Log("HandleOnSnappedToDropZone:");
            if (sender is VRTK_InteractableObject)
            {
                var interactableObject = (VRTK_InteractableObject)sender;
                var forStartBlock = interactableObject.GetComponent<ForLoopStart>();
                // Ungrabbed and dropped not within the Zone (aka outside)
                if (interactableObject.IsInSnapDropZone() && !forStartBlock.GetProgramBlock().IsInSnapDropZoneClone())
                {
                    forStartBlock.HideDummyForLoopEnd();
                    forStartBlock.EnableCounter();
                }
            }
        }
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
        void EnableCounter()
        {
            _counterAdd.GetComponent<BoxCollider>().enabled = true;
            _counterMinus.GetComponent<BoxCollider>().enabled = true;
        }
        void DisableCounter()
        {
            _counterAdd.GetComponent<BoxCollider>().enabled = false;
            _counterMinus.GetComponent<BoxCollider>().enabled = false;
        }
        void ExpandCollider()
        {
            var collider = GetProgramBlock().GetBoxCollider();
            if (collider == null) { return; }
            collider.center = new Vector3(0, 0, 0.75f);
            collider.size = new Vector3(2, 1, 2.5f);
        }
        void ResetCollider()
        {
            var collider = GetProgramBlock().GetBoxCollider();
            if (collider == null) { return; }
            collider.center = new Vector3(0, 0, 0);
            collider.size = new Vector3(1f, 1f, 1f);
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
        GameObject GetDummyForLoopEnd()
        {
            return transform.Find("ForEnd").gameObject;
        }
        #endregion
    }
}
