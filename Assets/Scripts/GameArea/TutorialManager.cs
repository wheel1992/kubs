using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
	public class TutorialManager : MonoBehaviour
	{
		private Dictionary<int, List<TutorialBlock>> tutorialBlocks;

		void Start()
		{
			tutorialBlocks = new Dictionary<int, List<TutorialBlock>>();
		}

		public void CollectChildren(Transform parent)
		{
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
		}

		public void ShowStage(int stage)
		{
			if (stage == 0)
			{
				return;
			}

			foreach (var block in tutorialBlocks[stage])
			{
				block.Grow();
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
	}
}
