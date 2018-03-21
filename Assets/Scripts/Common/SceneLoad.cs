using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kubs
{
    public class SceneLoad : MonoBehaviour
    {
        [SerializeField] private GameObject _forwardBlockPrefab;
        [SerializeField] private GameObject _rotateLeftBlockPrefab;

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

        void CreateBlocks()
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

        GameObject CreateForwardBlock(Vector3 position)
        {
            var forwardBlock = (GameObject) Instantiate(
               _forwardBlockPrefab,
               position,
               Quaternion.identity);
            forwardBlock.tag = Constant.TAG_BLOCK_PROGRAM;

            ProgramBlock block = forwardBlock.AddComponent<ProgramBlock>();
            block.Type = ProgramBlockType.Forward;

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

            return rotateleftBlock;
        }


    }
}