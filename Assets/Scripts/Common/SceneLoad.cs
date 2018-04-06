using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class SceneLoad : MonoBehaviour
    {
        public delegate void ProgramBlockShiftEventHandler(int startZoneIndex);
        public delegate void ProgramBlockPlaceEventHandler(int startZoneIndex);
        public delegate void ProgramBlockSnapEventHandler(GameObject block, int zoneId);
        public event ProgramBlockShiftEventHandler ProgramBlockShiftRightWhenHover;
        public event ProgramBlockShiftEventHandler ProgramBlockShiftRevert;
        public event ProgramBlockSnapEventHandler ProgramBlockSnap;

        public Material skybox;
        [SerializeField] private GameObject _forwardBlockPrefab;
        [SerializeField] private GameObject _forLoopStartBlockPrefab;
        [SerializeField] private GameObject _forLoopEndBlockPrefab;
        [SerializeField] private GameObject _rotateLeftBlockPrefab;
        [SerializeField] private GameObject _rotateRightBlockPrefab;
        [SerializeField] private GameObject _jumpBlockPrefab;
        [SerializeField] private GameObject _sweepTestChildBlockPrefab;
        private AudioSource _mAudioSource;
        // private UnityAction<object> onBlockProgramRegisterHoverEventListener;
        // private UnityAction<object> onBlockProgramRegisterSnapEventListener;
        private ButtonStart _buttonStart;
        private GameObject _zonesObject;

        void Awake()
        {
            _mAudioSource = GetComponent<AudioSource>();
            //_mAudioSource.Stop();
            _mAudioSource.Play();
        }
        void Start()
        {
            var forwardBlock = CreateForwardBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneForward().ForceSnap(forwardBlock);

            // For Friday 6 April 2018 demo, we do not allow clone ForLoop
            // var forStartBlock = CreateForStartBlock(new Vector3(0, 0, 0));
            // GetVRTKSnapDropZoneCloneForStartEnd().ForceSnap(forStartBlock);
            var forStartblock = GetForLoopStartProgramBlock();
            if (forStartblock != null) {
                forStartblock.Type = ProgramBlockType.ForLoopStart;
            }
  
            var jumpBlock = CreateJumpBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneJump().ForceSnap(jumpBlock);

            var rotateLeftBlock = CreateRotateLeftBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneRotateLeft().ForceSnap(rotateLeftBlock);

            var rotateRightBlock = CreateRotateRightBlock(new Vector3(0, 0, 0));
            GetVRTKSnapDropZoneCloneRotateRight().ForceSnap(rotateRightBlock);

            // Load tutorial
            Invoke("LoadTutorial", 1);
        }

        void LoadTutorial()
        {
            // Set skybox
            GameObject.FindGameObjectWithTag("MainCamera").AddComponent<Skybox>().material = skybox;

            StagesManager.loadPos = new Vector3(-25, 0, 0);
            StagesManager.loadScale = new Vector3(3, 3, 3);
            StagesManager.LoadStageAsync(0, this);
        }

        // Update is called once per frame
        void Update()
        {
            // ...
        }

        // private void DoProgramBlockHover(int targetZoneId)
        // {
        //     ProgramBlockShiftRightWhenHover(targetZoneId);
        // }
        // private void DoProgramBlockUnhover(int targetZoneId)
        // {
        //     ProgramBlockShiftRevert(targetZoneId);
        // }
        // private void DoProgramBlockSnap(GameObject block, int zoneId)
        // {
        //     ProgramBlockSnap(block, zoneId);
        // }
        // private void HandleBlockProgramRegisterHoverEventListener(object item)
        // {
        //     if (item is GameObject)
        //     {
        //         GameObject obj = (GameObject)item;
        //         RegisterProgramBlockHoverEventHandler(obj.GetComponent<ProgramBlock>());
        //     }
        // }
        // private void HandleBlockProgramRegisterSnapEventListener(object item)
        // {
        //     if (item is GameObject)
        //     {
        //         GameObject obj = (GameObject)item;
        //         RegisterProgramBlockSnapEventHandler(obj.GetComponent<ProgramBlock>());
        //     }
        // }
        // private void RegisterProgramBlockHoverEventHandler(ProgramBlock block)
        // {
        //     block.Hover += new ProgramBlock.HoverEventHandler(DoProgramBlockHover);
        //     block.Unhover += new ProgramBlock.HoverEventHandler(DoProgramBlockUnhover);
        // }
        // private void RegisterProgramBlockSnapEventHandler(ProgramBlock block)
        // {
        //     block.Snap += new ProgramBlock.SnapEventHandler(DoProgramBlockSnap);
        // }
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
