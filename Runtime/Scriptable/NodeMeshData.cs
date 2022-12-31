using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_PathFinder.Runtime;

namespace BlockBuilder.Core.Scriptable
{
    public struct NodeMeshInfo<T> where T : GridNode
    {
        public bool IsValid;
        
        public T Prefab;
        public Quaternion Rotation;
    }
    
    [CreateAssetMenu(fileName = "NodeBaseData", menuName = "ScriptableObjects/Node Mesh Data", order = 7)]
    public class NodeMeshData<T> : ScriptableObject where T : GridNode
    {
        [Header("Data Setting")]
        public bool IsUseComposite;
        
        [Header("Isolated")]
        public T IsolatedBlock;
        
        [Header("Top Surface")]
        public T TopSurfaceBlock;

        [Header("Side")]
        public T Side;
        public T SideComposite;
        [Header("Corner")]
        public T Corner;
        public T CornerComposite;
        [Header("I Shape")]
        public T IShape;
        public T IShapeComposite;
        
        
        [Header("Connect One Side")]
        public T ConnectOneSide;
        public T ConnectOneSideComposite;
        [Header("Connect Three Sides")]
        public T ConnectTwoSides;
        public T ConnectTwoSidesComposite;
        [Header("Connect Three Sides")]
        public T ConnectThreeSides;
        public T ConnectThreeSidesComposite;
        [Header("Connect Four Sides")]
        public T ConnectFourSides;
        public T ConnectFourSidesComposite;

        public T GetDefaultBlock()
        {
            return IsolatedBlock;
        }
        
        public NodeMeshData<T> SetUseComposite(bool _value)
        {
            IsUseComposite = _value;
            return this;
        }

        public NodeMeshInfo<T> GetBlockPrefabMiddleSection(int _bitMask)
        {
            var _nodeMeshInfo = new NodeMeshInfo<T>();

            int _westBit  = (_bitMask >> 7) & 0b_000_000_001;
            int _southBit = (_bitMask >> 5) & 0b_000_000_001;
            int _northBit = (_bitMask >> 3) & 0b_000_000_001;
            int _eastBit  = (_bitMask >> 1) & 0b_000_000_001;

            if (_westBit == 0 || _southBit == 0)
            {
                _bitMask &= 0b_011_111_111; //Remove SW Bit
            }

            if (_westBit == 0 || _northBit == 0)
            {
                _bitMask &= 0b_110_111_111;  //Remove NW Bit
            }
            
            if (_eastBit == 0 || _northBit == 0)
            {
                _bitMask &= 0b_111_111_110;  //Remove NE Bit
            }
            
            if (_eastBit == 0 || _southBit == 0)
            {
                _bitMask &= 0b_111_111_011;  //Remove SE Bit
            }
            
            switch (_bitMask)
            {
                case 0b_111_111_111:

                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = IsUseComposite ? null : TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                
                case 0b_000_011_000:
                    /*
                    * 0  1  0
                    * 0  1  0
                    * 0  0  0
                    */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = ConnectOneSide,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                
                case 0b_010_010_000:
                    
                    /*
                     * 0  0  0
                     * 1  1  0
                     * 0  0  0
                     */
                    
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = ConnectOneSide,
                        Rotation = Quaternion.identity
                    };
                    break;
                
                case 0b_010_011_000:
                    
                    /*
                     * 0  1  0
                     * 1  1  0
                     * 0  0  0
                     */
                    
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Corner,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                
                case 0b_011_011_000:
                    /*
                    * 1  1  0
                    * 1  1  0
                    * 0  0  0
                    */
                    
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Corner,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                 
                case 0b_000_010_010: 
                    /*
                    * 0  0  0
                    * 0  1  1
                    * 0  0  0
                    */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = ConnectOneSide,
                        Rotation = Quaternion.Euler(0, 180, 0)
                    };
                    break;
                
                 
                case 0b_000_011_010:
                     /*
                     * 0  1  0
                     * 0  1  1
                     * 0  0  0
                     */
                     _nodeMeshInfo = new NodeMeshInfo<T>
                     {
                         IsValid  = true,
                         Prefab   = Corner,
                         Rotation = Quaternion.Euler(0, 180, 0)
                     };
                     break;
                
                case 0b_000_011_011:
                    /*
                     * 0  1  1
                     * 0  1  1
                     * 0  0  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Corner,
                        Rotation = Quaternion.Euler(0, 180, 0)
                    };
                    break;
                
                case 0b_010_010_010:
                    
                    /*
                     * 0  0  0
                     * 1  1  1
                     * 0  0  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = IsUseComposite ? IShapeComposite : IShape,
                        Rotation = Quaternion.identity
                    };
                    break;
                
                
                case 0b_010_011_010:

                    /*
                     * 0  1  0
                     * 1  1  1
                     * 0  0  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = ConnectThreeSides,
                        Rotation = Quaternion.Euler(0, 0, 0)
                    };
                    break;
                
                case 0b_011_011_010:
                    
                    /*
                     * 1  1  0
                     * 1  1  1
                     * 0  0  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 0, 0)
                    };
                    break;
                
                case 0b_010_011_011:
                    /*
                     * 0  1  1
                     * 1  1  1
                     * 0  0  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 0, 0)
                    };
                    break;
                
                case 0b_011_011_011:
                    /*
                     * 1  1  1
                     * 1  1  1
                     * 0  0  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 0, 0)
                    };
                    break;
                
                case 0b_000_110_000:
                    /*
                     * 0  0  0
                     * 0  1  0
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = ConnectOneSide,
                        Rotation = Quaternion.Euler(0, 270, 0)
                    };
                    break;
                case 0b_000_111_000:
                    /*
                     * 0  1  0
                     * 0  1  0
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = IsUseComposite ? IShapeComposite : IShape,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                case 0b_010_110_000:
                    /*
                     * 0  0  0
                     * 1  1  0
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Corner,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_010_111_000:
                    /*
                     * 0  1  0
                     * 1  1  0
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = ConnectThreeSides,
                        Rotation = Quaternion.Euler(0, -90, 0)
                    };
                    break;
                case 0b_011_111_000:
                    /*
                     * 1  1  0
                     * 1  1  0
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, -90, 0)
                    };
                    break;
                case 0b_000_110_010:
                    /*
                     * 0  0  0
                     * 0  1  1
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Corner,
                        Rotation = Quaternion.Euler(0, 270, 0)
                    };
                    break;
                case 0b_000_111_010:
                    /*
                     * 0  1  0
                     * 0  1  1
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                case 0b_000_111_011:
                    /*
                     * 0  1  1
                     * 0  1  1
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                case 0b_010_110_010:
                    /*
                     * 0  0  0
                     * 1  1  1
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 180, 0)
                    };
                    break;
                case 0b_010_111_010:
                    /*
                     * 0  1  0
                     * 1  1  1
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_011_111_110:
                    /*
                     * 1  1  0
                     * 1  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_010_111_011:
                    /*
                     * 0  1  1
                     * 1  1  1
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_011_111_011:
                    /*
                     * 1  1  1
                     * 1  1  1
                     * 0  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_110_110_000:
                    /*
                     * 0  0  0
                     * 1  1  0
                     * 1  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Corner,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_110_111_000:
                    /*
                     * 0  1  0
                     * 1  1  0
                     * 1  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, -90, 0)
                    };
                    break;
                case 0b_111_111_000:
                    /*
                     * 1  1  0
                     * 1  1  0
                     * 1  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, -90, 0)
                    };
                    break;
                case 0b_110_110_010:
                    /*
                     * 0  0  0
                     * 1  1  1
                     * 1  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 180, 0)
                    };
                    break;
                case 0b_110_111_011:
                    /*
                     * 0  1  1
                     * 1  1  1
                     * 1  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_111_111_010:
                    /*
                     * 1  1  0
                     * 1  1  1
                     * 1  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                // case 0b_111_111_11:
                case 0b_111_111_011:
                    /*
                     * 1  1  1
                     * 1  1  1
                     * 1  1  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_000_110_110:
                    /*
                     * 0  0  0
                     * 0  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Corner,
                        Rotation = Quaternion.Euler(0, 270, 0)
                    };
                    break;
                case 0b_000_111_110:
                    /*
                     * 0  1  0
                     * 0  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                case 0b_000_111_111:
                    /*
                     * 0  1  1
                     * 0  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 90, 0)
                    };
                    break;
                case 0b_010_110_110:
                    /*
                     * 0  0  0
                     * 1  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 180, 0)
                    };
                    break;
                case 0b_010_111_110:
                    /*
                     * 0  1  0
                     * 1  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                // case 0b_011_111_110:
                case 0b_010_111_111:
                    /*
                     * 0  1  1
                     * 1  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_011_111_111:
                    /*
                     * 1  1  1
                     * 1  1  1
                     * 0  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_110_110_110:
                    /*
                     * 0  0  0
                     * 1  1  1
                     * 1  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = Side,
                        Rotation = Quaternion.Euler(0, 180, 0)
                    };
                    break;
                case 0b_110_111_110:
                    /*
                     * 0  1  0
                     * 1  1  1
                     * 1  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_111_111_110:
                    /*
                     * 1  1  0
                     * 1  1  1
                     * 1  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                case 0b_110_111_111:
                    /*
                     * 0  1  1
                     * 1  1  1
                     * 1  1  1
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                // case 0b_111_111_111:
                case 0b_000_010_000:
                    /*
                     * 0  0  0
                     * 0  1  0
                     * 0  0  0
                     */
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = true,
                        Prefab   = IsolatedBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
                
                // case 0b_000_011_000:
                //     /*
                //     * 0  1  0
                //     * 0  1  0
                //     * 0  0  0
                //     */
                //     _blockMeshInfo = new BlockMeshInfo
                //     {
                //         Prefab   = ConnectOneSide,
                //         Rotation = Quaternion.Euler(0, 270, 0)
                //     };
                //     break;
                
                default:
                    _nodeMeshInfo = new NodeMeshInfo<T>
                    {
                        IsValid  = false,
                        Prefab   = TopSurfaceBlock,
                        Rotation = Quaternion.identity
                    };
                    break;
            }

            return _nodeMeshInfo;
        }
    }
}
