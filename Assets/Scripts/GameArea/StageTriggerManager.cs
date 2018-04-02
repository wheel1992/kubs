using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class StageTriggerManager : MonoBehaviour
	{
		public bool autoSetNextStage = true;
		public int stage;

		private StageTrigger[] _stageTriggers;

		void Start()
		{
			AutoSetNextStageIfNeeded();
			CollectChildren();
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
		}

		private void OnChildTriggerEnter(StageTrigger trigger)
		{
			if (Array.TrueForAll(_stageTriggers, t => t.isTriggered))
			{
				StagesManager.LoadStage(stage);
			}
		}
	}
}
