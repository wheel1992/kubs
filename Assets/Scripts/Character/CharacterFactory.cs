using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class CharacterFactory : MonoBehaviour
	{
		public GameObject prefab { get { return _characterManager.activePrefab; }}
		public bool dirty;

		private CharacterManager _characterManager
		{
			get
			{
				if (__characterManager == null)
				{
					var go = GameObject.FindObjectOfType<SceneLoad>().characterManager;
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
			ReplaceCharacter();
		}

        void OnEnable()
        {
            EventManager.StartListening(Constant.EVENT_NAME_MENU_DISABLE, HandleMenuDisable);
			Debug.Log("CharacterFactory.OnEnable");
        }

        void OnDisable()
        {
            EventManager.StopListening(Constant.EVENT_NAME_MENU_DISABLE, HandleMenuDisable);
			Debug.Log("CharacterFactory.OnDisable");
        }

		public void SetDirty()
		{
			dirty = true;
		}

		public void HandleMenuDisable(object sender)
		{
			Debug.Log("CharacterFactory.HandleMenuDisable");

			if (!dirty) return;
			dirty = false;

			ReplaceCharacter();
		}

		private void ReplaceCharacter()
		{
			var placeholder = transform.GetChild(transform.childCount - 1).gameObject;
			placeholder.SetActive(false);

			var go = Instantiate(prefab, transform);
			go.SetActive(true);
			go.name = "Character";

			var character = go.AddComponent<Character>();
			character.transform.localPosition = placeholder.transform.localPosition;
			character.transform.localRotation = placeholder.transform.localRotation;
			character.showPopup = true;
			character.stageTriggerManager = GameObject.FindObjectOfType<StageTriggerManager>();
			character.tutorialManager = GameObject.FindObjectOfType<TutorialManager>();

			var boxCollider = go.AddComponent<BoxCollider>();
			boxCollider.center = placeholder.GetComponent<BoxCollider>().center;
			boxCollider.size = placeholder.GetComponent<BoxCollider>().size;

			// Copy audio sources
			character.CopyAudio(placeholder.GetComponent<Character>());

			// Copy popups
			character.CopyPopups(placeholder.GetComponent<Character>());
		}
	}
}
