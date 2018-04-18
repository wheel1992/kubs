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
        private ButtonStart _buttonStart;

        [SerializeField] GameObject ArrowPrefab;
        [SerializeField] AudioClip audioClipTutorialIntroduction;
        [SerializeField] AudioClip audioClipTutorialStageFour;

        private AudioSource _audioSourceTutorialIntroduction;
        private AudioSource _audioSourceTutorialStageFour;
        private List<AudioSource> _audioSources;
        //For purpose of UI guide Arrows for Demonstration
        private void Start()
        {
            Debug.Log("TutorialManager Start");
            //    var hints = new UIProgramBlockHints.ProgramBlockGrabEventHandler;
            //    hints.ProgramBlockGrab += new UIProgramBlockHints.ProgramBlockGrabEventHandler(HandleProgramBlockGrab);
            //    var uiProgramBlockHints = new UIProgramBlockHints.ProgramBlockGrabEventHandler(HandleProgramBlockGrab);
            onShowTutorialArrow = true;

            Vector3 arrowPos = GetProgramBlockSnapDropZoneCloneForward().transform.position + new Vector3(0, 2f, 0f);
            // if (_activeStage == 1)
            // {
            //     arrowPos = 
            // }
            // else if (_activeStage == 4)
            // {
            //     arrowPos = GetProgramBlockSnapDropZoneCloneForStartEnd().transform.position + new Vector3(0, 2f, 0f);
            // }
            // else
            // {
            //     return;
            // }

            if (GameObject.Find("UIHintsArrowPointer") == null)
            {
                arrowPointer = CreateArrowPointer(arrowPos);
            }
            else
            {
                arrowPointer = GameObject.Find("UIHintsArrowPointer");
            }

            _buttonStart = GetButtonStart();
            _buttonStart.OnTouched += new ButtonStart.ButtonEventHandler(HandleButtonStartOnTouched);

            _audioSources = new List<AudioSource>();
            InitAudioClips();

            if(_activeStage == 1) {
                StopAllAudioSources();
                _audioSourceTutorialIntroduction.Play();
            }
        }

        void OnDestroy()
        {
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
        public int GetCurrentActiveStage()
        {
            return _activeStage;
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

            if (_activeStage == 1)
            {
                StopAllAudioSources();
                _audioSourceTutorialIntroduction.Play();

                onShowTutorialArrow = true;
                SetArrowPointerPositionToSnapCloneForward();
            }
            else if (_activeStage == 4)
            {
                StopAllAudioSources();
                _audioSourceTutorialStageFour.Play();

                onShowTutorialArrow = true;
                SetArrowPointerPositionToSnapCloneForStartEnd();
            }
            else
            {
                onShowTutorialArrow = false;
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
        public void SetArrowPointerPositionToButtonStart()
        {
            Debug.Log("Set arrow to button start!");
            if (_buttonStart == null) { return; }
            var targetPos = _buttonStart.transform.position + new Vector3(0, 2f, 0);
            // arrowPointer.StopAllCoroutines();
            arrowPointer.GetComponent<UIHintsArrowPointer>().Hide();
            arrowPointer.transform.position = targetPos;

            Destroy(arrowPointer.GetComponent<UIHintsArrowPointer>());
            arrowPointer.gameObject.AddComponent<UIHintsArrowPointer>();

            arrowPointer.GetComponent<UIHintsArrowPointer>().Show();
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
        public void SetArrowPointerPositionToSnapCloneForward()
        {
            Debug.Log("Set arrow to snap clone forward!");
            var targetPos = GetProgramBlockSnapDropZoneCloneForward().transform.position + new Vector3(0, 2f, 0);
            // arrowPointer.StopAllCoroutines();
            arrowPointer.GetComponent<UIHintsArrowPointer>().Hide();
            arrowPointer.transform.position = targetPos;

            Destroy(arrowPointer.GetComponent<UIHintsArrowPointer>());
            arrowPointer.gameObject.AddComponent<UIHintsArrowPointer>();

            arrowPointer.GetComponent<UIHintsArrowPointer>().Show();
            //arrowPointer.BeginFloat();
        }
        public void SetArrowPointerPositionToSnapCloneForStartEnd()
        {
            Debug.Log("Set arrow to snap clone For Start End!");
            var targetPos = GetProgramBlockSnapDropZoneCloneForStartEnd().transform.position + new Vector3(0, 2f, 0);
            // arrowPointer.StopAllCoroutines();
            arrowPointer.GetComponent<UIHintsArrowPointer>().Hide();
            arrowPointer.transform.position = targetPos;

            Destroy(arrowPointer.GetComponent<UIHintsArrowPointer>());
            arrowPointer.gameObject.AddComponent<UIHintsArrowPointer>();

            arrowPointer.GetComponent<UIHintsArrowPointer>().Show();
        }
        private void HandleButtonStartOnTouched()
        {
            // Button Start is touched
            // Character is playing
            onShowTutorialArrow = false;
            // DestroyArrowPointer();
            HideArrowPointer();
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
        private void InitAudioClips()
        {
            _audioSourceTutorialIntroduction = gameObject.AddComponent<AudioSource>();
            _audioSourceTutorialIntroduction.clip = audioClipTutorialIntroduction;
            _audioSourceTutorialIntroduction.loop = false;
            _audioSourceTutorialIntroduction.playOnAwake = false;
            _audioSourceTutorialIntroduction.volume = 1.0f;

            _audioSources.Add(_audioSourceTutorialIntroduction);

            _audioSourceTutorialStageFour = gameObject.AddComponent<AudioSource>();
            _audioSourceTutorialStageFour.clip = audioClipTutorialStageFour;
            _audioSourceTutorialStageFour.loop = false;
            _audioSourceTutorialStageFour.playOnAwake = false;
            _audioSourceTutorialStageFour.volume = 1.0f;

            _audioSources.Add(_audioSourceTutorialStageFour);
        }
        private void StopAllAudioSources()
        {
            if (_audioSources == null || _audioSources.Count == 0) { return; }
            foreach (var source in _audioSources)
            {
                source.Stop();
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
        private GameObject GetProgramBlockSnapDropZoneCloneForStartEnd()
        {
            return GameObject.Find("Program_Block_SnapDropZone_Clone_ForStartEnd");
        }
        private ButtonStart GetButtonStart()
        {
            var go = GameObject.Find("ButtonStart_New");
            if (go == null) { return null; }
            return go.GetComponent<ButtonStart>();
        }
    }
}
