using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Runtime.Core;
using MugCup_PathFinder.Runtime;
using UnityEditor;

namespace MugCup_BlockBuilder.Runtime.Core
{
    //Will remove
    public class NodeDataBase : MonoBehaviour
    {
        //For transfer old data to this 
        [field:SerializeField] public GridNodeData GridNodeData { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(GridNodeData  == null) return;
            if(GridUnitNodes == null) return;

            GridNodeData.GridNodes = GridUnitNodes;
            GridNodeData.GridSize  = GridUnitSize;
            
            EditorUtility.SetDirty(GridNodeData);
        }
#endif
        //Will Clear node data below!!

        [field:SerializeField] public GridNode[][] MapNode       { get; private set; }
        [field:SerializeField] public GridNode[]   GridUnitNodes { get; private set; }
        
        private Dictionary<int, GridNode[]> levelTable = new Dictionary<int, GridNode[]>();

        public int RowUnit   ;
        public int ColumnUnit;
        public int LevelUnit ;
        
        public Vector3Int MapSize;
        public Vector3Int GridUnitSize;
        
        private bool GRID_DATA_INIT = false;
        private bool GRID_SIZE_INIT = false;
        
        public NodeDataBase InitializeGridUnitSize(Vector3Int _gridSize)
        {
            InitializeGridUnitSize(_gridSize.x, _gridSize.z, _gridSize.y);
            return this;
        }

        public NodeDataBase InitializeGridUnitSize(int _row, int _column, int _height)
        {
            RowUnit    = _row;
            ColumnUnit = _column;
            LevelUnit  = _height;

            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT = true;

            return this;
        }
        
        public NodeDataBase InitializeMapSize(int _row, int _column, int _height)
        {
            MapNode = new GridNode[_row * _column * _height][];
            MapSize = new Vector3Int(_row, _height, _column);

            return this;
        }
        //Duplicate code
        public NodeDataBase InitializeGridArray()
        { 
            int _rowUnit    = GridUnitSize.x;
            int _columnUnit = GridUnitSize.z;
            int _levelUnit  = GridUnitSize.y;

            Vector3Int _gridUnitSize = GridUnitSize;
            
            GridUnitNodes = new GridNode[_rowUnit * _columnUnit * _levelUnit];

            for (int _y = 0; _y < _levelUnit ; _y++)
            for (int _x = 0; _x < _rowUnit   ; _x++)
            for (int _z = 0; _z < _columnUnit; _z++)
                GridUnitNodes[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)] = null;

            return this;
        }
        
        
        
        
        
        
        //Try to move to GridBuilder
        #if UNITY_EDITOR
        public void PopulateGridBlocksByLevel(int _level)
        {
            var _blockPrefab  = AssetManager.AssetCollection.DefaultBlock.gameObject;
            
            GridGenerator.PopulateGridBlocksByLevel<Block>(GridUnitNodes, GridUnitSize, _level, _blockPrefab);

            var _selectedBlockLevel = GetAllNodeBasesAtLevel<GridNode>(_level);
            
            GridNodeData.GridNodes = GridUnitNodes;
            GridNodeData.GridSize  = GridUnitSize;

            if(!levelTable.ContainsKey(_level))
                levelTable.Add(_level, _selectedBlockLevel);
            else
                levelTable[_level] = _selectedBlockLevel;
        }
        #endif
        
        
        
        
        
        
        
        
        //Already try to move to  GridData Class
        //Duplicate code
        public T GetNode<T>(Vector3Int _nodePos) where T : GridNode
        {
            return GridUtility.GetNode(_nodePos, GridUnitSize, GridUnitNodes) as T;
        }
        //Duplicate code
        public void AddNode<T>(T _newNode, Vector3Int _nodePos) where T : GridNode
        {
            var _gridUnitNodeBases = GridUnitNodes;
            GridUtility.AddNode(_newNode, _nodePos, GridUnitSize, ref _gridUnitNodeBases);
        }
        //Duplicate code
        public void RemoveNode<T>(Vector3Int _nodePos) where T : GridNode
        {
            var _gridUnitNodeBases = GridUnitNodes;
            GridUtility.RemoveNode(_nodePos, GridUnitSize, ref _gridUnitNodeBases);
        }
        //------------------------------------------
        
#region Get Grid Unit NodeBases
        //This seems to get shallow reference
        public T[] GetGridUnitArray<T>() where T : GridNode
        {
            var _gridUnitArray = new T[GridUnitNodes.Length];

            for (var _i = 0; _i < _gridUnitArray.Length; _i++)
            {
                _gridUnitArray[_i] = GridUnitNodes[_i] as T;
            }

            return _gridUnitArray;
        }

        //move to gridData
        public IEnumerable<T> AvailableNodes<T>() where T : GridNode
        {
            foreach (var _node in GridUnitNodes)
            {
                if(_node == null) continue;

                if (_node is T _block)
                    yield return _block;
            }
        }
        
        //Must replace by GridUtility GetNodesByLevel by need time to test
        public T[] GetAllNodeBasesAtLevel<T>(int _gridLevel) where T : GridNode
        {
            int _rowUnit    = GridUnitSize.x;
            int _columnUnit = GridUnitSize.z;
            int _heightUnit = GridUnitSize.y;
            
            var _selectedBlockLevel = new T[_rowUnit * _columnUnit];
            
            for (var _x = 0; _x < _rowUnit   ; _x++)
            for (var _z = 0; _z < _columnUnit; _z++)
                _selectedBlockLevel[_z + _rowUnit * _x] = GridUnitNodes[_z + _rowUnit * (_x + _heightUnit * _gridLevel)] as T;

            return _selectedBlockLevel;
        }
#endregion
        
        //Move to grid Data
        public void ApplyAllNodes<T>(Action<T> _action) where T : GridNode
        {
            foreach (var _node in AvailableNodes<T>())
            {
                _action?.Invoke(_node);
            }
        }

        //?????
        public void StoreGridUnitNode(GridNode[] _nodeBases)
        {
            GridUnitNodes = _nodeBases;
        }

        
        //Moved to GridData
        public void EmptyGridUnitNodeBases()
        {
            for (var _i = 0; _i < GridUnitNodes.Length; _i++)
            {
                GridUnitNodes[_i] = null;
            }
        }
        
        //Move to GridData
        public void ClearGridUnitNodeBases()
        {
            GridUnitNodes = null;
        }
    }
}
