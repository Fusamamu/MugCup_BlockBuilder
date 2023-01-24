using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_Utilities.Runtime.DesignPattern.StateMachine;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public enum BuilderMode
    {
        EditMode,
        HandlerMode,
    }

    public enum ModeEvent
    {
        EnterEditMode,
        EnterSelectionMode
    }

    public class BlockBuilderFSM : MC_StateMachine<BuilderMode, BuilderMode, ModeEvent>
    {
            
    }

    public class StateManager : BaseBuilderManager
    {
        private BlockBuilderFSM fsm;
        
        private void Start()
        {
            fsm = new BlockBuilderFSM();
            
            fsm.AddState(BuilderMode.EditMode,      new BlockEditState     (false));
            fsm.AddState(BuilderMode.HandlerMode,   new BlockHandlerState(false));
            //fsm.AddState(BuilderMode.HandlerMode,   new BlockEditState     (false));
            
            fsm.SetStartState(BuilderMode.EditMode);
            fsm.Init();
        }

        private void Update()
        {
            fsm.OnUpdate();
        }

        public void RequestChangeState(BuilderMode _mode)
        {
            fsm.RequestStateChange(_mode);
        }
    }
}
