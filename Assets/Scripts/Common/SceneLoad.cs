using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VRTK;

namespace Kubs
{
    public class SceneLoad : MonoBehaviour
    {
        public delegate void ProgramBlockShiftEventHandler(int startZoneIndex);
        public delegate void ProgramBlockPlaceEventHandler(int startZoneIndex);
        public delegate void ProgramBlockSnapEventHandler(GameObject block, int zoneId);

        public GameObject menu;
        public GameObject characterManager
        {
            get
            {
                if (_characterManager == null)
                {
                    _characterManager = GetCharacterManager();
                }

                return _characterManager;
            }
        }
        private GameObject _characterManager;
        public Material skybox;

        [SerializeField] private GameObject _forwardBlockPrefab;
        [SerializeField] private GameObject _forLoopStartBlockPrefab;
        [SerializeField] private GameObject _rotateLeftBlockPrefab;
        [SerializeField] private GameObject _rotateRightBlockPrefab;
        [SerializeField] private GameObject _jumpBlockPrefab;
        [SerializeField] private GameObject _sweepTestChildBlockPrefab;
        private AudioSource _mAudioSource;
        private ButtonStart _buttonStart;
        private GameObject _zonesObject;
        private GameObject _rightCtrl;
        private RadialMenuManager _rightCtrlRadialMenuManager;
        
    
        void Awake()
        {
            _mAudioSource = GetComponent<AudioSource>();
            _mAudioSource.volume = 0.3f;
            _mAudioSource.Play();
        }
        void Start()
        {
            SceneManager.LoadScene("Showcase_preview", LoadSceneMode.Additive);

            var forwardBlock = CreateForwardBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneForward().ForceSnap(forwardBlock);

            var forStartBlock = CreateForStartBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneForStartEnd().ForceSnap(forStartBlock);

            var jumpBlock = CreateJumpBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneJump().ForceSnap(jumpBlock);

            var rotateLeftBlock = CreateRotateLeftBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneRotateLeft().ForceSnap(rotateLeftBlock);

            var rotateRightBlock = CreateRotateRightBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneRotateRight().ForceSnap(rotateRightBlock);

            _rightCtrl = GetRightController();
            _rightCtrlRadialMenuManager = GetRightControllerRadialMenuManager();

            // characterManager = GetCharacterManager();

            // Load tutorial    
            Invoke("LoadTutorial", 1);
        }
        void DisableCharacterManager()
        {
            characterManager.SetActive(false);
        }
        void DisableMenu()
        {
            menu.SetActive(false);
        }
        void EnableCharacterManager()
        {
            characterManager.SetActive(true);
            var menuPos = menu.transform.position;
            characterManager.transform.position = new Vector3(menuPos.x, menuPos.y, menuPos.z);
            characterManager.transform.rotation = menu.transform.rotation;
        }
        void EnableMenu()
        {
            menu.SetActive(true);
            var menuDistance = 7;
            var camera = GetVRTKHeadsetCamera();
            var menuPos = camera.position + camera.forward * menuDistance;
            menu.transform.position = menuPos;
            menu.transform.rotation = camera.rotation;
        }
       
        void HandleMenuDisable(object sender)
        {
            DisableMenu();
            DisableCharacterManager();
            if (_rightCtrlRadialMenuManager != null)
            {
                _rightCtrlRadialMenuManager.HandleMenuDisable();
            }

            var characterFactory = GameObject.FindObjectOfType<CharacterFactory>();
            if (characterFactory != null)
            {
                characterFactory.HandleMenuDisable(null);
            }
        }
        void HandleMenuEnable(object sender)
        {
            EnableMenu();
            EnableCharacterManager();
            if (_rightCtrlRadialMenuManager != null)
            {
                _rightCtrlRadialMenuManager.HandleMenuEnable();
            }
        }
        void LoadTutorial()
        {
            // Set skybox
            GameObject.FindGameObjectWithTag("MainCamera").AddComponent<Skybox>().material = skybox;

            StartCoroutine(LoadSceneAsync(Constant.NAME_SCENE_MENU_SCENE));
        }
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // Set the current Scene to be able to unload it later
            Scene currentScene = SceneManager.GetActiveScene();

            // The Application loads the Scene in the background at the same time as the current Scene.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Wait until the last operation fully loads to return anything
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Merge scenes
            var nextScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.MergeScenes(nextScene, currentScene);

            // Load tutorial
            StagesManager.loadPos = new Vector3(-25, 0, 0);
            StagesManager.loadScale = new Vector3(3, 3, 3);
            StagesManager.LoadStageAsync(0, this);

            if (sceneName.Equals(Constant.NAME_SCENE_MENU_SCENE))
            {
                menu = GetMenu();
                DisableMenu();
            }
        }
        void OnEnable()
        {
            EventManager.StartListening(Constant.EVENT_NAME_MENU_DISABLE, HandleMenuDisable);
            EventManager.StartListening(Constant.EVENT_NAME_MENU_ENABLE, HandleMenuEnable);
        }

        void OnDisable()
        {
            EventManager.StopListening(Constant.EVENT_NAME_MENU_DISABLE, HandleMenuDisable);
            EventManager.StopListening(Constant.EVENT_NAME_MENU_ENABLE, HandleMenuEnable);
        }

        GameObject CreateForwardBlock(Vector3 position)
        {
            var forwardBlock = (GameObject)Instantiate(
               _forwardBlockPrefab,
               position,
               Quaternion.identity);
            forwardBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = forwardBlock.GetComponent<ProgramBlock>();
            block.Type = ProgramBlockType.Forward;

            return forwardBlock;
        }
        GameObject CreateForStartBlock(Vector3 position)
        {
            var forStartBlock = (GameObject)Instantiate(
               _forLoopStartBlockPrefab,
               position,
               Quaternion.identity);
            forStartBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = forStartBlock.GetComponent<ProgramBlock>();
            block.Type = ProgramBlockType.ForLoopStart;

            return forStartBlock;
        }
        GameObject CreateJumpBlock(Vector3 position)
        {
            var jumpBlock = (GameObject)Instantiate(
               _jumpBlockPrefab,
               position,
               Quaternion.identity);
            jumpBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = jumpBlock.GetComponent<ProgramBlock>();
            block.Type = ProgramBlockType.Jump;

            return jumpBlock;
        }
        GameObject CreateRotateLeftBlock(Vector3 position)
        {
            var rotateleftBlock = (GameObject)Instantiate(
              _rotateLeftBlockPrefab,
              position,
              Quaternion.identity);
            rotateleftBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = rotateleftBlock.GetComponent<ProgramBlock>();
            block.Type = ProgramBlockType.RotateLeft;

            return rotateleftBlock;
        }
        GameObject CreateRotateRightBlock(Vector3 position)
        {
            var rotateRightBlock = (GameObject)Instantiate(
              _rotateRightBlockPrefab,
              position,
              Quaternion.identity);
            rotateRightBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = rotateRightBlock.GetComponent<ProgramBlock>();
            block.Type = ProgramBlockType.RotateRight;

            return rotateRightBlock;
        }
        GameObject GetCharacterManager()
        {
            return GameObject.Find("CharacterManager");
        }
        ProgramBlock GetForLoopStartProgramBlock()
        {
            var area = GameObject.Find("SnapCloneBlockArea");
            for (int i = 0; i < area.transform.childCount; i++)
            {
                if (area.transform.GetChild(i).gameObject.name.CompareTo("ForStart_ProgramBlock") == 0)
                {
                    return area.transform.GetChild(i).gameObject.GetComponent<ProgramBlock>();
                }
            }
            return null;
        }
        GameObject GetMenu()
        {
            return GameObject.FindGameObjectWithTag(Constant.TAG_CANVAS_MENU);
        }
        GameObject GetRightController()
        {
            return GameObject.Find("RightController");
        }
        RadialMenuManager GetRightControllerRadialMenuManager()
        {
            if (_rightCtrl == null) { return null; }
            var radialMenuPanel = _rightCtrl.transform.Find("RadialMenu/RadialMenuUI/Panel");
            if (radialMenuPanel == null) { return null; }
            return radialMenuPanel.GetComponent<RadialMenuManager>();
        }
        Transform GetVRTKHeadsetCamera()
        {
            return VRTK_DeviceFinder.HeadsetCamera();
        }
        VRTK_SnapDropZone GetVRTKSnapDropZoneCloneForward()
        {
            return GameObject.FindGameObjectWithTag(Constant.TAG_SNAP_DROP_ZONE_CLONE_FORWARD).GetComponent<VRTK_SnapDropZone>();
        }
        VRTK_SnapDropZone GetVRTKSnapDropZoneCloneForStartEnd()
        {
            return GameObject.FindGameObjectWithTag(Constant.TAG_SNAP_DROP_ZONE_CLONE_FOR_START_END).GetComponent<VRTK_SnapDropZone>();
        }
        VRTK_SnapDropZone GetVRTKSnapDropZoneCloneJump()
        {
            return GameObject.FindGameObjectWithTag(Constant.TAG_SNAP_DROP_ZONE_CLONE_JUMP).GetComponent<VRTK_SnapDropZone>();
        }
        VRTK_SnapDropZone GetVRTKSnapDropZoneCloneRotateLeft()
        {
            return GameObject.FindGameObjectWithTag(Constant.TAG_SNAP_DROP_ZONE_CLONE_ROTATELEFT).GetComponent<VRTK_SnapDropZone>();
        }
        VRTK_SnapDropZone GetVRTKSnapDropZoneCloneRotateRight()
        {
            return GameObject.FindGameObjectWithTag(Constant.TAG_SNAP_DROP_ZONE_CLONE_ROTATERIGHT).GetComponent<VRTK_SnapDropZone>();
        }
    }
}
