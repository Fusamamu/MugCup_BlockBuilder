using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_PathFinder.Runtime;
using MugCup_Utilities.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core
{
	/// <summary>
	/// Class to keep track of blocks when blocks get added, removed and updated.
	/// This class modify blocks data [Block Array] via GridBlockDataManager.
	/// </summary>
    public class BlockManager : MonoBehaviour, IBlockManager
    {
	    [SerializeField] private GridBlockDataManager gridBlockDataManager;
	    
	    private const string TextParentName  = "[-------Grid Position Text-------]";
	    private const string BlockParentName = "[-------------Blocks-------------]";

	    public void Initialized()
	    {
		    gridBlockDataManager = FindObjectOfType<GridBlockDataManager>();
		    gridBlockDataManager.Initialized();
	    }

	    public void InitializeWith(GridDataSettingSO _gridDataSetting, BlockMeshData _meshDataSetting)
	    {
		    gridBlockDataManager = FindObjectOfType<GridBlockDataManager>();
		    gridBlockDataManager.InitializeWith(_gridDataSetting, _meshDataSetting);
	    }

	    public void GenerateGridBlocks()
	    {
		    var _gridBlocks   = gridBlockDataManager.GridUnitIBlocks;
		    var _gridUnitSize = gridBlockDataManager.GridUnitSize;
		    var _gridLevel    = 0;
		    var _blockPrefab  = AssetManager.AssetCollection.DefualtBlock.gameObject;

		    GridBlockGenerator.PopulateGridIBlocksByLevel<Block>(_gridBlocks, _gridUnitSize, _gridLevel, _blockPrefab);

		    gridBlockDataManager.InitializeBlocksData(); //-->1 Same

		    gridBlockDataManager.AvailableBlocksApplyAll(UpdateMeshBlock);
			
		    gridBlockDataManager.AvailableBlocksApplyAll(_block => //--2 Same
		    {
			    _block.GetSurroundingIBlocksReference();
			    _block.SetBitMask();
		    });
		    
		    CreateTextOverlay  ();
		    GroupBlocksToParent();
	    }
	    
        public void UpdateMeshBlocks(IEnumerable<Block> _blocks)
		{
			foreach (var _block in _blocks)
				UpdateMeshBlock(_block);
		}
		
		public void UpdateMeshBlock(Block _block)
		{
			Vector3Int _targetNodePos = _block.NodePosition;
				
			Block _blockPrefab = gridBlockDataManager.GetBlockMeshData().GetBlockPrefab(_block.BitMask);
				
			if(_blockPrefab == null) return;
				
			RemoveBlock(_targetNodePos);
			AddBlock   (_blockPrefab, _targetNodePos);
		}

#region Add/Remove Block by Node Position
		public void AddBlock(Block _prefab, Vector3Int _nodePos)
		{
			if (IsOccupied(_nodePos)) return;
			
			var _targetNodeWorldPos = gridBlockDataManager.GetGridDataSetting().GetGridWorldNodePosition(_nodePos);
			
			var _newBlock = Instantiate(_prefab, _targetNodeWorldPos, _prefab.transform.localRotation);
			_newBlock.Init(_targetNodeWorldPos, _nodePos);
			
			AddIBlockRef(_newBlock, _nodePos);
		}

		public void RemoveBlock(Vector3Int _nodePos)
		{
			if (!IsOccupied(_nodePos)) return;
			
			DestroyIBlockObject(_nodePos);
			RemoveIBlockRef    (_nodePos);
		}
#endregion
		
		public bool IsOccupied(Vector3Int _nodePos)
		{
			if (GetIBlock(_nodePos) != null) {
				Debug.Log($"<color=red>[Warning] : </color> Grid at position {_nodePos} is occupied");
				return true;
			}

			Debug.Log($"<color=yellow>[Info] : </color> Grid at position {_nodePos} is empty");
			return false;
		}
		
		public void DestroyIBlockObject(Vector3Int _nodePos)
		{
			var _blockToBeRemoved = GetIBlock(_nodePos);
			Destroy(_blockToBeRemoved.gameObject);
		}
		
		public void AddIBlockRef(IBlock _newBlock, Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GridUnitIBlocks;
			
			GridUtility.AddNode(_newBlock, _nodePos, _gridUnitSize, ref _gridUnitIBlocks);
		}
		
		public void RemoveIBlockRef(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GridUnitIBlocks;
			
			GridUtility.RemoveNode(_nodePos, _gridUnitSize, ref _gridUnitIBlocks);
		}

#region Get Blocks
		public Block GetBlock(Vector3Int _nodePos) => GetIBlock(_nodePos) as Block;
		
		public T GetBlock<T>(Vector3Int _nodePos) where T : IBlock
		{
			return (T)GetIBlock(_nodePos);
		}
#endregion

#region Get IBlocks
		public IBlock GetIBlock(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GridUnitIBlocks;
			
			return GridUtility.GetNode(_nodePos, _gridUnitSize, _gridUnitIBlocks);
		}

		public List<IBlock> GetIBlocks(Vector3Int _startPos, Vector3Int _endPos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GridUnitIBlocks;
			
			return GridUtility.GetNodesRectArea(_startPos, _endPos, _gridUnitSize, _gridUnitIBlocks);
		}

		public List<IBlock> GetIBlocks3x3Cube(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GridUnitIBlocks;
			
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
		}
#endregion
	    
	    private static void GroupBlocksToParent()
	    {
		    var _blockParent = new GameObject(BlockParentName);

		    GridBlockData.AvailableBlocksApplyAll(_block =>
		    {
			    _block.transform.SetParent(_blockParent.transform);
		    });
	    }

	    private static void CreateTextOverlay()
	    {
		    var _textParent = new GameObject(TextParentName);

		    GridBlockData.AvailableBlocksApplyAll(_block =>
		    {
			    Vector3Int _nodePos = ((INode)_block).NodePosition;

			    var _posText   = _nodePos.ToString();
			    var _targetPos = _block.transform.position + Vector3.up * 1.5f;

			    Utility.CreateWorldTextPro(_posText, _targetPos, _textParent.transform);
		    });
	    }

		private void OnDrawGizmos()
		{
			if(!Application.isPlaying) return;

			foreach (var _block in gridBlockDataManager.GridUnitIBlocks)
			{
				if (_block != null)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(_block.transform.position, 0.05f);
				}
			}
		}
    }
}
