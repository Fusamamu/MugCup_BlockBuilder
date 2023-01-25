using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_PathFinder.Runtime;

namespace BlockBuilder.Runtime.Core
{
    [System.Serializable]
    public static class GridGenerator
    {
        public static NormalFace SelectedFace;
        
        /// <summary>
        /// Create Blocks and Store in NodeBase Array 
        /// </summary>
        public static void PopulateGridBlocksByLevel<T>(GridNode[] _nodeBases, Vector3Int _gridUnitSize, int _heightLevel, GameObject _nodeBasePrefab, GameObject _parent = null) where T : Block
        {
            int _rowUnit    = _gridUnitSize.x;
            int _columnUnit = _gridUnitSize.z;

            for (var _x = 0; _x < _rowUnit; _x++)
            {
                for (var _z = 0; _z < _columnUnit; _z++)
                {
                    var _targetNodePos = new Vector3Int(_x, _heightLevel, _z);

                    var _blockObject = Object.Instantiate(_nodeBasePrefab, _targetNodePos, Quaternion.identity);
                    
                    /*TODO : Must Find way to get GridWorldNodePosition*/
                    // Vector3    _targetNodeWorldPos = Vector3.zero;
                    //
                    // if (GridBlockData.TryGetGridDataSetting(out var _gridData))
                    //     _targetNodeWorldPos = _gridData.GetGridWorldNodePosition(_targetNodePos);//---> Just Need Function
                    //
                    // Block _block = Object.Instantiate(_blockPrefab, _targetNodeWorldPos, Quaternion.identity).AddComponent<Block>();
                        
                    if (_parent != null)
                    {
                        _blockObject.transform.position += _parent.transform.position;
                        _blockObject.transform.SetParent(_parent.transform);
                    }
                    
                    if (!_blockObject.TryGetComponent<T>(out var _block))
                        _block = _blockObject.AddComponent<T>();

                    _block
                        .SetPosition(_targetNodePos, _targetNodePos)
                        .Init();

                    _nodeBases[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _heightLevel)] = _block;
                }
            }
        }

        public static void PopulateGridIBlocksByLevel<T>(Block[] _blocks, Vector3Int _gridUnitSize, int _heightLevel, GameObject _blockPrefab, GameObject _parent = null) where T : Block
        {
            int _rowUnit    = _gridUnitSize.x;
            int _columnUnit = _gridUnitSize.z;

            for (var _x = 0; _x < _rowUnit; _x++)
            {
                for (var _z = 0; _z < _columnUnit; _z++)
                {
                    var _targetNodePos = new Vector3Int(_x, _heightLevel, _z);

                    var _blockObject = Object.Instantiate(_blockPrefab, _targetNodePos, Quaternion.identity);
                    
                    /*TODO : Must Find way to get GridWorldNodePosition*/
                    // Vector3    _targetNodeWorldPos = Vector3.zero;
                    //
                    // if (GridBlockData.TryGetGridDataSetting(out var _gridData))
                    //     _targetNodeWorldPos = _gridData.GetGridWorldNodePosition(_targetNodePos);//---> Just Need Function
                    //
                    // Block _block = Object.Instantiate(_blockPrefab, _targetNodeWorldPos, Quaternion.identity).AddComponent<Block>();
                        
                    if (_parent != null)
                    {
                        _blockObject.transform.position += _parent.transform.position;
                        _blockObject.transform.SetParent(_parent.transform);
                    }
                    
                    if (!_blockObject.TryGetComponent<T>(out var _block))
                        _block = _blockObject.AddComponent<T>();
                    
                    _block
                        .SetPosition(_targetNodePos, _targetNodePos)
                        .Init();

                    _blocks[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _heightLevel)] = _block;
                }
            }
        }

        // public static Block[][] GenerateMap(Vector3Int _mapSize, Vector3Int _unitSize)
        // {
        //     GameObject _mainMap = new GameObject("Main Map");
        //     
        //     int _mapRow     = _mapSize.x;
        //     int _mapColumn  = _mapSize.z;
        //     int _mapLevel   = _mapSize.y;
        //     
        //     Block[][] _map  = new Block[_mapRow * _mapColumn * _mapLevel][];
        //     
        //     GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //     
        //     for (int y = 0; y < _mapLevel; y++)
        //     {
        //         for (int x = 0; x < _mapRow; x++)
        //         {
        //             for (int z = 0; z < _mapColumn; z++)
        //             {
        //                 GameObject _unit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     
        //                 float _posX = x * _unitSize.x;
        //                 float _posY = y * _unitSize.y;
        //                 float _posZ = z * _unitSize.z;
        //                 
        //                 _unit.transform.position = new Vector3(_posX, _posY, _posZ);
        //                 _unit.transform.SetParent(_mainMap.transform);
        //                 _unit.name = $"Unit: {x}, {y}, {z}";
        //                 
        //                 //need to change array position calculation?
        //                 _map[x + _mapSize.x * (z + _mapSize.z * y)] = GenerateGridBlocks(_unitSize, _blockPrefab, _unit);
        //             }
        //         }
        //     }
        //     
        //     Object.DestroyImmediate(_blockPrefab);
        //
        //     return _map;
        // }
        
        public static T[] GenerateGridBlocks<T>(Vector3Int _gridUnitSize, GameObject _nodePrefab, GameObject _parent = null) where T : Component, IGridCoord
        {
             int _rowUnit    = _gridUnitSize.x;
             int _columnUnit = _gridUnitSize.z;
             int _levelUnit  = _gridUnitSize.y;
            
             T[] _gridNodes = new T[_rowUnit * _columnUnit * _levelUnit];
            
             for (int _y = 0; _y < _levelUnit; _y++)
             {
                 for (int _x = 0; _x < _rowUnit; _x++)
                 {
                     for (int _z = 0; _z < _columnUnit; _z++)
                     {
                         Vector3 _position = new Vector3(_x, _y, _z);

                         var _nodeObject = Object.Instantiate(_nodePrefab, _position, Quaternion.identity);

                         _nodeObject.name = $"GE_{_x}_{_y}_{_z}";

                         if (_parent != null)
                         {
                             _nodeObject.transform.position += _parent.transform.position;
                             _nodeObject.transform.SetParent(_parent.transform);
                         }
                     
                         if (!_nodeObject.TryGetComponent<T>(out var _node))
                             _node = _nodeObject.AddComponent<T>();

                         _node.SetNodePosition(new Vector3Int(_x, _y, _z));
                         
                         _gridNodes[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)] = _node;
                     }
                 }
             }

             return _gridNodes;
        }
       
        public static void AddBlock(Vector3 _position, Transform _parent, GameObject _blockPrefab)
        {
            GameObject _block = Object.Instantiate(_blockPrefab, _position, Quaternion.identity, _parent);
        }
    }
}
