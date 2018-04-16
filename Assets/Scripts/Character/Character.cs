using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Kubs
{
    public class Character : MonoBehaviour
    {
        public delegate void CharacterEventHandler();
        public event CharacterEventHandler OnReset;

        public StageTriggerManager stageTriggerManager;
        public TutorialManager tutorialManager;

        // Components
        private Animator _animator;
        private Rigidbody _rigidbody;

        // Animation
        private bool _isAnimating;
        private Queue<ProgramBlockType> _queue = new Queue<ProgramBlockType>();
        private ProgramBlockType _type;

        // Position
        private Vector3 startPos;
        private Vector3 endPos;
        private Vector3 _originalPos;
        private float trajectoryHeight;

        // Rotation
        private Quaternion startRot;
        private Quaternion endRot;
        private Quaternion _originalRot;

        // Flags and miscellaneous
        private bool _isDebug = false;
        private bool _resetFlag = false;
        public float _scale;
        public bool showPopup;

        // Audio
        public AudioClip audioClipWalk;
        public AudioClip audioClipChewFood;
        public AudioClip audioClipJump;
        private AudioSource _audioSourceWalk;
        private AudioSource _audioSourceChewFood;
        private AudioSource _audioSourceJump;

        private GameObject _zonesObject;

        public HintPrefabs HintProgramBlockPrefabs;
        private bool IsShowingPopup = false;

        [Serializable]
        public struct HintPrefabs
        {
            public GameObject forwardBlockPrefab;
            public GameObject JumpBlockPrefab;
            public GameObject RotateLeftBlockPrefab;
            public GameObject RotateRightBlockPrefab;
        }

        GameObject GetCurrentChildPopup()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag(Constant.TAG_BLOCK_PROGRAM))
                {
                    return transform.GetChild(i).gameObject;
                }
            }
            return null;
        }
        void ShowPopup(GameObject popupPrefab)
        {
            if (popupPrefab == null) { return; }

            DestroyPopupIfAny();

            var popupObject = Instantiate(popupPrefab, new Vector3(transform.position.x, transform.position.y + (_scale * 1.2f), transform.position.z), Quaternion.identity);
            popupObject.transform.SetParent(transform);
            popupObject.GetComponent<Rigidbody>().isKinematic = true;
            popupObject.GetComponent<VRTK_InteractableObject>().enabled = false;
            popupObject.GetComponent<BoxCollider>().enabled = false;

            IsShowingPopup = true;
        }
        void DestroyPopupIfAny()
        {
            var childObj = GetCurrentChildPopup();
            if (childObj != null)
            {
                Destroy(childObj);
                IsShowingPopup = false;
            }
        }

        // Use this for initialization
        void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _originalPos = transform.localPosition;
            _originalRot = transform.localRotation;
            _scale = transform.lossyScale.x;

            InitAudioClips();

            if (_isDebug)
            {
                // Test blockchain movement
                var buttonStart = GameObject.Find("ButtonStart_New");
                if (buttonStart != null)
                {
                    buttonStart.GetComponent<ButtonStart>().buttonPressed();
                }

                // Start with forward
                Invoke("Forward", 1);

                // Set 1
                Invoke("Forward", 5);
                Invoke("RotateLeft", 5);
                Invoke("Forward", 5);

                var delay = 11;
                Invoke("Forward", delay);
                Invoke("RotateLeft", delay);
                Invoke("Forward", delay);
                Invoke("Jump", delay);

                delay = 18;
                for (int i = 1; i <= 4; i++)
                {
                    Invoke("Forward", delay);
                    Invoke("RotateLeft", delay);
                    Invoke("Forward", delay);
                    Invoke("Jump", delay);

                    Invoke("Forward", delay);
                }

                // Set 2
                /*
                Invoke("RotateLeft", 1);
                Invoke("Jump", 1);
                Invoke("Jump", 1);
                */
            }

            _zonesObject = GetZonesGameObject();

            if (gameObject.tag != "UICharacter")
            {
                EventManager.TriggerEvent(Constant.EVENT_NAME_CHARACTER_DID_START, this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (IsShowingPopup)
            {
                var child = GetCurrentChildPopup();
                // var headRot = VRTK_DeviceFinder.HeadsetTransform().rotation;
                // var targetRot = new Vector3(headRot.x, headRot.y, -90f);
                // child.transform.LookAt(targetRot);

                // Vector3 direction = VRTK_DeviceFinder.HeadsetCamera().position - child.transform.position;
                // direction.z -= 0;
                // child.transform.rotation = Quaternion.Slerp(child.transform.rotation, Quaternion.LookRotation(direction), 100f * Time.deltaTime);
                Debug.Log("LookAtRotation");
                child.transform.LookAt(VRTK_DeviceFinder.HeadsetCamera());
                child.transform.Rotate(0f, -90f, -90f);


                // child.transform.rotation = Quaternion.Euler();
            }

            if (_isAnimating)
            {
                // No action
            }
            else if (IsFalling())
            {
                // No action
            }
            else if (_queue.Count > 0)
            {
                switch (_queue.Dequeue())
                {
                    case ProgramBlockType.Forward:
                        Forward();
                        break;
                    case ProgramBlockType.Jump:
                        Jump();
                        break;
                    case ProgramBlockType.RotateLeft:
                        RotateLeft();
                        break;
                    case ProgramBlockType.RotateRight:
                        RotateRight();
                        break;
                    default:
                        break;
                }
            }
            else if (_resetFlag)
            {
                _resetFlag = false;
                Invoke("Reset", 2f);
            }
        }

        void OnEnable()
        {
            EventManager.StartListening(Constant.EVENT_NAME_GAME_AREA_DID_SCALE, UpdateScale);
        }

        void OnDisable()
        {
            EventManager.StopListening(Constant.EVENT_NAME_GAME_AREA_DID_SCALE, UpdateScale);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Grass")
            {
                Stop();
                _queue.Clear();

                Set(Animations.Die2);
                StopCoroutine("UpdatePosition");
            }
            else if (other.gameObject.tag == "Hole")
            {
                Stop();
                _queue.Clear();

                SetResetFlag();
            }
            else if (other.gameObject.tag == "Collectable")
            {
                _audioSourceChewFood.Play();

                other.gameObject.SetActive(false);

                var collectableBlock = other.gameObject.GetComponent<CollectableBlock>();
                if (collectableBlock != null)
                {
                    if (tutorialManager != null)
                    {
                        tutorialManager.ShowStage(collectableBlock.nextStage, false);

                        var tutorialBlock = collectableBlock.GetComponent<TutorialBlock>();
                        if (tutorialBlock == null || tutorialBlock.stage != tutorialManager.lastStage)
                        {
                            SetResetFlag();
                        }
                    }

                    Set(Animations.Victory);
                }
            }
        }

        public bool Forward()
        {
            SetResetFlag();

            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.Forward);
                return false;
            }

            if (showPopup)
            {
                ShowPopup(HintProgramBlockPrefabs.forwardBlockPrefab);
            }

            startPos = transform.position;
            endPos = transform.position + transform.forward * _scale;
            trajectoryHeight = 0;

            Set(Animations.Move);
            _type = ProgramBlockType.Forward;

            _audioSourceWalk.Play();

            StartCoroutine("UpdatePosition");
            return true;
        }

        public bool Jump()
        {
            SetResetFlag();

            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.Jump);
                return false;
            }

            if (showPopup)
            {
                ShowPopup(HintProgramBlockPrefabs.JumpBlockPrefab);
            }

            startPos = transform.position;

            if (IsBlocked())
            {
                // Jump up
                endPos = transform.position + (transform.forward + transform.up) * _scale;
                trajectoryHeight = 1.2f * _scale;
            }
            else
            {
                // Jump over
                endPos = transform.position + (transform.forward + transform.forward) * _scale;
                trajectoryHeight = 0.5f * _scale;
            }

            Set(Animations.Jump);
            _type = ProgramBlockType.Jump;

            _audioSourceJump.Play();

            StartCoroutine("UpdatePosition");
            return true;
        }

        public bool RotateLeft()
        {
            SetResetFlag();

            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.RotateLeft);
                return false;
            }

            if (showPopup)
            {
                ShowPopup(HintProgramBlockPrefabs.RotateLeftBlockPrefab);
            }

            startRot = transform.rotation;
            endRot = Quaternion.LookRotation(-transform.right);

            Set(Animations.Move_L);
            _type = ProgramBlockType.RotateLeft;

            StartCoroutine("UpdateRotation");
            return true;
        }

        public bool RotateRight()
        {
            SetResetFlag();

            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.RotateRight);
                return false;
            }

            if (showPopup)
            {
                ShowPopup(HintProgramBlockPrefabs.RotateRightBlockPrefab);
            }

            startRot = transform.rotation;
            endRot = Quaternion.LookRotation(transform.right);

            Set(Animations.Move_R);
            _type = ProgramBlockType.RotateRight;

            StartCoroutine("UpdateRotation");
            return true;
        }

        private void DebugLog(object s)
        {
            if (_isDebug)
            {
                Debug.Log(s);
            }
        }

        private bool IsBlocked()
        {
            var maxDistance = _scale;
            var position = transform.position;
            position.y += 0.5f * _scale;
            return Physics.Raycast(position, transform.forward, maxDistance);
        }

        private bool IsFalling()
        {
            return _rigidbody.velocity.y < -0.1;
        }

        private void InitAudioClips()
        {
            _audioSourceWalk = gameObject.AddComponent<AudioSource>();
            _audioSourceWalk.clip = audioClipWalk;
            _audioSourceWalk.loop = false;
            _audioSourceWalk.playOnAwake = false;
            _audioSourceWalk.volume = 1.0f;

            _audioSourceChewFood = gameObject.AddComponent<AudioSource>();
            _audioSourceChewFood.clip = audioClipChewFood;
            _audioSourceChewFood.loop = false;
            _audioSourceChewFood.playOnAwake = false;
            _audioSourceChewFood.volume = 1.0f;

            _audioSourceJump = gameObject.AddComponent<AudioSource>();
            _audioSourceJump.clip = audioClipJump;
            _audioSourceJump.loop = false;
            _audioSourceJump.playOnAwake = false;
            _audioSourceJump.volume = 0.8f;
        }

        public void Reset()
        {
            DebugLog("Reset");
            _resetFlag = false;

            transform.localPosition = _originalPos;
            transform.localRotation = _originalRot;

            Set(Animations.Idle);
            Stop();

            if (OnReset != null)
            {
                OnReset();
            }

            //if (_zonesObject != null)
            //{
            //    _zonesObject.SetActive(true);
            //    // _zonesObject.GetComponent<ZoneMovementController>().MoveBlockChain();
            //}

            stageTriggerManager.Reset();
        }

        private void SetResetFlag()
        {
            DebugLog("SetResetFlag");
            _resetFlag = true;
        }

        private void Stop()
        {
            StopCoroutine("UpdatePosition");
            StopCoroutine("UpdateRotation");

            _isAnimating = false;
            _queue.Clear();
            _rigidbody.velocity = Vector3.zero;
        }

        private Animations GetAnimation()
        {
            return (Animations)_animator.GetInteger("animation");
        }

        private void Set(Animations animation)
        {
            _animator.SetInteger("animation", (int)animation);
        }

        private IEnumerator UpdatePosition()
        {
            _isAnimating = true;

            var incrementor = 0f;

            while (Vector3.SqrMagnitude(transform.position - endPos) > 0.00000001)
            {
                // https://answers.unity.com/questions/8318/throwing-object-with-acceleration-equationscript.html
                // calculate current time within our lerping time range
                incrementor += Time.deltaTime;
                // calculate straight-line lerp position:
                Vector3 currentPos = Vector3.Lerp(startPos, endPos, incrementor);
                // add a value to Y, using Sine to give a curved trajectory in the Y direction
                currentPos.y += trajectoryHeight * Mathf.Sin(Mathf.Clamp01(incrementor) * Mathf.PI);
                // finally assign the computed position to our gameObject:
                transform.position = currentPos;

                if (Vector3.SqrMagnitude(transform.position - endPos) < 0.00000001)
                {
                    break;
                }

                yield return null;
            }

            if (GetAnimation() != Animations.Victory)
            {
                Set(Animations.Idle);
            }

            _rigidbody.velocity = Vector3.zero;
            _isAnimating = false;

            DestroyPopupIfAny();

            yield break;
        }

        private IEnumerator UpdateRotation()
        {
            _isAnimating = true;

            var incrementor = 0f;
            var scalingFactor = 1; // Bigger for slower

            while (transform.rotation != endRot)
            {
                // calculate current time within our lerping time range
                incrementor += Time.deltaTime / scalingFactor;
                // calculate straight-line lerp rotation:
                var currentRot = Quaternion.Lerp(startRot, endRot, incrementor);
                // check quarternion
                if (currentRot == transform.rotation) break;
                // finally assign the computed rotation to our gameObject:
                transform.rotation = currentRot;
                yield return null;
            }

            if (GetAnimation() != Animations.Victory)
            {
                Set(Animations.Idle);
            }

            _isAnimating = false;
            yield break;
        }

        private void UpdateScale(object localScale)
        {
            _scale = ((Vector3)localScale).x;
            DebugLog(_scale);
        }

        private GameObject GetZonesGameObject()
        {
            return GameObject.Find("Zones");
        }
    }
}
