using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_PathFinder.Runtime;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core
{
    //Per stage / Per Scene / Save/Load Scene?
    [Serializable]
    public class GridBlockDataManager : MonoBehaviour
    {
        [SerializeField] private NodeBase[][] map;
        [SerializeField] private NodeBase[] gridUnitNodeBases;
        
        private Dictionary<int, NodeBase[]> gridNodeBasesLevelTable = new Dictionary<int, NodeBase[]>();

        public int RowUnit   ;
        public int ColumnUnit;
        public int LevelUnit ;
        
        public Vector3Int MapSize;
        public Vector3Int GridUnitSize;

#region Get Grid Unit NodeBases
        public NodeBase[] GetGridUnitNodeBases => gridUnitNodeBases;

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
        
        public IEnumerable<Block> GetAvailableBlocks()
        {
            var _blocks = new List<Block>();
            
            foreach (var _node in gridUnitNodeBases)
            {
                if(_node == null) continue;
                
                if(_node is Block _block)
                    _blocks.Add(_block);
            }

            return _blocks;
        }
#endregion
 
#region Get Grid Data/BlockMesh Data
        public GridDataSettingSO GetGridDataSetting() => gridData;
        public BlockMeshData     GetBlockMeshData  () => blockMeshData;
        public BlockMeshData     GetPathMeshData   () => pathMeshData;

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
                { typeof(Block)    , blockMeshData },
                { typeof(Path), pathMeshData  }
            };
        }
#endregion
        
        //These can be used as fallback. In case, data cannot be found from BuildBuilderManager//
        //Or User use to use this class directly.
        [SerializeField] private GridDataSettingSO gridData;
        [SerializeField] private BlockMeshData     blockMeshData;
        [SerializeField] private BlockMeshData     pathMeshData;
         
        private bool GRID_DATA_INIT = false;
        private bool GRID_SIZE_INIT = false;

        public void ClearGridUnitNodeBases()
        {
            gridUnitNodeBases = null;
        }
        
        public void InitializeMapSize(int _row, int _column, int _height)
        {
            map     = new NodeBase[_row * _column * _height][];
            MapSize = new Vector3Int(_row, _height, _column);
        }

#region Initialize GridUnitSize Overloads
        public void DefaultInitialized()
        {
            GridDataSettingSO _gridDataSetting = null;
            BlockMeshData     _meshDataSetting = null;

            _gridDataSetting = gridData;
            _meshDataSetting = blockMeshData;
            
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

            if (gridData == null)
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
                _blockDataSetting.GridDataSetting          = gridData;
                _blockDataSetting.BlockMeshDataSetting     = blockMeshData;
                _blockDataSetting.PathBlockMeshDataSetting = pathMeshData;
                
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

            if (gridData == null)
                Debug.LogWarning($"GridBlockDataManager Initialized Failed. Missing Grid Data Setting.");
        }

        private void CacheData(BlockDataSetting _blockDataSetting)
        {
            gridData      = _blockDataSetting.GridDataSetting;
            
            blockMeshData = _blockDataSetting.BlockMeshDataSetting;
            pathMeshData  = _blockDataSetting.PathBlockMeshDataSetting;
        }
        
        private void CacheData(ref GridDataSettingSO _gridData, ref BlockMeshData _meshData)
        {
            LoadGridDataSetting(ref _gridData);
            LoadMeshBlocksData (ref _meshData);
        }
        
        private void LoadMeshBlocksData (ref BlockMeshData     _meshData) => blockMeshData = _meshData;
        private void LoadGridDataSetting(ref GridDataSettingSO _gridData) => gridData = _gridData;

        
#region Initialize Grid Unit Size OverLoads
        public void InitializeGridUnitSize(int _row, int _column, int _height)
        {
            RowUnit    = _row;
            ColumnUnit = _column;
            LevelUnit  = _height;

            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT = true;
        }
        
        public void InitializeGridUnitSize(Vector3Int _gridSize)
        {
            RowUnit    = _gridSize.x;
            ColumnUnit = _gridSize.z;
            LevelUnit  = _gridSize.y;
        
            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT = true;
        }

        public void InitializeGridUnitSize(GridDataSettingSO _gridData)
        {
            RowUnit    = _gridData.GridUnitSize.x;
            ColumnUnit = _gridData.GridUnitSize.z;
            LevelUnit  = _gridData.GridUnitSize.y;
        
            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT = true;
        }
#endregion
        
        public void InitializeGridArray()
        { 
            int _rowUnit    = gridData.GridUnitSize.x;
            int _columnUnit = gridData.GridUnitSize.z;
            int _levelUnit  = gridData.GridUnitSize.y;

            Vector3Int _gridUnitSize = gridData.GridUnitSize;
            
            gridUnitNodeBases = new NodeBase[_rowUnit * _columnUnit * _levelUnit];

            for (int _y = 0; _y < _levelUnit ; _y++)
            for (int _x = 0; _x < _rowUnit   ; _x++)
            for (int _z = 0; _z < _columnUnit; _z++)
                gridUnitNodeBases[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)] = null;
        }
#endregion
        
        public bool TryGetGridDataSetting(out GridDataSettingSO _data)
        {
            if (gridData == null) 
            {
                Debug.Log($"<color=red>[Warning] : </color> Missing Grid Data. Try Loading from Resource Folder.");
                gridData = Resources.Load<GridDataSettingSO>("BlockBuilder/Setting/GridData/Setting");
            }
            
            _data = gridData;
            
            if (_data == null) 
            {
                Debug.Log($"<color=red>[Warning] : </color> Have not loaded Grid Data Setting");
                return false;
            }
            
            return true;
        }

        public void AvailableBlocksApplyAll(Action<Block> _action)
        {
            Block[] _blocks = GetAvailableBlocks().ToArray();
            
            foreach (Block _block in _blocks)
                _action?.Invoke(_block);
        }

        //Where to push it? BlockManager or This GridBlockDataManager.
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
            
            var _selectedBlockLevel = new T[_rowUnit * _columnUnit];
            
            for (var _x = 0; _x < _rowUnit; _x++)
            for (var _z = 0; _z < _columnUnit; _z++)
                _selectedBlockLevel[_z + GridUnitSize.x * _x] = gridUnitNodeBases[_z + GridUnitSize.x * (_x + GridUnitSize.y * _gridLevel)] as T;

            return _selectedBlockLevel;
        }
        
        public void InitializeBlocksData(BlockManager _blockManager)
        {
            foreach (Block _block in GetAvailableBlocks())
                _block.InjectDependency(_blockManager);
            
            foreach (Block _block in GetAvailableBlocks())
            {
                if (_block == null) continue;
                
                _block.GetSurroundingBlocksReference();
                _block.SetBitMask();
                    
                Debug.Log("Initializing a Block");
            }
        }
    }
}
