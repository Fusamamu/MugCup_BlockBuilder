using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core;
using UnityEngine;

namespace BlockBuilder.Runtime.Core
{
	//This represents UNIT GRID//
    public class GridUnit
    {
        private int row, column, height;

        private Vector3Int gridSize = new Vector3Int();
        private Vector3    origin   = new Vector3();
		
        private Block[] blocks;
        
        public GridUnit(Vector3Int _gridSize)
        {
	        gridSize = _gridSize;
	        
	        row      = _gridSize.x;
	        column   = _gridSize.z;
	        height   = _gridSize.y;

	        blocks   = new Block[row * column * height];
        }

        public GridUnit(GridDataSettingSO _gridData)
        {
	        gridSize = _gridData.GridUnitSize;

	        row    = _gridData.UnitRow;
	        column = _gridData.UnitColumn;
	        height = _gridData.UnitHeight;
	        
	        blocks   = new Block[row * column * height];
        }

        public Block this[int i, int j] 
        {
	        get => blocks[0];
	        set
	        {
		        blocks[0] = value;
	        }
        }

        public void GetAllAdjacentBlocks()
        {
	        
        }
    }
}
