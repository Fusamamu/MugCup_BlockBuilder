using System;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Managers;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public class BlockSelectionManager : BaseBuilderManager
    {
	    //Need to check whether IBlockRaycast can use IPointerHandler in new input system or not//
	    
        [SerializeField] private Block currentSelectedBlock;

        [SerializeField] private Dictionary<int, Block> allSelectedBlocks = new Dictionary<int, Block>();

        [SerializeField] private List<Block> allCurrentSelectedBlocks = new List<Block>();

#region Dependencies
        private PointerVisualizer   pointerVisualizer;
        private IBlockRaycaster     gridBlockSelection;
        private IInputManager       inputManager;
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
		    
	    }

	    private void OnDisable()
	    {
		    
	    }

	    private void Update()
	    {
		    if (inputManager.CheckLeftMouseClicked())
		    {
			    var _hitBlock = gridBlockSelection.GetHitObject<Block>();

			    if (_hitBlock)
			    {
					SelectBlock(_hitBlock);
				    return;
			    }
			    
			    DeselectAllBlocks();
		    }
	    }

	    //Need to use Game State Pattern to toggle between Selection Mode and Edit Mode//
        //Need to create State Machine in Utility Class//
        
        //BlockSelectionManager Functionalities//
        
        //Be able to Select a Block
			//Click Selection
			//Rect Box Selection 
			
	    //Move Selected Block/Blocks -> let Handler Manager Handle this.
	    
	    //When Block Selected
			//Visual FeedBack
				//Outline Box
				//Change Box's Color

	    public Block GetCurrentSelectedBlock()
	    {
		    if (!currentSelectedBlock)
			    return currentSelectedBlock;
		    
			return null;
	    }

	    private void SelectBlock(Block _block)
	    {
		    if(HasCurrentSelected()) return;
		    
		    currentSelectedBlock = _block;

		    var _blockID = _block.GetInstanceID();

		    if (!allSelectedBlocks.ContainsKey(_blockID))
		    {
			    allSelectedBlocks.Add(_blockID, _block);
			    return;
		    }

		    allSelectedBlocks[_blockID] = _block;
	    }

	    private void DeselectAllBlocks()
	    {
		    currentSelectedBlock = null;
		    allSelectedBlocks.Clear();
	    }

	    public bool HasCurrentSelected() => currentSelectedBlock != null;

    }
}
