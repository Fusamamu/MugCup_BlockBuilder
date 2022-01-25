using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
//using MugCup_BlockBuilder.Editor;
using UnityEditor;

namespace BlockBuilder.Runtime.Core
{
    public static class GridBlockData
    {
        public static IBlock[][] Map;
        public static IBlock[] GridUnitIBlocks;
        
        public static int RowUnit;
        public static int ColumnUnit;
        public static int LevelUnit;
        
        public static Vector3Int MapSize;
        public static Vector3Int GridUnitSize;

        public static void LoadGridBlocksData (ref IBlock[] _blockData)         => GridUnitIBlocks = _blockData;
        public static void LoadGridDataSetting(ref GridDataSettingSO _gridData) => gridData = _gridData;
        public static void LoadMeshBlocksData (ref BlockMeshData _meshData)     => meshData = _meshData;

        public static IBlock[] GetBlocks() => GridUnitIBlocks;
        public static IEnumerable<IBlock> GetAvailableIBlocks() => GridUnitIBlocks.Where(_iBlock => _iBlock != null);
        public static IEnumerable<Block>  GetAvailableBlocks()  => GridUnitIBlocks.Where(_iBlock => _iBlock != null).Select(_iBlock => _iBlock as Block);

        public static GridDataSettingSO GetGridDataSetting() => gridData;
        public static BlockMeshData     GetBlockMeshData()   => meshData;

        private static GridDataSettingSO gridData;
        private static BlockMeshData     meshData;
        
        private static bool GRID_DATA_INIT = false;
        private static bool GRID_SIZE_INIT = false;
        
        public static void InitializeMapSize(int _row, int _column, int _height)
        {
            Map     = new IBlock[_row * _column * _height][];
            MapSize = new Vector3Int(_row, _height, _column);
        }

#region Initialize GridUnitSize Overloads
        public static void Initialized()
        {
            var _gridDataSetting = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>(DataPath.GridDataSettingPath     );
            var _meshDataSetting = AssetDatabase.LoadAssetAtPath<BlockMeshData>    (DataPath.DefaultMeshBlockDataPath);
            
            CacheData(ref _gridDataSetting, ref _meshDataSetting);
            
            if(!TryGetGridDataSetting(out var _gridData)) return;
            
            InitializeGridUnitSize(_gridData);
            InitializeGridArray();
        }

        public static void InitializeWith(GridDataSettingSO _gridDataSetting, BlockMeshData _meshDataSetting)
        {
            CacheData(ref _gridDataSetting, ref _meshDataSetting);
            
            if(!TryGetGridDataSetting(out var _gridData)) return;
            
            InitializeGridUnitSize(_gridData);
            InitializeGridArray();
        }
        
        public static void CacheData(ref GridDataSettingSO _gridData, ref BlockMeshData _meshData)
        {
            LoadGridDataSetting(ref _gridData);
            LoadMeshBlocksData (ref _meshData);
        }
        
        public static void InitializeGridUnitSize(int _row, int _column, int _height)
        {
            RowUnit    = _row;
            ColumnUnit = _column;
            LevelUnit  = _height;

            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT = true;
        }
        
        public static void InitializeGridUnitSize(Vector3Int _gridSize)
        {
            RowUnit    = _gridSize.x;
            ColumnUnit = _gridSize.z;
            LevelUnit  = _gridSize.y;
        
            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT = true;
        }

        public static void InitializeGridUnitSize(GridDataSettingSO _gridData)
        {
            RowUnit    = _gridData.GridUnitSize.x;
            ColumnUnit = _gridData.GridUnitSize.z;
            LevelUnit  = _gridData.GridUnitSize.y;
        
            GridUnitSize = new Vector3Int(RowUnit, LevelUnit, ColumnUnit);
            
            GRID_SIZE_INIT= true;
        }
        
        public static void InitializeGridArray()
        { 
            int RowUnit    = gridData.GridUnitSize.x;
            int ColumnUnit = gridData.GridUnitSize.z;
            int LevelUnit  = gridData.GridUnitSize.y;

            Vector3Int GridUnitSize = gridData.GridUnitSize;
            
            GridUnitIBlocks = new IBlock[RowUnit * ColumnUnit * LevelUnit];

            for (int y = 0; y < LevelUnit; y++)
            for (int x = 0; x < RowUnit; x++)
            for (int z = 0; z < ColumnUnit; z++)
                GridUnitIBlocks[z + GridUnitSize.x * (x + GridUnitSize.y * y)] = null;
        }
#endregion
        
        public static bool TryGetGridDataSetting(out GridDataSettingSO _data)
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

        public static void AvailableBlocksApplyAll(Action<Block> _action)
        {
            Block[] _blocks = GetAvailableBlocks().ToArray();
            foreach (Block _block in _blocks)
                _action?.Invoke(_block);
        }
        
        public static void InitializeBlocksData()
        {
            foreach (IBlock _block in GetAvailableIBlocks())
            {
                Block _checkedBlock = _block as Block;

                if (_checkedBlock != null)
                {
                    _checkedBlock.GetSurroundingIBlocksReference();
                    _checkedBlock.SetBitMask();
                }
            }
        }
        
        // public static void InitializeIBlocksData<T>(IEnumerable<IBlock> _blocks) where T : Block
        // {
        //     foreach (IBlock _block in _blocks)
        //     {
        //         T _checkedBlock = _block as T;
        //
        //         if (_checkedBlock != null)
        //         {
        //             _checkedBlock.GetSurroundingIBlocksReference();
        //             _checkedBlock.SetBitMask();
        //         }
        //     }
        // }
    }
}
