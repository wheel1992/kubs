using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubs
{
    public class ObstacleBlock : Block
    {

        public ObstacleBlockType Type { get; set; }

        #region Lifecycle

        void Awake()
        {
            Category = BlockCategory.Obstacle;
        }

        #endregion

        #region Public methods



        #endregion
    }

    public enum ObstacleBlockType
    {
        Water = 0,
        Mud = 1,
        Beehive = 2
    }
}