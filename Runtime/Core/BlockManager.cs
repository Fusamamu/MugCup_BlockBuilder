using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_PathFinder.Runtime;
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

	    public void GetBlockDataManager()
	    {
		    //Need to find way to get and init GridBlockDataManaer
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
