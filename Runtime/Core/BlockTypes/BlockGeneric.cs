using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder.Runtime
{
    public class BlockGeneric<T> : Block where T : Block
    {
        [SerializeField] protected T[] castBlocks;
        
        public T[] TopCastBlocks    = new T[9];
        public T[] MiddleCastBlocks = new T[9];
        public T[] BottomCastBlocks = new T[9];
        
        public override void GetSurroundingBlocksReference()
        {
            base.GetSurroundingBlocksReference();
            
            if(!IsGridDataInit()) return;
            
            var _castBlocks = new List<T>();

            foreach (var _block in gridNodeBases)
            {
                var _castBlock = _block as T;
                
                _castBlocks.Add(_castBlock);
            }

            castBlocks = _castBlocks.ToArray();
            
            TopCastBlocks    = GridUtility.GetTopSectionNodesFrom3x3Cube   (NodeGridPosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
            MiddleCastBlocks = GridUtility.GetMiddleSectionNodesFrom3x3Cube(NodeGridPosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
            BottomCastBlocks = GridUtility.GetBottomSectionNodesFrom3x3Cube(NodeGridPosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
        }
        
        public override void SetBitMask()
        {
            base.SetBitMask();
            BitMaskComposite = base.GetBitMask();
        }

        public override int GetBitMask()
        {
            var _bitMask = 0b_000000000_000000000_000000000;
            
            int _startBit = 0b_100000000_000000000_000000000;

            foreach (var _block in TopCastBlocks)
            {
                if (_block != null)
                    _bitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in MiddleCastBlocks)
            {
                if (_block != null)
                    _bitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in BottomCastBlocks)
            {
                if (_block != null)
                    _bitMask |= _startBit;
                
                _startBit >>= 1;
            }

            return _bitMask;
        }
    }
}
