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
        protected IBlockManager blockBuilderManager;

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
            blockBuilderManager = BlockBuilderManager.Instance;
        }
    }
}
