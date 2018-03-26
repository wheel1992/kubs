using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class Character : MonoBehaviour
	{
		private Animator _animator;
		private bool _isAnimating;
		private Queue<ProgramBlockType> _queue = new Queue<ProgramBlockType>();
		private ProgramBlockType _type;

		// Forward
		private Vector3 startPos;
		private Vector3 endPos;
		private float trajectoryHeight;

		private bool _isDebug = true;

		// Use this for initialization
		void Start ()
		{
			_animator = GetComponent<Animator>();

			if (_isDebug)
			{
				Invoke("Forward", 1);
				Invoke("Jump", 1);
				Invoke("Forward", 1);
			}
		}

		// Update is called once per frame
		void Update ()
		{
			if (_isAnimating)
			{
				switch (_type)
				{
					case ProgramBlockType.Forward:
					case ProgramBlockType.Jump:
					default:
						break;
				}
			}
			else if (_queue.Count > 0)
			{
				switch (_queue.Dequeue())
				{
					case ProgramBlockType.Forward:
						Forward_Start();
						break;
					case ProgramBlockType.Jump:
						Jump_Start();
						break;
					default:
						break;
				}
			}
		}

		public bool Forward()
		{
			return _isAnimating ? Enqueue(ProgramBlockType.Forward) : Forward_Start();
		}

		private bool Forward_Start()
		{
			startPos = transform.position;
			endPos = transform.position + transform.forward;
			trajectoryHeight = 0;

			Set(Animations.Move);

			_type = ProgramBlockType.Forward;
			_isAnimating = true;

			StartCoroutine("Forward_Update");

			return true;
		}

		private IEnumerator Forward_Update()
		{
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

			DebugLog("end");
			SetIdle();
			_isAnimating = false;
			yield break;
		}

		public bool Jump()
		{
			return _isAnimating ? Enqueue(ProgramBlockType.Jump) : Jump_Start();
		}

		private bool Jump_Start()
		{
			startPos = transform.position;
			endPos = transform.position + transform.forward * 2;
			trajectoryHeight = 0.5f;

			Set(Animations.Jump);

			_type = ProgramBlockType.Jump;
			_isAnimating = true;

			StartCoroutine("Forward_Update");

			return true;
		}

		public bool RotateLeft()
		{
			return false;
		}

		public bool RotateRight()
		{
			return false;
		}

		private void DebugLog(object s)
		{
			if (_isDebug)
			{
				Debug.Log(s);
			}
		}

		private bool Enqueue(ProgramBlockType type)
		{
			_queue.Enqueue(type);
			return false;
		}

		private void Set(Animations animation)
		{
			_animator.SetInteger("animation", (int)animation);
		}

		private void SetIdle()
		{
			Set(Animations.Idle);
		}
	}
}
