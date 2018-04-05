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
            _mAudioSource.Stop();
            //_mAudioSource.Play();

            // onBlockProgramRegisterHoverEventListener = new UnityAction<object>(HandleBlockProgramRegisterHoverEventListener);
            // onBlockProgramRegisterSnapEventListener = new UnityAction<object>(HandleBlockProgramRegisterSnapEventListener);
        }
        // void OnEnable()
        // {
        //     EventManager.StartListening(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_HOVER_EVENT, onBlockProgramRegisterHoverEventListener);
        //     EventManager.StartListening(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_SNAP_EVENT, onBlockProgramRegisterSnapEventListener);
        // }
        // void OnDisable()
        // {
        //     EventManager.StopListening(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_HOVER_EVENT, onBlockProgramRegisterHoverEventListener);
        //     EventManager.StopListening(Constant.EVENT_NAME_CLONE_BLOCK_PROGRAM_REGISTER_SNAP_EVENT, onBlockProgramRegisterSnapEventListener);
        // }
        // Use this for initialization
        void Start()
        {
            var forwardBlock = CreateForwardBlock(new Vector3(0, 0, 0));
            var forStartBlock = CreateForStartBlock(new Vector3(0, 0, 0));
            var jumpBlock = CreateJumpBlock(new Vector3(0, 0, 0));
            var rotateLeftBlock = CreateRotateLeftBlock(new Vector3(0, 0, 0));
            var rotateRightBlock = CreateRotateRightBlock(new Vector3(0, 0, 0));

            GetVRTKSnapDropZoneCloneForward().ForceSnap(forwardBlock);
            GetVRTKSnapDropZoneCloneForStartEnd().ForceSnap(forStartBlock);
            GetVRTKSnapDropZoneCloneJump().ForceSnap(jumpBlock);
            GetVRTKSnapDropZoneCloneRotateLeft().ForceSnap(rotateLeftBlock);
            GetVRTKSnapDropZoneCloneRotateRight().ForceSnap(rotateRightBlock);

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
            //block.PauseSweepChildTrigger();
            //RegisterProgramBlockHoverEventHandler(block);

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
            //block.PauseSweepChildTrigger();
            //RegisterProgramBlockHoverEventHandler(block);

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
            //block.PauseSweepChildTrigger();
            //RegisterProgramBlockHoverEventHandler(block);

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
            //block.PauseSweepChildTrigger(); 
            //RegisterProgramBlockHoverEventHandler(block);

            return rotateRightBlock;
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