using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	private bool _isAnimating;
	private ProgramBlockType _type;

	// Forward
	private Vector3 startPos;
	private Vector3 endPos;
	private float trajectoryHeight;

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		if (_isAnimating)
		{
			switch (_type)
			{
				case ProgramBlockType.Forward:
					// https://answers.unity.com/questions/8318/throwing-object-with-acceleration-equationscript.html
					// calculate current time within our lerping time range
					float cTime = Time.time * 0.5f;
					// calculate straight-line lerp position:
					Vector3 currentPos = Vector3.Lerp(startPos, endPos, cTime);
					// add a value to Y, using Sine to give a curved trajectory in the Y direction
					currentPos.y += trajectoryHeight * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
					Debug.Log(transform.position);
					if (transform.position == endPos)
					{
						Debug.Log("end");
						_isAnimating = false;
					}
					// finally assign the computed position to our gameObject:
					transform.position = currentPos;
					break;
				default:
					break;
			}
		}

	}

	public bool Forward()
	{
		startPos = new Vector3(0, 0.5f, 0);
		endPos = new Vector3(0, 0.5f, 4);
		trajectoryHeight = 1;

		_type = ProgramBlockType.Forward;
		_isAnimating = true;

		return false;
	}

	public bool Jump()
	{
		return false;
	}

	public bool RotateLeft()
	{
		return false;
	}

	public bool RotateRight()
	{
		return false;
	}
}
