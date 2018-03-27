using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class Block : MonoBehaviour
    {
        public BlockCategory Category { get; set; }

        public int? Value { get; set; }

        // Use this for initialization
        void Start()
        {
            gameObject.AddComponent<BlockController>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public enum BlockCategory
    {
        Collectable = 0,
        Obstacle = 1,
        Program = 2,
        Terrain = 3,
    }
}

