using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_Utilities.Runtime.DesignPattern.StateMachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public class BlockHandlerState : StateBase<BuilderMode>
    {
        private readonly BaseBuilderManager blockEditorManager; 
        
        private Func<BlockHandlerState, bool> canExit;

        public BlockHandlerState(bool _needsExitTime) : base(_needsExitTime)
        {
            blockEditorManager = Object.FindObjectOfType<BlockSelectionManager>();
        }

        public override void Init()
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Enter Block Handler State");
            blockEditorManager.EnableManager();
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            blockEditorManager.DisableManager();
        }

        public override void RequestExit()
        {
            if (!NeedsExitTime || canExit != null && canExit(this))
            {
                FSM.StateCanExit();
            }
        }
    }
}
