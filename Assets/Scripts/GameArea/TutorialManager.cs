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
        public GameObject arrowPointer { set; get; }

        private int _activeStage = 1;
        private Dictionary<int, List<TutorialBlock>> tutorialBlocks;

        [SerializeField] GameObject ArrowPrefab;

        //For purpose of UI guide Arrows for Demonstration
        private void Start()
        {
            Debug.Log("TutorialManager Start");
            //    var hints = new UIProgramBlockHints.ProgramBlockGrabEventHandler;
            //    hints.ProgramBlockGrab += new UIProgramBlockHints.ProgramBlockGrabEventHandler(HandleProgramBlockGrab);
            //    var uiProgramBlockHints = new UIProgramBlockHints.ProgramBlockGrabEventHandler(HandleProgramBlockGrab);
            onShowTutorialArrow = true;
            var arrowPos = GetProgramBlockSnapDropZoneCloneForward().transform.position + new Vector3(0, 2f, 0f);
			if (GameObject.Find("UIHintsArrowPointer") == null) {
				arrowPointer = CreateArrowPointer(arrowPos);
			} else {
				arrowPointer = GameObject.Find("UIHintsArrowPointer");
			}
        }

		void OnDestroy() {
			Destroy(arrowPointer);
		}

        private GameObject CreateArrowPointer(Vector3 position)
        {
            GameObject tempArrowPointer = Instantiate(ArrowPrefab);
            tempArrowPointer.transform.position = position;
            tempArrowPointer.AddComponent<UIHintsArrowPointer>();
            tempArrowPointer.name = "UIHintsArrowPointer";
            return tempArrowPointer;
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
        public void DestroyArrowPointer()
        {
            if (arrowPointer == null) { return; }
            Destroy(arrowPointer);
        }
        public void HideArrowPointer()
        {
            if (arrowPointer != null)
            {
                arrowPointer.GetComponent<UIHintsArrowPointer>().Hide();
            }
        }
        public void ShowArrowPointer()
        {
            if (arrowPointer != null)
            {
				var uiPointer = arrowPointer.GetComponent<UIHintsArrowPointer>();
                uiPointer.Show();
				uiPointer.BeginFloat();
            }
        }
        public void SetArrowPointerPositionToZone()
        {
            Debug.Log("Set arrow to zone there!");
            var targetPos = GameObject.Find("Zones").transform.GetChild(0).transform.position + new Vector3(0, 2f, 0);
            // arrowPointer.StopAllCoroutines();
            arrowPointer.GetComponent<UIHintsArrowPointer>().Hide();
            arrowPointer.transform.position = targetPos;
			
			Destroy(arrowPointer.GetComponent<UIHintsArrowPointer>());
			arrowPointer.gameObject.AddComponent<UIHintsArrowPointer>();

            arrowPointer.GetComponent<UIHintsArrowPointer>().Show();
            //arrowPointer.BeginFloat();
        }
        public void SetArrowPointerPositionToSnapClone()
        {
            Debug.Log("Set arrow to snap clone!");
            var targetPos = GetProgramBlockSnapDropZoneCloneForward().transform.position + new Vector3(0, 2f, 0);
            // arrowPointer.StopAllCoroutines();
            arrowPointer.GetComponent<UIHintsArrowPointer>().Hide();
            arrowPointer.transform.position = targetPos;

			Destroy(arrowPointer.GetComponent<UIHintsArrowPointer>());
			arrowPointer.gameObject.AddComponent<UIHintsArrowPointer>();

            arrowPointer.GetComponent<UIHintsArrowPointer>().Show();
            //arrowPointer.BeginFloat();
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
