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