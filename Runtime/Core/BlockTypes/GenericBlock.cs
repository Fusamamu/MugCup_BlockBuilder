using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MugCup_BlockBuilder.Runtime;
using MugCup_PathFinder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public class GenericBlock<T> : Block where T : Block
    {
        [SerializeField] protected T[] castBlocks;
        
        public T[] TopCastBlocks    = new T[9];
        public T[] MiddleCastBlocks = new T[9];
        public T[] BottomCastBlocks = new T[9];
        
        // public override void UpdateBlockData()
        // {
        //     base.UpdateBlockData();
        //     
        //     GetSurroundingBlocksReference();
        //     SetBitMask();
        // }

        public override void GetSurroundingBlocksReference()
        {
            //May not needed
            //base.GetSurroundingBlocksReference();
            
            if(!IsGridDataInit()) return;
            
            var _castBlocks = new List<T>();

            foreach (var _block in gridNodeBases)
            {
                var _castBlock = _block as T;
                
                _castBlocks.Add(_castBlock);
            }

            castBlocks = _castBlocks.ToArray();
           
            
            TopCastBlocks    = GridUtility.GetTopSectionNodesFrom3x3Cube   (NodePosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
            MiddleCastBlocks = GridUtility.GetMiddleSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
            BottomCastBlocks = GridUtility.GetBottomSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
        }
        
        public override void SetBitMask()
        {
            //base.SetBitMask();
            
            BitMask = 0b_000000000_000000000_000000000;
            
            int _startBit = 0b_100000000_000000000_000000000;

            foreach (var _block in TopCastBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in MiddleCastBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in BottomCastBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
        }
    }
}
