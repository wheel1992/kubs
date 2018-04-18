using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class CharacterFactory : MonoBehaviour
	{
		public GameObject prefab;

		// Use this for initialization
		void Start()
		{
			var placeholder = transform.GetChild(0).gameObject;
			placeholder.SetActive(false);

			var go = Instantiate(prefab, transform);
			var character = go.AddComponent<Character>();
			character.transform.localPosition = placeholder.transform.localPosition;
			character.showPopup = true;
			character.stageTriggerManager = GameObject.FindObjectOfType<StageTriggerManager>();
			character.tutorialManager = GameObject.FindObjectOfType<TutorialManager>();
		}
	}
}
