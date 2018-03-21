using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class TerrainBlock : Block
    {

        public TerrainBlockType Type { get; set; }

        #region Lifecycle

        void Awake()
        {
            Category = BlockCategory.Terrain;
        }

        // Use this for initialization
        void Start()
        {
            Start();
        }

        // Update is called once per frame
        void Update()
        {
            Update();
        }

        #endregion

        #region Public methods



        #endregion
    }

    public enum TerrainBlockType
    {
        Grass = 0,
        Sand = 1
    }
}