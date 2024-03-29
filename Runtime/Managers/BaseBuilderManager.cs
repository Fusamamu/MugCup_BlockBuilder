using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace MugCup_BlockBuilder.Runtime.Core
{
    /// <summary>
    /// This is the base class for all manager in Block Builder project.
    /// Inherit this in order to easily handle all derived managers.
    /// </summary>
    public abstract class BaseBuilderManager : MonoBehaviour
    {
        private IGridManager gridManager;
        
        private BlockManager blockManager;

        /// <summary>
        /// For cache BlockManager during editing in Unity Editor
        /// </summary>
        private static BlockManager blockManagerInstance;

        public virtual void EnableManager()
        {
            enabled = true;
        }

        public virtual void DisableManager()
        {
            enabled = false;
        }

        public virtual void Init()
        {
            gridManager = FindObjectOfType<BlockManager>();
            
            blockManager  = (BlockManager)gridManager;
        }

        public void InjectBlockManager(BlockManager _blockManager)
        {
            blockManager = _blockManager;
        }

        protected BlockManager GetBlockManager()
        {
            if (Application.isPlaying)
                return blockManager;

            //------Need to separate editor code vs gameplay code more properly//
            if (!blockManagerInstance)
                blockManagerInstance = FindObjectOfType<BlockManager>();
            
            if(!blockManagerInstance)
                Debug.LogWarning($"Cannot find BlockManager.");
            
            return blockManagerInstance;
        }
    }
}
