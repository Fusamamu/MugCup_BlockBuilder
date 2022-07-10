using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MugCup_BlockBuilder.Runtime;
using MugCup_PathFinder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public class BlockGeneric<T> : Block where T : Block
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

                if (_block is T)
                {
                    Debug.Log($"Cast block : {_block.name} : {_block.GetType()}");
                }
            }

            castBlocks = _castBlocks.ToArray();
            
            // Debug.Log($"Cast Blocks Count : {castBlocks.Length}");
            // Debug.Log($"Path Block Count : {_castBlocks.Where(_b => _b != null).ToList().Count}");
            // Debug.Log($"NodePosition: {NodePosition}");
           
            
            TopCastBlocks    = GridUtility.GetTopSectionNodesFrom3x3Cube   (NodePosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
            MiddleCastBlocks = GridUtility.GetMiddleSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();
            BottomCastBlocks = GridUtility.GetBottomSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _castBlocks.ToArray()).ToArray();

            foreach (var _b in MiddleCastBlocks)
            {
                if(_b == null) continue;
                
                Debug.Log($"{_b.name} : ");
            }
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
