using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kubs
{
	public class CharacterSelector : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{
			GetComponent<Button>().onClick.AddListener(TaskOnClick);
		}

		private void TaskOnClick()
		{
			var factory = GameObject.FindObjectOfType<CharacterFactory>();
			if (factory == null)
			{
				return;
			}

			factory.dirty = true;
		}
	}
}
