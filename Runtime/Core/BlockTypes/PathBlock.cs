using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_PathFinder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public class PathBlock : Block
    {
        //May not needed
        [SerializeField] protected PathBlock[] pathBlocks;
        
        public PathBlock[] TopPathBlocks    = new PathBlock[9];
        public PathBlock[] MiddlePathBlocks = new PathBlock[9];
        public PathBlock[] BottomPathBlocks = new PathBlock[9];


        public override void GetSurroundingBlocksReference()
        {
            base.GetSurroundingBlocksReference();
            
            if(!IsGridDataInit()) return;
            
            var _pathBlocks = Array.ConvertAll(gridBlocks, _block => (PathBlock)_block);

            TopPathBlocks    = GridUtility.GetTopSectionNodesFrom3x3Cube   (NodePosition, gridData.GridUnitSize, _pathBlocks).ToArray();
            MiddlePathBlocks = GridUtility.GetMiddleSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _pathBlocks).ToArray();
            BottomPathBlocks = GridUtility.GetBottomSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _pathBlocks).ToArray();
        }
    }
}
