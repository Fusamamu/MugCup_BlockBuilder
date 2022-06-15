using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core;

namespace BlockBuilder.Core.Scriptable
{
    public struct BlockMeshInfo
    {
        public Block Prefab;
        public Quaternion Rotation;
    }
    
    [CreateAssetMenu(fileName = "BlockMeshData", menuName = "ScriptableObjects/BlockMeshDataObject", order = 6)]
    public class BlockMeshData : ScriptableObject
    {
        [Header("Isolated")]
        public Block IsolatedBlock;
        
        [Header("Top Surface")]
        public Block TopSurfaceBlock;

        [Header("Side")]
        public Block Side;
        [Header("Corner")]
        public Block Corner;
        [Header("I Shape")]
        public Block IShape;
        [Header("Connect One Side")]
        public Block ConnectOneSide;
        

        [Header("Sides")]
        public Block SideW;
        public Block SideN;
        public Block SideE;
        public Block SideS;

        [Header("Corners")]
        public Block CornerNW;
        public Block CornerNE;
        public Block CornerSW;
        public Block CornerSE;

        [Header("Connect 1 Side")]
        public Block Side_1_W;
        public Block Side_1_N;
        public Block Side_1_S;
        public Block Side_1_E;

        [Header("Connect 4 Side - Plus Shape")]
        public Block Side_4_PlusShape;

        [Header("Connect 2 Sides - I Shape")]
        public Block IShape_WtoE;
        public Block IShape_StoN;

        [Header("Connect 2 Side - L Shape")]
        public Block LShape_NW;
        public Block LShape_NE;
        public Block LShape_SE;
        public Block LShape_SW;

        public Block GetDefaultBlock()
        {
            return IsolatedBlock;
        }

        public BlockMeshInfo GetBlockPrefabMiddleSection(int _bitMask)
        {
            BlockMeshInfo _blockMeshInfo = new BlockMeshInfo();
            
            switch (_bitMask)
            {
                case 0b_111_111_111:

                    _blockMeshInfo = new BlockMeshInfo
                    {
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                    
                case 0b_000_011_000:
                    
                    _blockMeshInfo = new BlockMeshInfo
                    {
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                // case 0b_010_010_000:
                //     return ConnectOneSide;
                // case 0b_010_011_000:
                //     return Corner;
                // case 0b_011_011_000:
                //     return Corner;
                // case 0b_000_010_010:
                //     return ConnectOneSide;
                // case 0b_000_011_010:
                //     return Corner;
                // case 0b_000_011_011:
                //     return Corner;
                // case 0b_010_010_010:
                //     return IShape;
                // case 0b_010_011_010:
                //     return Side;
                // case 0b_011_011_010:
                //     return Side;
                // case 0b_010_011_011:
                //     return Side;
                // case 0b_011_011_011:
                //     return Side;
                // case 0b_000_110_000:
                //     return ConnectOneSide;
                // case 0b_000_111_000:
                //     return null;
                // case 0b_010_110_000:
                //     return null;
                // case 0b_010_111_000:
                //     return null;
                // case 0b_011_111_000:
                //     return null;
                // case 0b_000_110_010:
                //     return null;
                // case 0b_000_111_010:
                //     return null;
                // case 0b_000_111_011:
                //     return null;
                // case 0b_010_110_010:
                //     return null;
                // case 0b_010_111_010:
                //     return null;
                // case 0b_011_111_110:
                //     return null;
                // case 0b_010_111_011:
                //     return null;
                // case 0b_011_111_011:
                //     return null;
                // case 0b_110_110_000:
                //     return null;
                // case 0b_110_111_000:
                //     return null;
                // case 0b_111_111_000:
                //     return null;
                // case 0b_110_110_010:
                //     return null;
                // case 0b_110_111_011:
                //     return null;
                // case 0b_111_111_010:
                //     return null;
                // // case 0b_111_111_11:
                // //     return 32;
                // case 0b_111_111_011:
                //     return null;
                // case 0b_000_110_110:
                //     return null;
                // case 0b_000_111_110:
                //     return null;
                // case 0b_000_111_111:
                //     return null;
                // case 0b_010_110_110:
                //     return null;
                // case 0b_010_111_110:
                //     return null;
                // // case 0b_011_111_110:
                // //     return 39;
                // case 0b_010_111_111:
                //     return null;
                // case 0b_011_111_111:
                //     return null;
                // case 0b_110_110_110:
                //     return null;
                // case 0b_110_111_110:
                //     return null;
                // case 0b_111_111_110:
                //     return null;
                // case 0b_110_111_111:
                //     return null;
                // // case 0b_111_111_111:
                // //     return 46;
                // case 0b_000_010_000:
                //     return null;
                default:
                    _blockMeshInfo = new BlockMeshInfo
                    {
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
            }

            return _blockMeshInfo;
        }

        public Block GetBlockPrefab(int _bitMask)
        {
            if (_bitMask == 0b_000000000_000000000_000000000)
                return null;

            if (_bitMask == 0b_000000000_000010000_000000000)
                return IsolatedBlock;

            if (_bitMask == 0b_000000000_111111111_000000000)
                return TopSurfaceBlock;

            
#region Corners
            if (_bitMask == 0b_000000000_000110110_000000000)
                return CornerNW;

            if (_bitMask == 0b_000000000_110110000_000000000)
                return CornerNE;
            
            if (_bitMask == 0b_000000000_000011011_000000000)
                return CornerSW;
            
            if (_bitMask == 0b_000000000_011011000_000000000)
                return CornerSE;
#endregion

#region Sides
            if (_bitMask == 0b_000000000_000111111_000000000)
                return SideW;
            
            if (_bitMask == 0b_000000000_110110110_000000000)
                return SideN;
            
            if (_bitMask == 0b_000000000_111111000_000000000)
                return SideE;

            if (_bitMask == 0b_000000000_011011011_000000000)
                return SideS;
#endregion
            
#region Connect 1 Side
            bool _middle = Occupy_Middle (_bitMask);
            bool _south  = Connect_South (_bitMask);
            bool _west   = Connect_West  (_bitMask);
            bool _east   = Connect_East  (_bitMask);
            bool _north  = Connect_North (_bitMask);

            bool _se = Connect_SE(_bitMask);
             
            if (_middle && !_south && !_west &&  _north && !_east)
                return Side_1_S;
            if (_middle &&  _south && !_west && !_north && !_east)
                return Side_1_N;
            if (_middle && !_south && !_west && !_north &&  _east)
                return Side_1_W;
            if (_middle && !_south &&  _west && !_north && !_east)
                return Side_1_E;
#endregion

            if (_bitMask == 0b_000000000_010111010_000000000)
                return Side_4_PlusShape;

            if (_middle &&  _south && !_west &&  _north && !_east)
                return IShape_StoN;

            if (_middle && !_south &&  _west && !_north &&  _east)
                return IShape_WtoE;

            if (_middle &&  _south && !_west && !_north && _east && !_se)
                return LShape_NW;

            return null;
        }

        private bool Occupy_Middle (int _bitMask) => (_bitMask & (1 << 13)) == 1 << 13;
        
        private bool Connect_South (int _bitMask) => (_bitMask & (1 << 14)) == 1 << 14;
        private bool Connect_West  (int _bitMask) => (_bitMask & (1 << 16)) == 1 << 16;
        private bool Connect_North (int _bitMask) => (_bitMask & (1 << 12)) == 1 << 12;
        private bool Connect_East  (int _bitMask) => (_bitMask & (1 << 10)) == 1 << 10;

        private bool Connect_SE    (int _bitMask) => (_bitMask & (1 << 11)) == 1 << 11;

        private bool Check (int _bitMask)
        {
            bool _nullWest  = (_bitMask & (1 << 16)) == 0;
            bool _nullSouth = (_bitMask & (1 << 14)) == 0;
            bool _hasMiddle = (_bitMask & (1 << 13)) == (1 << 13);
            bool _hasNorth  = (_bitMask & (1 << 12)) == (1 << 12);
            bool _nullEast  = (_bitMask & (1 << 10)) == 0;

            return _nullWest && _nullSouth && _hasMiddle && _hasNorth && _nullEast;
        }
    }
}
