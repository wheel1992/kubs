using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kubs
{
    public class TutorialManager : MonoBehaviour
    {
        public int lastStage;
        public bool onShowTutorialArrow { get; set; }

        private int _activeStage = 1;
        private Dictionary<int, List<TutorialBlock>> tutorialBlocks;
        public GameObject arrowPointer { set; get; }

        //For purpose of UI guide Arrows for Demonstration
        private void Start()
        {
        //    var hints = new UIProgramBlockHints.ProgramBlockGrabEventHandler;
        //    hints.ProgramBlockGrab += new UIProgramBlockHints.ProgramBlockGrabEventHandler(HandleProgramBlockGrab);
        //    var uiProgramBlockHints = new UIProgramBlockHints.ProgramBlockGrabEventHandler(HandleProgramBlockGrab);
            onShowTutorialArrow = true;
            GameObject forwardBlock = GameObject.Find("Program_Block_SnapDropZone_Clone_Forward");
            Vector3 arrowPos = forwardBlock.transform.position + new Vector3(0, 2f, 0f);
            arrowPointer = CreateArrowPointer(arrowPos);
        }

        private GameObject CreateArrowPointer(Vector3 position)
        {
            UIHintsArrowPointer uIHintsArrowPointer = new UIHintsArrowPointer();
            return uIHintsArrowPointer.CreateArrowPointer(position);
        }

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

		public void ShowStage(int stage, bool hidePreviousStages = true)
		{
			if (stage == 0)
			{
				return;
			}

			var prevStage = _activeStage;
			_activeStage = stage;

			if (stage < prevStage && hidePreviousStages)
			{
				HideStagesInRange(stage, prevStage);
			}

			if (stage != 1)
			{
				ShowStagesInRange(prevStage + 1, stage);
			}

			EventManager.TriggerEvent(Constant.EVENT_NAME_MENU_DISABLE, null);
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

        private GameObject GetProgramBlockSnapDropZoneCloneForward()
        {
            return GameObject.Find("Program_Block_SnapDropZone_Clone_Forward");
        }

    }
}
