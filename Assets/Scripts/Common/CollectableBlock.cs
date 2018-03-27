using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class CollectableBlock : Block
    {
        public int nextStage;
        public CollectableBlockType Type { get; set; }

        #region Lifecycle

        void Awake()
        {
            Category = BlockCategory.Collectable;
        }

        void Update()
        {
            transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        }

        #endregion

        #region Public methods

        #endregion
    }

    public enum CollectableBlockType
    {
        Fish = 0
    }
}
