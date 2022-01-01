using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MugCup_Utilities;
using MugCup_PathFinder.Runtime;
using BlockBuilder.Scriptable;
using BlockBuilder.Core.Scriptable;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace BlockBuilder.Runtime.Core
{
	public class BlockManager : Singleton<BlockManager>, IBlockManager
	{
		[SerializeField] private GameObject blockDefaultPrefab;

		[SerializeField] private IBlock[] gridUnitIBlocks;
		
		[SerializeField] private GridDataSettingSO gridData;
		[SerializeField] private BlockMeshData     meshData;

		private List<Block> resultPath = new List<Block>();
		
		protected override void Awake()
		{
			GridBlockData.CacheData(ref gridData, ref meshData);
			GridBlockData.Initialized();
			
			GridBlockGenerator.PopulateGridIBlocksByLevel<Block>
				(GridBlockData.GridUnitIBlocks, GridBlockData.GridUnitSize, 0, blockDefaultPrefab);
			
			GridBlockData.InitializeBlocksData();
			
			gridUnitIBlocks = GridBlockData.GridUnitIBlocks; //Why have to be between these lines??  For it to work//
			
			GridBlockData.AvailableBlocksApplyAll(UpdateMeshBlock);
			GridBlockData.AvailableBlocksApplyAll(_block =>
			{
				_block.GetSurroundingIBlocksReference();
				_block.SetBitMask();
			});
			
			// Block _blockA = GetBlock(new Vector3Int(2, 0, 2));
			// Block _blockB = GetBlock(new Vector3Int(7, 0, 7));

			// AStar<Block>.InitializeGridData(gridData.GridUnitSize, gridUnitBlocks);
			// resultPath = AStar<Block>.FindPath(_blockA, _blockB).ToList();
				
			GameObject _parent = new GameObject("[-------Grid Position Text-------");
			
			foreach (IBlock _block in GridBlockData.GridUnitIBlocks)
			{
				if(_block == null) continue;
				
				Vector3Int _nodePos = ((INode)_block).NodePosition;
				string _posText = _nodePos.ToString();
				
				Vector3 _targetPos = _block.transform.position + Vector3.up * 1.5f;
				Utility.CreateWorldTextPro(_posText, _targetPos, _parent.transform);
			}
		}
		
		public void UpdateMeshBlocks(IEnumerable<Block> _blocks)
		{
			foreach (Block _block in _blocks)
				UpdateMeshBlock(_block);
		}
		
		private void UpdateMeshBlock(Block _block)
		{
			Vector3Int _targetNodePos = _block.NodePosition;
				
			Block _blockPrefab = meshData.GetBlockPrefab(_block.BitMask);
				
			if(_blockPrefab == null) return;
				
			RemoveBlock(_targetNodePos);
			AddBlock   (_blockPrefab, _targetNodePos);
		}

#region Add/Remove Block by Node Position
		public void AddBlock(Block _prefab, Vector3Int _nodePos)
		{
			if (IsOccupied(_nodePos)) return;
			
			Vector3 _targetNodeWorldPos = gridData.GetGridWorldNodePosition(_nodePos);
			
			var _newBlock = Instantiate(_prefab, _targetNodeWorldPos, _prefab.transform.localRotation);
			_newBlock.Init(_targetNodeWorldPos, _nodePos);
			
			AddIBlockRef(_newBlock, _nodePos);
		}

		public void RemoveBlock(Vector3Int _nodePos)
		{
			if (!IsOccupied(_nodePos)) return;
			
			DestroyIBlockObject(_nodePos);
			RemoveIBlockRef(_nodePos);
		}
#endregion
		//public bool IsOccupied(Vector3Int _blockPos) => GetIBlock(_blockPos) != null;

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

		// public void AddIBlockRef_Top(IBlock _newBlock, Vector3Int _nodePos)
		// {
		// 	GridUtility.AddNodeUp(_newBlock, _nodePos, gridData.GridUnitSize, ref gridUnitIBlocks);
		// }
		//
		// public void AddIBlockRef_Left(IBlock _newBlock, Vector3Int _nodePos)
		// {
		// 	GridUtility.AddNodeLeft(_newBlock, _nodePos, gridData.GridUnitSize, ref gridUnitIBlocks);
		// }
		//
		// public void AddIBlockRef_Right(IBlock _newBlock, Vector3Int _nodePos)
		// {
		// 	GridUtility.AddNodeLeft(_newBlock, _nodePos, gridData.GridUnitSize, ref gridUnitIBlocks);
		// }
		//
		// public void AddIBlockRef_Forward(IBlock _newBlock, Vector3Int _nodePos)
		// {
		// 	GridUtility.AddNodeLeft(_newBlock, _nodePos, gridData.GridUnitSize, ref gridUnitIBlocks);
		// }
		//
		// public void AddIBlockRef_Back(IBlock _newBlock, Vector3Int _nodePos)
		// {
		// 	GridUtility.AddNodeLeft(_newBlock, _nodePos, gridData.GridUnitSize, ref gridUnitIBlocks);
		// }
		
		public void AddIBlockRef(IBlock _newBlock, Vector3Int _nodePos)
		{
			GridUtility.AddNode(_newBlock, _nodePos, gridData.GridUnitSize, ref gridUnitIBlocks);
		}
		
		public void RemoveIBlockRef(Vector3Int _nodePos)
		{
			GridUtility.RemoveNode(_nodePos, gridData.GridUnitSize, ref gridUnitIBlocks);
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
			return GridUtility.GetNode(_nodePos, gridData.GridUnitSize, gridUnitIBlocks);
		}

		public List<IBlock> GetIBlocks(Vector3Int _startPos, Vector3Int _endPos)
		{
			return GridUtility.GetNodesRectArea(_startPos, _endPos, gridData.GridUnitSize, gridUnitIBlocks);
		}

		public List<IBlock> GetIBlocks3x3Cube(Vector3Int _nodePos)
		{
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, gridData.GridUnitSize, gridUnitIBlocks).ToList();
		}
#endregion

		private void OnDrawGizmos()
		{
			if(!Application.isPlaying) return;
			// if (resultPath.Count == 0) return;
			//
			// foreach (Block _block in resultPath)
			// {
			// 	Gizmos.color = Color.yellow;
			// 	Gizmos.DrawSphere(_block.transform.position + Vector3.up, 0.25f);
			// }

			foreach (var _block in gridUnitIBlocks)
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
