using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class Character : MonoBehaviour
    {
        public TutorialManager tutorialManager;

        private Animator _animator;
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

        private bool _isDebug = false;
        private bool _isStopped;
        public float _scale;
        public AudioClip audioClipWalk;
		public AudioClip audioClipChewFood;
		public AudioClip audioClipJump;
        private AudioSource _audioSourceWalk;
		private AudioSource _audioSourceChewFood;
		private AudioSource _audioSourceJump;
        // Use this for initialization
        void Start()
        {
            _animator = GetComponent<Animator>();
            _originalPos = transform.position;
            _scale = transform.lossyScale.x;

			InitAudioClips();

            //Test Blockchain Movement
            //var zoneMovementController = GameObject.Find("Zones").GetComponent<ZoneMovementController>();
            //Debug.Log("Character Script Run");
            //zoneMovementController.MoveBlockChain();

            if (_isDebug)
            {
                Invoke("Forward", 1);

                Invoke("Forward", 5);
                Invoke("RotateLeft", 5);
                Invoke("Forward", 5);

                Invoke("Forward", 10);
                Invoke("RotateLeft", 10);
                Invoke("Forward", 10);
                Invoke("Jump", 10);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_isStopped)
            {
                return;
            }

            if (_isAnimating)
            {
                switch (_type)
                {
                    case ProgramBlockType.Forward:
                    case ProgramBlockType.Jump:
                    case ProgramBlockType.RotateLeft:
                    case ProgramBlockType.RotateRight:
                    default:
                        break;
                }
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
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Hole")
            {
                _isStopped = true;
            }
            else if (other.gameObject.tag == "Collectable")
            {
				_audioSourceChewFood.Play();

                other.gameObject.SetActive(false);

                var collectableBlock = other.gameObject.GetComponent<CollectableBlock>();
                if (collectableBlock != null)
                {
                    tutorialManager.ShowStage(collectableBlock.nextStage);

					Set(Animations.Victory);
                    Invoke("Reset", 2f);
                }
            }
        }

        public bool Forward()
        {
            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.Forward);
                return false;
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
            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.Jump);
                return false;
            }

            startPos = transform.position;
            endPos = transform.position + transform.forward * _scale * 2;
            trajectoryHeight = 0.5f;

            Set(Animations.Jump);
            _type = ProgramBlockType.Jump;

			_audioSourceJump.Play();

            StartCoroutine("UpdatePosition");
            return true;
        }

        public bool RotateLeft()
        {
            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.RotateLeft);
                return false;
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
            if (_isAnimating)
            {
                _queue.Enqueue(ProgramBlockType.RotateRight);
                return false;
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

		private void InitAudioClips() {
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

        private void Reset()
        {
            transform.position = _originalPos;
            transform.rotation = _originalRot;

			Set(Animations.Idle);
            //bool toggle = false;
            //while (!GameObject.Find("Zones").GetComponent<ZoneMovementController>().forward)
            //{
            //    if(!toggle)
            //    {
            //        GameObject.Find("Zones").GetComponent<ZoneMovementController>().MoveBlockChain();
            //        toggle = true;
            //    }
            //}
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

            while (transform.position != endPos)
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
                yield return null;
            }

            if (GetAnimation() != Animations.Victory)
			{
				Set(Animations.Idle);
			}

            _isAnimating = false;
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
    }
}
