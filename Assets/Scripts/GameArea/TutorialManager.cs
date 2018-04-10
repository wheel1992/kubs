using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kubs
{
	public class TutorialManager : MonoBehaviour
	{
		public int lastStage;

		private int _activeStage = 1;
		private Dictionary<int, List<TutorialBlock>> tutorialBlocks;

		public void CollectChildren(Transform parent)
		{
			if (tutorialBlocks == null)
			{
				tutorialBlocks = new Dictionary<int, List<TutorialBlock>>();
			}

			foreach (Transform child in parent)
			{
				var tutorialBlock = child.gameObject.GetComponent<TutorialBlock>();
				if (tutorialBlock == null || tutorialBlock.stage == 0)
				{
					continue;
				}

				var stage = tutorialBlock.stage;
				if (!tutorialBlocks.ContainsKey(stage))
				{
					tutorialBlocks[stage] = new List<TutorialBlock>();
				}

				tutorialBlocks[stage].Add(tutorialBlock);
			}

			lastStage = tutorialBlocks.Keys.Max();

			StartCoroutine(TriggerReadOnNextFrame());
		}

		public void ShowStage(int stage)
		{
			if (stage == 0)
			{
				return;
			}

			var prevStage = _activeStage;
			_activeStage = stage;

			if (stage < prevStage)
			{
				HideStagesInRange(stage, prevStage);
			}

			if (stage != 1)
			{
				ShowStagesInRange(prevStage + 1, stage);
			}
		}

		public void TransferComponent(GameObject from, GameObject to)
		{
			var fromComponent = from.GetComponent<TutorialBlock>();
			if (fromComponent != null)
			{
				var toComponent = to.AddComponent<TutorialBlock>();
				toComponent.stage = fromComponent.stage;

				fromComponent.stage = 0;
			}
		}

		private void HideStagesInRange(int start, int end)
		{
			for (int stage = end; stage > start; stage--)
			{
				foreach (var block in tutorialBlocks[stage])
				{
					block.Shrink();
				}
			}
		}

		private void ShowStagesInRange(int start, int end)
		{
			for (int stage = start; stage <= end; stage++)
			{
				foreach (var block in tutorialBlocks[stage])
				{
					block.Grow();
				}
			}
		}

		private IEnumerator TriggerReadOnNextFrame()
		{
			yield return null;

			EventManager.TriggerEvent(Constant.EVENT_NAME_TUTORIAL_MANAGER_READY, this);
		}
	}
}
