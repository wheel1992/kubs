using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace Kubs
{
    public class CollectableBlock : Block
    {
        public CollectableBlockType Type { get; set; }

        #region Lifecycle

        void Awake()
        {
            Category = BlockCategory.Collectable;
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

    public enum CollectableBlockType
    {
        Fish = 0
    }
}