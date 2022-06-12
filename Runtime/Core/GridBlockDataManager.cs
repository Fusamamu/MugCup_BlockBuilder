using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core
{
    //Per stage / Per Scene / Save/Load Scene?
    public class GridBlockDataManager : MonoBehaviour
    {
        [SerializeField] private Block[][] map;
        [SerializeField] private Block[]   gridUnitBlocks;
        
        public int RowUnit;
        public int ColumnUnit;
        public int LevelUnit;
        
        public Vector3Int MapSize;
        public Vector3Int GridUnitSize;

        public void LoadGridBlocksData(Block[] _blockData) => gridUnitBlocks = _blockData;
        
        public void LoadGridDataSetting(ref GridDataSettingSO _gridData) => gridData = _gridData;
        public void LoadMeshBlocksData (ref BlockMeshData     _meshData) => meshData = _meshData;

        public Block[] GetGridUnitBlocks() => gridUnitBlocks;

        public IEnumerable<Block> GetAvailableBlocks() => gridUnitBlocks.Where(_iBlock => _iBlock != null);

        public GridDataSettingSO GetGridDataSetting() => gridData;
        public BlockMeshData     GetBlockMeshData  () => meshData;

        private  GridDataSettingSO gridData;
        private  BlockMeshData     meshData;
        
        private  bool GRID_DATA_INIT = false;
        private  bool GRID_SIZE_INIT = false;
        
        public void InitializeMapSize(int _row, int _column, int _height)
        {
            map     = new Block[_row * _column * _height][];
            MapSize = new Vector3Int(_row, _height, _column);
        }

#region Initialize GridUnitSize Overloads
        public void Initialized()
        {
            var _gridDataSetting = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>(DataPath.GridDataSettingPath     );
            var _meshDataSetting = AssetDatabase.LoadAssetAtPath<BlockMeshData>    (DataPath.DefaultMeshBlockDataPath);
            
            CacheData(ref _gridDataSetting, ref _meshDataSetting);
            
            if(!TryGetGridDataSetting(out var _gridData)) return;
            
            InitializeGridUnitSize(_gridData);
            InitializeGridArray();
        }

        public void InitializeWith(GridDataSettingSO _gridDataSetting, BlockMeshData _meshDataSetting)
        {
            CacheData(ref _gridDataSetting, ref _meshDataSetting);
            
            if(!TryGetGridDataSetting(out var _gridData)) return;
            
            InitializeGridUnitSize(_gridData);
            InitializeGridArray();
        }
        
        public void CacheData(ref GridDataSettingSO _gridData, ref BlockMeshData _meshData)
        {
            LoadGridDataSetting(ref _gridData);
            LoadMeshBlocksData (ref _meshData);
        }
        
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
        
        public void InitializeGridArray()
        { 
            int _rowUnit    = gridData.GridUnitSize.x;
            int _columnUnit = gridData.GridUnitSize.z;
            int _levelUnit  = gridData.GridUnitSize.y;

            Vector3Int _gridUnitSize = gridData.GridUnitSize;
            
            gridUnitBlocks = new Block[_rowUnit * _columnUnit * _levelUnit];

            for (int _y = 0; _y < _levelUnit ; _y++)
            for (int _x = 0; _x < _rowUnit   ; _x++)
            for (int _z = 0; _z < _columnUnit; _z++)
                gridUnitBlocks[_z + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)] = null;
        }
#endregion
        
        public bool TryGetGridDataSetting(out GridDataSettingSO _data)
        {
            if (gridData == null) {
                Debug.Log($"<color=red>[Warning] : </color> Missing Grid Data. Try Loading from Resource Folder.");
                gridData = Resources.Load<GridDataSettingSO>("BlockBuilder/Setting/GridData/Setting");
            }
            
            _data = gridData;
            
            if (_data == null) {
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
        
        public  void InitializeBlocksData()
        {
            foreach (Block _block in GetAvailableBlocks())
            {
                Block _checkedBlock = _block;

                if (_checkedBlock != null)
                {
                    _checkedBlock.GetSurroundingIBlocksReference();
                    _checkedBlock.SetBitMask();
                }
            }
        }
    }
}
