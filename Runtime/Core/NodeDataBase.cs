using System;
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Runtime.Core;
using UnityEngine;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public class NodeDataBase : MonoBehaviour
    {
        [SerializeField] private NodeBase[][] map;
        [SerializeField] private NodeBase[] gridUnitNodeBases;
        public NodeBase[] GetGridUnitNodeBases => gridUnitNodeBases;
        
        private Dictionary<int, NodeBase[]> gridNodeBasesLevelTable = new Dictionary<int, NodeBase[]>();

        public int RowUnit   ;
        public int ColumnUnit;
        public int LevelUnit ;
        
        public Vector3Int MapSize;
        public Vector3Int GridUnitSize;
        
        
#region Get Grid Unit NodeBases
        //This seems to get shallow reference
        public T[] GetGridUnitArray<T>() where T : NodeBase
        {
            var _gridUnitArray = new T[gridUnitNodeBases.Length];

            for (var _i = 0; _i < _gridUnitArray.Length; _i++)
            {
                _gridUnitArray[_i] = gridUnitNodeBases[_i] as T;
            }

            return _gridUnitArray;
        }

        public IEnumerable<T> AvailableNodes<T>() where T : NodeBase
        {
            foreach (var _node in gridUnitNodeBases)
            {
                if(_node == null) continue;

                if (_node is T _block)
                    yield return _block;
            }
        }
#endregion
        
        public void InitializeMapSize(int _row, int _column, int _height)
        {
            map     = new NodeBase[_row * _column * _height][];
            MapSize = new Vector3Int(_row, _height, _column);
        }
        
        public void InitializeGridArray()
        { 
            int _rowUnit    = GridUnitSize.x;
            int _columnUnit = GridUnitSize.z;
            int _levelUnit  = GridUnitSize.y;

            Vector3Int _gridUnitSize = GridUnitSize;
            
            gridUnitNodeBases = new NodeBase[_rowUnit * _columnUnit * _levelUnit];

            for (int _y = 0; _y < _levelUnit ; _y++)
            for (int _x = 0; _x < _rowUnit   ; _x++)
            for (int _z = 0; _z < _columnUnit; _z++)
                gridUnitNodeBases[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)] = null;
        }
        
        public void ClearGridUnitNodeBases()
        {
            gridUnitNodeBases = null;
        }
        
        public void ApplyAllNodes<T>(Action<T> _action) where T : NodeBase
        {
            foreach (var _node in AvailableNodes<T>())
            {
                _action?.Invoke(_node);
            }
        }
        
        public void PopulateGridBlocksByLevel(int _gridLevel)
        {
            var _blockPrefab  = AssetManager.AssetCollection.DefualtBlock.gameObject;
            
            GridBlockGenerator.PopulateGridBlocksByLevel<Block>(gridUnitNodeBases, GridUnitSize, _gridLevel, _blockPrefab);

            var _selectedBlockLevel = GetAllNodeBasesAtLevel<NodeBase>(_gridLevel);

            if(!gridNodeBasesLevelTable.ContainsKey(_gridLevel))
                gridNodeBasesLevelTable.Add(_gridLevel, _selectedBlockLevel);
            else
                gridNodeBasesLevelTable[_gridLevel] = _selectedBlockLevel;
        }
        
        public T[] GetAllNodeBasesAtLevel<T>(int _gridLevel) where T : NodeBase
        {
            int _rowUnit    = GridUnitSize.x;
            int _columnUnit = GridUnitSize.z;
            int _heightUnit = GridUnitSize.y;
            
            var _selectedBlockLevel = new T[_rowUnit * _columnUnit];
            
            for (var _x = 0; _x < _rowUnit   ; _x++)
            for (var _z = 0; _z < _columnUnit; _z++)
                _selectedBlockLevel[_z + _rowUnit * _x] = gridUnitNodeBases[_z + _rowUnit * (_x + _heightUnit * _gridLevel)] as T;

            return _selectedBlockLevel;
        }
    }
}
