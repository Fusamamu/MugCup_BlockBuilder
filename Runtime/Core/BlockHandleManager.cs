using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_BlockBuilder.Runtime.Core.Managers;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public class BlockHandleManager : BaseBuilderManager
    {
        //[SerializeField] private Block currentSelectedBlock;
        //[SerializeField] private List<Block> allCurrentSelectedBlocks = new List<Block>();

#region Dependencies
	    private BlockSelectionManager blockSelectionManager;
        private PointerVisualizer     pointerVisualizer;
        private IBlockRaycaster       gridBlockSelection;
        private IInputManager         inputManager;
#endregion

        public override void Init()
        {
            base.Init();
		    
            pointerVisualizer  = FindObjectOfType<PointerVisualizer>();
            gridBlockSelection = FindObjectOfType<GridBlockSelection>();
            inputManager       = FindObjectOfType<InputManager>();
        }

        private void OnEnable()
        {
            if (!blockSelectionManager)
            {
                if (blockBuilderManager is BlockBuilderManager _blockBuilderManager)
                {
                    blockSelectionManager = _blockBuilderManager.GetManager<BlockSelectionManager>();
                }
            }
        }

        private void OnDisable()
        {
            blockSelectionManager = null;
        }

        private void Update()
        {
            if (inputManager.CheckLeftMouseDown() && blockSelectionManager.HasCurrentSelected())
            {
                
            }
        }
    }
}
