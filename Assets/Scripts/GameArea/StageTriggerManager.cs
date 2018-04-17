using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class StageTriggerManager : MonoBehaviour
	{
		public TutorialManager tutorialManager;

		public bool autoSetNextStage = true;
		public int stage;

		private Menu _menu;
		private StageTrigger[] _stageTriggers;

		void Start()
		{
			_menu = GameObject.FindObjectOfType<SceneLoad>().menu.GetComponent<Menu>();

			AutoSetNextStageIfNeeded();
			CollectChildren();
		}

		public void Reset()
		{
			foreach (var stageTrigger in _stageTriggers)
			{
				stageTrigger.isTriggered = false;
				stageTrigger.gameObject.SetActive(true);
			}
		}

		private void AutoSetNextStageIfNeeded()
		{
			if (autoSetNextStage)
			{
				stage = StagesManager.GetActiveStage() + 1;
			}
		}

		private void CollectChildren()
		{
			_stageTriggers = GetComponentsInChildren<StageTrigger>();

			if (tutorialManager != null)
			{
				tutorialManager.CollectChildren(transform);
			}
		}

		private void OnChildTriggerEnter(StageTrigger trigger)
		{
			if (Array.TrueForAll(_stageTriggers, t => t.isTriggered))
			{
				var fadeSpeed = GetComponent<Fading>().BeginFade(1);
				Invoke("LoadStage", 1f / fadeSpeed);
			}
		}

		private void LoadStage()
		{
			_menu.ShowMedal(stage - 1);
			StagesManager.LoadStageAsync(stage, this);
		}
	}
}
