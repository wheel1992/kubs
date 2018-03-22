using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kubs
{
    public class SceneLoad : MonoBehaviour
    {
        public delegate void ProgramBlockShiftRightEventHandler(int startZoneIndex);
        public event ProgramBlockShiftRightEventHandler ProgramBlockShiftRight;

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

        private void DoProgramBlockHover(ProgramBlock hoveredBlock)
        {
            ProgramBlockShiftRight(hoveredBlock.ZoneId);
        }

        private void RegisterProgramBlockEventHandler(ProgramBlock block)
        {
            block.Hover += new ProgramBlock.HoverEventHandler(DoProgramBlockHover);
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

            RegisterProgramBlockEventHandler(block);

            return rotateleftBlock;
        }

        GameObject CreateSweepTestChildBlock()
        {
            return Instantiate(_sweepTestChildBlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }


    }
}