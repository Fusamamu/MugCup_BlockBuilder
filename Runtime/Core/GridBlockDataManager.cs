using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BlockBuilder.Scriptable;
using BlockBuilder.Core.Scriptable;
using MugCup_PathFinder.Runtime;
using Unity.Collections;

namespace MugCup_BlockBuilder.Runtime.Core
{
    //Per stage / Per Scene / Save/Load Scene?
    [Serializable]
    public class GridBlockDataManager : MonoBehaviour
    {
        [field:SerializeField] public GridNodeData GridNodeData { get; private set; }
        
#region Get Grid Data/BlockMesh Data
        [field:ReadOnly, SerializeField] public GridDataSettingSO GridDataSetting { get; private set; }
        [field:ReadOnly, SerializeField] public BlockMeshData     BlockMeshData   { get; private set; }
        [field:ReadOnly, SerializeField] public BlockMeshData     PathMeshData    { get; private set; }

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

#region Initialize 
        public void DefaultInitialized()
        {
            GridDataSettingSO _gridDataSetting = null;
            BlockMeshData     _meshDataSetting = null;

            _gridDataSetting = GridDataSetting;
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

            if (GridDataSetting == null)
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
                _blockDataSetting.GridDataSetting          = GridDataSetting;
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

            if (GridDataSetting == null)
                Debug.LogWarning($"GridBlockDataManager Initialized Failed. Missing Grid Data Setting.");
        }

        public void CacheData(BlockDataSetting _blockDataSetting)
        {
            GridDataSetting      = _blockDataSetting.GridDataSetting;
            BlockMeshData = _blockDataSetting.BlockMeshDataSetting;
            PathMeshData  = _blockDataSetting.PathBlockMeshDataSetting;
        }
        
        private void CacheData(ref GridDataSettingSO _gridData, ref BlockMeshData _meshData)
        {
            LoadGridDataSetting(ref _gridData);
            LoadMeshBlocksData (ref _meshData);
        }
        
        private void LoadMeshBlocksData (ref BlockMeshData     _meshData) => BlockMeshData = _meshData;
        private void LoadGridDataSetting(ref GridDataSettingSO _gridData) => GridDataSetting      = _gridData;
#endregion
        
#region Initialize Grid Unit Size 
        public void InitializeGridUnitSize(GridDataSettingSO _gridData)
        {
            //InitializeGridUnitSize(_gridData.GridUnitSize.x, _gridData.GridUnitSize.z, _gridData.GridUnitSize.y);
        }
#endregion
        
        public bool TryGetGridDataSetting(out GridDataSettingSO _data)
        {
            if (GridDataSetting == null) 
            {
                Debug.Log($"<color=red>[Warning] : </color> Missing Grid Data. Try Loading from Resource Folder.");
                GridDataSetting = Resources.Load<GridDataSettingSO>("BlockBuilder/Setting/GridData/Setting");
            }
            
            _data = GridDataSetting;
            
            if (_data == null) 
            {
                Debug.Log($"<color=red>[Warning] : </color> Have not loaded Grid Data Setting");
                return false;
            }
            
            return true;
        }
        
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
