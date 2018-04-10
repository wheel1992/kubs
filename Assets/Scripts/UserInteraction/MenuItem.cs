using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kubs
{
	public class MenuItem : VRUIItem
	{
		public bool isTutorial;
		public int stage;

		// Use this for initialization
		void Start ()
		{
			GetComponent<Button>().onClick.AddListener(TaskOnClick);
		}

		private void TaskOnClick()
		{
			if (isTutorial)
			{
				EventManager.StartListening(Constant.EVENT_NAME_TUTORIAL_MANAGER_READY, ShowTutorial);
				StagesManager.LoadStageAsync(0, this);
			}
			else
			{
				StagesManager.LoadStageAsync(stage, this);
			}
		}

		private void ShowTutorial(object o)
		{
			EventManager.StopListening(Constant.EVENT_NAME_TUTORIAL_MANAGER_READY, ShowTutorial);

			var tutorialManager = o as TutorialManager;
			if (tutorialManager != null)
			{
				tutorialManager.ShowStage(stage);
			}
		}
	}
}
