using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace BlockBuilder.Runtime.Core
{
    [System.Serializable]
    public static class GridBlockGenerator
    {
        public static NormalFace SelectedFace;

        public static void PopulateGridIBlocksByLevel<T>(IBlock[] _blocks, Vector3Int _gridUnitSize, int _heightLevel, GameObject _blockPrefab, GameObject _parent = null) where T : Block
        {
            int _rowUnit    = _gridUnitSize.x;
            int _columnUnit = _gridUnitSize.z;

            for (int x = 0; x < _rowUnit; x++)
            {
                for (int z = 0; z < _columnUnit; z++)
                {
                    Vector3Int _targetNodePos = new Vector3Int(x, _heightLevel, z);
                    
                    T _block = Object.Instantiate(_blockPrefab, _targetNodePos, Quaternion.identity).AddComponent<T>();
                    
                    /*TODO : Must Find way to get GridWorldNodePosition*/
                    // Vector3    _targetNodeWorldPos = Vector3.zero;
                    //
                    // if (GridBlockData.TryGetGridDataSetting(out var _gridData))
                    //     _targetNodeWorldPos = _gridData.GetGridWorldNodePosition(_targetNodePos);//---> Just Need Function
                    //
                    // Block _block = Object.Instantiate(_blockPrefab, _targetNodeWorldPos, Quaternion.identity).AddComponent<Block>();
                        
                    if (_parent != null)
                    {
                        _block.transform.position += _parent.transform.position;
                        _block.transform.SetParent(_parent.transform);
                    }
                    
                    _block.Init(_targetNodePos, _targetNodePos);

                    _blocks[z + _gridUnitSize.x * (x + _gridUnitSize.y * _heightLevel)] = _block;
                }
            }
        }

        public static Block[][] GenerateMap(Vector3Int _mapSize, Vector3Int _unitSize)
        {
            GameObject _mainMap = new GameObject("Main Map");
            
            int _mapRow     = _mapSize.x;
            int _mapColumn  = _mapSize.z;
            int _mapLevel   = _mapSize.y;
            
            Block[][] _map  = new Block[_mapRow * _mapColumn * _mapLevel][];
            
            GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            for (int y = 0; y < _mapLevel; y++)
            {
                for (int x = 0; x < _mapRow; x++)
                {
                    for (int z = 0; z < _mapColumn; z++)
                    {
                        GameObject _unit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
                        float _posX = x * _unitSize.x;
                        float _posY = y * _unitSize.y;
                        float _posZ = z * _unitSize.z;
                        
                        _unit.transform.position = new Vector3(_posX, _posY, _posZ);
                        _unit.transform.SetParent(_mainMap.transform);
                        _unit.name = $"Unit: {x}, {y}, {z}";
                        
                        _map[x + _mapSize.x * (z + _mapSize.z * y)] = GenerateGridBlocks(_unitSize, _blockPrefab, _unit);
                    }
                }
            }
            
            Object.DestroyImmediate(_blockPrefab);

            return _map;
        }
        
        public static Block[] GenerateGridBlocks(Vector3Int _gridUnitSize, GameObject _blockPrefab, GameObject _parent = null)
        {
             int _rowUnit    = _gridUnitSize.x;
             int _columnUnit = _gridUnitSize.z;
             int _levelUnit  = _gridUnitSize.y;
            
             Block[] _blocks = new Block[_rowUnit * _columnUnit * _levelUnit];
            
             for (int y = 0; y < _levelUnit; y++)
             {
                 for (int x = 0; x < _rowUnit; x++)
                 {
                     for (int z = 0; z < _columnUnit; z++)
                     {
                         Vector3 _position = new Vector3(x, y, z);
            
                         Block _block = Object.Instantiate(_blockPrefab, _position, Quaternion.identity).AddComponent<Block>();
                         
                         if (_parent != null)
                         {
                             _block.transform.position += _parent.transform.position;
                             _block.transform.SetParent(_parent.transform);
                         }
                     
                         _block.Init(_block.transform.position, new Vector3Int(x, y, z));
                         
                         _blocks[z + _gridUnitSize.x * (x + _gridUnitSize.y * y)] = _block;
                     }
                 }
             }

             return _blocks;
        }
       
        public static void AddBlock(Vector3 _position, Transform _parent, GameObject _blockPrefab)
        {
            GameObject _block = Object.Instantiate(_blockPrefab, _position, Quaternion.identity, _parent);
        }
    }
}
