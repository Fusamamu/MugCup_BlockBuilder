using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder.Runtime.Core
{
    //Per stage / Per Scene / Save/Load Scene?
    [Serializable]
    public class GridBlockDataManager : NodeDataBase
    {
        // [SerializeField] private NodeBase[][] map;
        // [SerializeField] private NodeBase[] gridUnitNodeBases;
        // public NodeBase[] GetGridUnitNodeBases => gridUnitNodeBases;
        //
        // private Dictionary<int, NodeBase[]> gridNodeBasesLevelTable = new Dictionary<int, NodeBase[]>();
        //
        // public int RowUnit   ;
        // public int ColumnUnit;
        // public int LevelUnit ;
        //
        // public Vector3Int MapSize;
        // public Vector3Int GridUnitSize;

// #region Get Grid Unit NodeBases
//         //This seems to get shallow reference
//         public T[] GetGridUnitArray<T>() where T : NodeBase
//         {
//             var _gridUnitArray = new T[gridUnitNodeBases.Length];
//
//             for (var _i = 0; _i < _gridUnitArray.Length; _i++)
//             {
//                 _gridUnitArray[_i] = gridUnitNodeBases[_i] as T;
//             }
//
//             return _gridUnitArray;
//         }
//
//         public IEnumerable<T> AvailableNodes<T>() where T : NodeBase
//         {
//             foreach (var _node in gridUnitNodeBases)
//             {
//                 if(_node == null) continue;
//
//                 if (_node is T _block)
//                     yield return _block;
//             }
//         }
// #endregion
 
#region Get Grid Data/BlockMesh Data
        [field:SerializeField] public GridDataSettingSO GridData      { get; private set; }
        [field:SerializeField] public BlockMeshData     BlockMeshData { get; private set; }
        [field:SerializeField] public BlockMeshData     PathMeshData  { get; private set; }
         
        private bool GRID_DATA_INIT = false;
        private bool GRID_SIZE_INIT = false;

        private static Dictionary<Type, BlockMeshData> blockMeshDataTable = new Dictionary<Type, BlockMeshData>();

        public BlockMeshData GetBlockMeshData<T>() where T : Block
        {
            var _blockMeshDataTable = GetBlockMeshDataTable();

            if (_blockMeshDataTable != null)
            {
                if (_blockMeshDataTable.TryGetValue(typeof(T), out var _meshData))
                {
                    return _meshData;
                }
            }
            
            return null;
        }

        private Dictionary<Type, BlockMeshData> GetBlockMeshDataTable()
        {
            InitializeBlockMeshDataTable();

            return blockMeshDataTable;
        }

        private void InitializeBlockMeshDataTable()
        {
            blockMeshDataTable = new Dictionary<Type, BlockMeshData>()
            {
                { typeof(Block), BlockMeshData },
                { typeof(Path) , PathMeshData  }
            };
        }
#endregion

#region Initialize GridUnitSize Overloads
        public void DefaultInitialized()
        {
            GridDataSettingSO _gridDataSetting = null;
            BlockMeshData     _meshDataSetting = null;

            _gridDataSetting = GridData;
            _meshDataSetting = BlockMeshData;
            
            #if UNITY_EDITOR
            if (_gridDataSetting == null || _meshDataSetting == null)
            {
                //These can be used for fallback. If cannot find anything
                _gridDataSetting = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>(DataPath.GridDataSettingPath);
                _meshDataSetting = AssetDatabase.LoadAssetAtPath<BlockMeshData>    (DataPath.DefaultMeshBlockDataPath);
                
                Debug.Log($"Cannot find any data. Fallback tos default setting in AssetDatabase.");
            }
            #endif
            
            CacheData(ref _gridDataSetting, ref _meshDataSetting);
            
            if(!TryGetGridDataSetting(out var _gridData)) return;
            
            InitializeGridUnitSize(_gridData); 
            
            Debug.Log($"GridBlockDataManager Initialized.");

            if (GridData == null)
            {
                Debug.LogWarning($"GridBlockDataManager Initialized Failed. Missing Grid Data Setting.");
            }
        }
        
        public void InitializedWith(BlockDataSetting _blockDataSetting)
        {
            //Right now there are 3 ways to get grid n mesh Data
            //1 from AssetDatabase
            //2 from ref serialized field in this class
            //3 from BlockBuilder Manager

            GridDataSettingSO _gridDataSetting = _blockDataSetting.GridDataSetting;
            BlockMeshData     _meshDataSetting = _blockDataSetting.BlockMeshDataSetting;
            BlockMeshData _pathMeshDataSetting = _blockDataSetting.PathBlockMeshDataSetting;

            if (_gridDataSetting == null || _meshDataSetting == null)
            {
                _blockDataSetting.GridDataSetting          = GridData;
                _blockDataSetting.BlockMeshDataSetting     = BlockMeshData;
                _blockDataSetting.PathBlockMeshDataSetting = PathMeshData;
                
                Debug.Log($"Cannot find data from Block Builder Manager. Fallback to data in GridBlockDataManager.");
            }
            
            #if UNITY_EDITOR
            if (_gridDataSetting == null || _meshDataSetting == null)
            {
                //These can be used for fallback. If cannot find anything
                _blockDataSetting.GridDataSetting          = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>(DataPath.GridDataSettingPath);
                _blockDataSetting.BlockMeshDataSetting     = AssetDatabase.LoadAssetAtPath<BlockMeshData>    (DataPath.DefaultMeshBlockDataPath);
                //_blockDataSetting.PathBlockMeshDataSetting = AssetDatabase.LoadAssetAtPath<BlockMeshData>    (DataPath.DefaultMeshBlockDataPath);
                
                Debug.Log($"Cannot find any data. Fallback tos default setting in AssetDatabase.");
            }
            #endif
            
            CacheData(_blockDataSetting);
            
            if(!TryGetGridDataSetting(out var _gridData)) return;
            
            InitializeGridUnitSize(_gridData);
            
            Debug.Log($"GridBlockDataManager Initialized.");

            if (GridData == null)
                Debug.LogWarning($"GridBlockDataManager Initialized Failed. Missing Grid Data Setting.");
        }

        private void CacheData(BlockDataSetting _blockDataSetting)
        {
            GridData      = _blockDataSetting.GridDataSetting;
            
            BlockMeshData = _blockDataSetting.BlockMeshDataSetting;
            PathMeshData  = _blockDataSetting.PathBlockMeshDataSetting;
        }
        
        private void CacheData(ref GridDataSettingSO _gridData, ref BlockMeshData _meshData)
        {
            LoadGridDataSetting(ref _gridData);
            LoadMeshBlocksData (ref _meshData);
        }
        
        private void LoadMeshBlocksData (ref BlockMeshData     _meshData) => BlockMeshData = _meshData;
        private void LoadGridDataSetting(ref GridDataSettingSO _gridData) => GridData      = _gridData;

        
#region Initialize Grid Unit Size OverLoads
        public void InitializeGridUnitSize(GridDataSettingSO _gridData)
        {
            InitializeGridUnitSize(_gridData.GridUnitSize.x, _gridData.GridUnitSize.z, _gridData.GridUnitSize.y);
        }
        
        public void InitializeGridUnitSize(Vector3Int _gridSize)
        {
            InitializeGridUnitSize(_gridSize.x, _gridSize.z, _gridSize.y);
        }

        public void InitializeGridUnitSize(int _row, int _column, int _height)
        {
            RowUnit    = _row;
            ColumnUnit = _column;
            LevelUnit  = _height;

            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT = true;
        }
#endregion
        
        // public void InitializeMapSize(int _row, int _column, int _height)
        // {
        //     map     = new NodeBase[_row * _column * _height][];
        //     MapSize = new Vector3Int(_row, _height, _column);
        // }
        //
        // public void InitializeGridArray()
        // { 
        //     int _rowUnit    = GridData.GridUnitSize.x;
        //     int _columnUnit = GridData.GridUnitSize.z;
        //     int _levelUnit  = GridData.GridUnitSize.y;
        //
        //     Vector3Int _gridUnitSize = GridData.GridUnitSize;
        //     
        //     gridUnitNodeBases = new NodeBase[_rowUnit * _columnUnit * _levelUnit];
        //
        //     for (int _y = 0; _y < _levelUnit ; _y++)
        //     for (int _x = 0; _x < _rowUnit   ; _x++)
        //     for (int _z = 0; _z < _columnUnit; _z++)
        //         gridUnitNodeBases[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)] = null;
        // }
        //
        // public void ClearGridUnitNodeBases()
        // {
        //     gridUnitNodeBases = null;
        // }
#endregion
        
        public bool TryGetGridDataSetting(out GridDataSettingSO _data)
        {
            if (GridData == null) 
            {
                Debug.Log($"<color=red>[Warning] : </color> Missing Grid Data. Try Loading from Resource Folder.");
                GridData = Resources.Load<GridDataSettingSO>("BlockBuilder/Setting/GridData/Setting");
            }
            
            _data = GridData;
            
            if (_data == null) 
            {
                Debug.Log($"<color=red>[Warning] : </color> Have not loaded Grid Data Setting");
                return false;
            }
            
            return true;
        }

        // public void ApplyAllNodes<T>(Action<T> _action) where T : NodeBase
        // {
        //     foreach (var _node in AvailableNodes<T>())
        //     {
        //         _action?.Invoke(_node);
        //     }
        // }

        public void AvailableBlocksApplyAll(Action<Block> _action)
        {
            foreach (Block _block in AvailableNodes<Block>())
                _action?.Invoke(_block);
        }

        //Where to push it? BlockManager or This GridBlockDataManager.
        // public void PopulateGridBlocksByLevel(int _gridLevel)
        // {
        //     var _blockPrefab  = AssetManager.AssetCollection.DefualtBlock.gameObject;
        //     
        //     GridBlockGenerator.PopulateGridBlocksByLevel<Block>(gridUnitNodeBases, GridUnitSize, _gridLevel, _blockPrefab);
        //
        //     var _selectedBlockLevel = GetAllNodeBasesAtLevel<NodeBase>(_gridLevel);
        //
        //     if(!gridNodeBasesLevelTable.ContainsKey(_gridLevel))
        //         gridNodeBasesLevelTable.Add(_gridLevel, _selectedBlockLevel);
        //     else
        //         gridNodeBasesLevelTable[_gridLevel] = _selectedBlockLevel;
        // }
        //
        // public T[] GetAllNodeBasesAtLevel<T>(int _gridLevel) where T : NodeBase
        // {
        //     int _rowUnit    = GridUnitSize.x;
        //     int _columnUnit = GridUnitSize.z;
        //     int _heightUnit = GridUnitSize.y;
        //     
        //     var _selectedBlockLevel = new T[_rowUnit * _columnUnit];
        //     
        //     for (var _x = 0; _x < _rowUnit   ; _x++)
        //     for (var _z = 0; _z < _columnUnit; _z++)
        //         _selectedBlockLevel[_z + _rowUnit * _x] = gridUnitNodeBases[_z + _rowUnit * (_x + _heightUnit * _gridLevel)] as T;
        //
        //     return _selectedBlockLevel;
        // }
        
        public void InitializeBlocksData(BlockManager _blockManager)
        {
            foreach (Block _block in AvailableNodes<Block>())
                _block.InjectDependency(_blockManager);
            
            foreach (Block _block in AvailableNodes<Block>())
            {
                if (_block == null) continue;
                
                _block.GetSurroundingBlocksReference();
                _block.SetBitMask();
                    
                Debug.Log("Initializing a Block");
            }
        }
    }
}
