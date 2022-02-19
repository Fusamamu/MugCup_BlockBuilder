using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Managers;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public class BlockSelectionManager : MonoBehaviour
    {
        [SerializeField] private Block currentSelectedBlock;

        [SerializeField] private List<Block> allCurrentSelectedBlocks = new List<Block>();

#region Dependencies
        private BlockBuilderManager blockBuilderManager;
        private PointerVisualizer   pointerVisualizer;
        private IBlockRaycaster     gridBlockSelection;
        private IInputManager       inputManager;
#endregion
        
        private void Awake()
        {
            blockBuilderManager = BlockBuilderManager.Instance;
            
             pointerVisualizer  = FindObjectOfType<PointerVisualizer>();
             gridBlockSelection = FindObjectOfType<GridBlockSelection>();
             inputManager       = FindObjectOfType<InputManager>();
        }
        //Need to use Game State Pattern to toggle between Selection Mode and Edit Mode//
        //Need to create State Machine in Utility Class//
        
        //BlockSelectionManager Functionalities//
        //Be able to Select a Block
			//Click Selection
			//Rect Box Selection 
	    //Move Selected Block/Blocks
	    //When Block Selected
			//Visual FeedBack
				//Outline Box
				//Change Box's Color

	    public void SelectBlock(Block _block)
	    {
		    
	    }
    }
}
