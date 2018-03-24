using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kubs
{
    public class SceneLoad : MonoBehaviour
    {
        public delegate void ProgramBlockShiftEventHandler(int startZoneIndex);
        public delegate void ProgramBlockPlaceEventHandler(int startZoneIndex);
        public delegate void ProgramBlockSnapEventHandler(GameObject block, int zoneId);
        public event ProgramBlockShiftEventHandler ProgramBlockShiftRightWhenHover;
        public event ProgramBlockShiftEventHandler ProgramBlockShiftRevert;
        public event ProgramBlockPlaceEventHandler ProgramBlockPlace;
        public event ProgramBlockSnapEventHandler ProgramBlockSnap;

        [SerializeField] private GameObject _forwardBlockPrefab;
        [SerializeField] private GameObject _rotateLeftBlockPrefab;
        [SerializeField] private GameObject _sweepTestChildBlockPrefab;

        // Use this for initialization
        void Start()
        {
            CreateBlocks();
        }

        // Update is called once per frame
        void Update()
        {
            // ...
        }

        private void CreateBlocks()
        {
            float blockSize = 1f;
            float startX = -3f;
            float startZ = -2f;

            // let's create a few forward blocks
            CreateForwardBlock(new Vector3(startX, Constant.DEFAULT_Y, startZ));
            startZ += blockSize;
            CreateForwardBlock(new Vector3(startX, Constant.DEFAULT_Y, startZ));
            startZ += blockSize;
            CreateForwardBlock(new Vector3(startX, Constant.DEFAULT_Y, startZ));

            startX = -1f;
            startZ = -2f;
            CreateRotateLeftBlock(new Vector3(startX, Constant.DEFAULT_Y, startZ));
            startZ += blockSize;
            CreateRotateLeftBlock(new Vector3(startX, Constant.DEFAULT_Y, startZ));
            startZ += blockSize;
            CreateRotateLeftBlock(new Vector3(startX, Constant.DEFAULT_Y, startZ));
        }

        private void DoProgramBlockHover(int targetZoneId)
        {
            ProgramBlockShiftRightWhenHover(targetZoneId);
        }

        private void DoProgramBlockUnhover(int targetZoneId)
        {
            ProgramBlockShiftRevert(targetZoneId);
        }

        private void DoProgramBlockPlace(int targetZoneId)
        {
            ProgramBlockPlace(targetZoneId);
        }

        private void DoProgramBlockSnap(GameObject block, int zoneId)
        {
            ProgramBlockSnap(block, zoneId);
        }

        private void RegisterProgramBlockEventHandler(ProgramBlock block)
        {
            block.Hover += new ProgramBlock.HoverEventHandler(DoProgramBlockHover);
            block.Unhover += new ProgramBlock.HoverEventHandler(DoProgramBlockUnhover);
            block.Place += new ProgramBlock.PlaceEventHandler(DoProgramBlockPlace);
            block.Snap += new ProgramBlock.SnapEventHandler(DoProgramBlockSnap);
        }

        GameObject CreateForwardBlock(Vector3 position)
        {
            var forwardBlock = (GameObject) Instantiate(
               _forwardBlockPrefab,
               position,
               Quaternion.identity);
            forwardBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = forwardBlock.AddComponent<ProgramBlock>();
            block.Type = ProgramBlockType.Forward;
            block.PauseSweepChildTrigger();

            RegisterProgramBlockEventHandler(block);

            return forwardBlock;
        }

        GameObject CreateRotateLeftBlock(Vector3 position)
        {
            var rotateleftBlock = (GameObject) Instantiate(
              _rotateLeftBlockPrefab,
              position,
              Quaternion.identity);
            rotateleftBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = rotateleftBlock.AddComponent<ProgramBlock>();
            block.Type = ProgramBlockType.RotateLeft;
            block.PauseSweepChildTrigger();
            RegisterProgramBlockEventHandler(block);

            return rotateleftBlock;
        }

    }
}