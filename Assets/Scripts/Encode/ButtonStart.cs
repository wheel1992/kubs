using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

namespace Kubs
{
    //public struct ButtonStartEventArgs
    //{
    //    public int Index;
    //    public bool IsAttachedMove;
    //}
    public class ButtonStart : MonoBehaviour
    {
        public AudioClip audioClipButtonPressed;
        private AudioSource _audioSourceButtonPressed;
        private VRTK_Button_UnityEvents buttonEvents;
        private Material _defaultMaterial;

        private bool _isAnimating = false;

        private GameObject _zonesObject;
        private ZoneGroupController _zoneGroupController;
        private ZoneMovementController _zoneMovementController;

        private bool HasTouchedByController = false;
        private bool IsTouched = false;
        // Use this for initialization
        void Start()
        {
            // buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
            // if (buttonEvents == null)
            // {
            //     buttonEvents = gameObject.AddComponent<VRTK_Button_UnityEvents>();
            // }
            // buttonEvents.OnPushed.AddListener(HandlePush);
            //_defaultMaterial = GetCurrentMaterial();

            InitAudioClips();

            _zonesObject = GetZonesGameObject();
            _zoneGroupController = GetZoneGroupController();
            _zoneMovementController = _zonesObject.GetComponent<ZoneMovementController>();
            _zoneMovementController.OnCompleted += Decode;

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

        //Testing Method
        public void buttonPressed()
        {
            Collider other = new Collider();
            OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            //Debug.Log("OnTriggerExit");
            //gameObject.GetComponent<Renderer>().material = _defaultMaterial;

            // Run();
        }
        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("OnTriggerEnter");
            //ChangeColor();
            // Debug.Log("OnTriggerEnter: " + other);
            // // if (!_isAnimating)
            // // {
            // //     Run();
            // // }
            // StartCoroutine("Depress");
        }
        private void HandleOnTouched(object sender, InteractableObjectEventArgs args)
        {
            // Debug.Log("HandleOnTouched: " + args.interactingObject);
            if (args.interactingObject.name.CompareTo("RightController") == 0 || args.interactingObject.name.CompareTo("LeftController") == 0)
            {
                if (!HasTouchedByController)
                {
                    HasTouchedByController = true;
                    _audioSourceButtonPressed.Play();
                    StartCoroutine(Depress());
                }
            }
        }
        private void HandleOnUntouched(object sender, InteractableObjectEventArgs args)
        {
            // Debug.Log("HandleOnUntouched: " + args.interactingObject);
            if (HasTouchedByController)
            {
                HasTouchedByController = false;
                Run();
            }
        }

        //private void HandlePush(object sender, Control3DEventArgs e)
        //{
        //    VRTK_Logger.Info("Pushed");
        //    Run();
        //}
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
        private void InitAudioClips()
        {
            _audioSourceButtonPressed = gameObject.AddComponent<AudioSource>();
            _audioSourceButtonPressed.clip = audioClipButtonPressed;
            _audioSourceButtonPressed.loop = false;
            _audioSourceButtonPressed.playOnAwake = false;
            _audioSourceButtonPressed.volume = 1.0f;
        }

        private void Run()
        {
            // var listBlocks = GetSnapDropZoneBlockGroup().GetListOfSnappedProgramBlocks();
            // //Debug.Log("HandlePush: list blocks count = " + listBlocks.Count);

            GameObject.Find("Character").GetComponent<Character>().Reset();
            // _zoneMovementController.MoveBlockChain();
            Decode();
        }

        private void Decode()
        {
            Debug.Log("Decode");
            // _zonesObject.SetActive(false);

            var listBlocks = _zoneGroupController.CompileProgramBlocks();
            GetDecoder().Decode(listBlocks);
        }

        private void ChangeColor()
        {
            // gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        private Material GetCurrentMaterial()
        {
            return gameObject.GetComponent<Renderer>().material;
        }

        /*
        private GameObject GetButtonStart() {
            return gameObject.transform.GetChild(0);
        }
        */

        private IEnumerator Depress()
        {
            if (_isAnimating)
            {
                //Debug.Log("_isAnimating");
                yield break;
            }

            _isAnimating = true;

            var startScale = transform.localScale;
            var endScale = startScale;
            endScale.y /= 2;

            var incrementor = 0.5f;

            // Debug.Log(Vector3.SqrMagnitude(transform.localScale - endScale));
            // Debug.Log("+");

            while (Vector3.SqrMagnitude(transform.localScale - endScale) > 0.00000001)
            {
                incrementor += Time.deltaTime;
                var currentScale = Vector3.Lerp(startScale, endScale, incrementor);
                transform.localScale = currentScale;
                //Debug.Log(1);
                yield return null;
            }

            endScale = startScale;
            startScale = transform.localScale;
            incrementor = 0.5f;

            while (Vector3.SqrMagnitude(transform.localScale - endScale) > 0.00000001)
            {
                incrementor += Time.deltaTime;
                var currentScale = Vector3.Lerp(startScale, endScale, incrementor);
                transform.localScale = currentScale;
                //Debug.Log(2);
                yield return null;
            }

            //Debug.Log(3);

            _isAnimating = false;
            yield break;
        }

        private Decoder GetDecoder()
        {
            return GameObject.FindGameObjectWithTag(Constant.TAG_LEVEL).GetComponent<Decoder>();
        }
        public VRTK_InteractableObject GetVRTKInteractableObject()
        {
            return gameObject.GetComponent<VRTK_InteractableObject>();
        }
        private Behaviour GetHalo()
        {
            return (Behaviour)gameObject.GetComponent("Halo");
        }
        private ZoneGroupController GetZoneGroupController()
        {
            return GameObject.Find(Constant.NAME_ZONES).GetComponent<ZoneGroupController>();
        }

        GameObject GetZonesGameObject()
        {
            return GameObject.Find("Zones");
        }

    }
}
