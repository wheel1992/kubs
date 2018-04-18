using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class CharacterFactory : MonoBehaviour
	{
		public GameObject prefab { get { return _characterManager.activePrefab; }}

		private CharacterManager _characterManager
		{
			get
			{
				if (__characterManager == null)
				{
					var go = GameObject.Find("CharacterManager");
					if (go == null)
					{
						return null;
					}

					__characterManager = go.GetComponent<CharacterManager>();
				}

				return __characterManager;
			}
		}
		private CharacterManager __characterManager;

		// Use this for initialization
		void Start()
		{
			var placeholder = transform.GetChild(0).gameObject;
			placeholder.SetActive(false);

			var go = Instantiate(prefab, transform);
			go.SetActive(true);

			var character = go.AddComponent<Character>();
			character.transform.localPosition = placeholder.transform.localPosition;
			character.transform.localRotation = placeholder.transform.localRotation;
			character.showPopup = true;
			character.stageTriggerManager = GameObject.FindObjectOfType<StageTriggerManager>();
			character.tutorialManager = GameObject.FindObjectOfType<TutorialManager>();

			var boxCollider = go.AddComponent<BoxCollider>();
			boxCollider.center = placeholder.GetComponent<BoxCollider>().center;
			boxCollider.size = placeholder.GetComponent<BoxCollider>().size;
		}
	}
}
