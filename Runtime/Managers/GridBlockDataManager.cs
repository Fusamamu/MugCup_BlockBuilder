using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BlockBuilder.Scriptable;
using BlockBuilder.Core.Scriptable;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_PathFinder.Runtime;
using Unity.Collections;

namespace MugCup_BlockBuilder
{
    //Per stage / Per Scene / Save/Load Scene?
    public class GridBlockDataManager : MonoBehaviour
    {
        [field: SerializeField] public GridNodeData GridNodeData { get; private set; }
        
#region Get Grid Data/BlockMesh Data
        [field:ReadOnly, SerializeField] public GridDataSettingSO GridDataSetting { get; private set; }
        [field:ReadOnly, SerializeField] public BlockMeshData     BlockMeshData   { get; private set; }
        [field:ReadOnly, SerializeField] public BlockMeshData     PathMeshData    { get; private set; }

        private static Dictionary<Type, BlockMeshData> blockMeshDataTable = new Dictionary<Type, BlockMeshData>();

        public GridBlockDataManager SetGridDataSetting(GridDataSettingSO _gridDataSetting)
        {
            GridDataSetting = _gridDataSetting;
            return this;
        }

        public GridBlockDataManager SetBlockMeshData(BlockMeshData _blockMeshData)
        {
            BlockMeshData = _blockMeshData;
            return this;
        }

        public GridBlockDataManager SetPathMeshData(BlockMeshData _pathMeshData)
        {
            PathMeshData = _pathMeshData;
            return this;
        }

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

        public void Initialized()
        {
            if (GridDataSetting == null || BlockMeshData == null)
            {
                //These can be used for fallback. If cannot find anything
                GridDataSetting = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>(DataPath.GridDataSettingPath);
                BlockMeshData   = AssetDatabase.LoadAssetAtPath<BlockMeshData>    (DataPath.DefaultMeshBlockDataPath);
                
                Debug.Log($"Cannot find any data. Fallback tos default setting in AssetDatabase.");
            }
            
            //Need Fixe
            //InitializeGridUnitSize(_gridData); 
        }
       
        //Need Fix
        public void InitializeBlocksData(BlockManager _blockManager)
        {
            // foreach (Block _block in AvailableNodes<Block>())
            //     _block.InjectDependency(_blockManager);
            //
            // foreach (Block _block in AvailableNodes<Block>())
            // {
            //     if (_block == null) continue;
            //     
            //     _block.GetSurroundingBlocksReference();
            //     _block.SetBitMask();
            // }
        }
    }
}
