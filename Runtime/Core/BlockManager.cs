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
		
		protected override void Awake()
		{
			GridBlockData.Initialized();

			var _gridBlocks   = GridBlockData.GridUnitIBlocks;
			var _gridUnitSize = GridBlockData.GridUnitSize;
			var _gridLevel    = 0;
			var _blockPrefab  = AssetManager.AssetCollection.DefualtBlock.gameObject;
			
			GridBlockGenerator.PopulateGridIBlocksByLevel<Block>(_gridBlocks, _gridUnitSize, _gridLevel, _blockPrefab);
			
			GridBlockData.InitializeBlocksData();//-->1 Same
			
			GridBlockData.AvailableBlocksApplyAll(UpdateMeshBlock);
			
			GridBlockData.AvailableBlocksApplyAll(_block => //--2 Same
			{
				_block.GetSurroundingIBlocksReference();
				_block.SetBitMask();
			});
				
			GameObject _textParent = new GameObject("[-------Grid Position Text-------]");
			
			GridBlockData.AvailableBlocksApplyAll(_block =>
			{
				Vector3Int _nodePos = ((INode)_block).NodePosition;
				string _posText = _nodePos.ToString();
				
				Vector3 _targetPos = _block.transform.position + Vector3.up * 1.5f;
				Utility.CreateWorldTextPro(_posText, _targetPos, _textParent.transform);
			});

			var _blockParent = new GameObject("[-------------Blocks-------------]");
			
			GridBlockData.AvailableBlocksApplyAll(_block =>
			{
				_block.transform.SetParent(_blockParent.transform);
			});
		}

		private void Start()
		{
			gameObject.AddComponent<GridBlockSelection>();
			gameObject.AddComponent<BlockEditor>();
		}

		public void UpdateMeshBlocks(IEnumerable<Block> _blocks)
		{
			foreach (Block _block in _blocks)
				UpdateMeshBlock(_block);
		}
		
		private void UpdateMeshBlock(Block _block)
		{
			Vector3Int _targetNodePos = _block.NodePosition;
				
			Block _blockPrefab = GridBlockData.GetBlockMeshData().GetBlockPrefab(_block.BitMask);
				
			if(_blockPrefab == null) return;
				
			RemoveBlock(_targetNodePos);
			AddBlock   (_blockPrefab, _targetNodePos);
		}

#region Add/Remove Block by Node Position
		public void AddBlock(Block _prefab, Vector3Int _nodePos)
		{
			if (IsOccupied(_nodePos)) return;
			
			Vector3 _targetNodeWorldPos = GridBlockData.GetGridDataSetting().GetGridWorldNodePosition(_nodePos);
			
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
			GridUtility.AddNode(_newBlock, _nodePos, GridBlockData.GetGridDataSetting().GridUnitSize, ref GridBlockData.GridUnitIBlocks);
		}
		
		public void RemoveIBlockRef(Vector3Int _nodePos)
		{
			GridUtility.RemoveNode(_nodePos, GridBlockData.GetGridDataSetting().GridUnitSize, ref GridBlockData.GridUnitIBlocks);
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
			return GridUtility.GetNode(_nodePos, GridBlockData.GetGridDataSetting().GridUnitSize, GridBlockData.GridUnitIBlocks);
		}

		public List<IBlock> GetIBlocks(Vector3Int _startPos, Vector3Int _endPos)
		{
			return GridUtility.GetNodesRectArea(_startPos, _endPos, GridBlockData.GetGridDataSetting().GridUnitSize, GridBlockData.GridUnitIBlocks);
		}

		public List<IBlock> GetIBlocks3x3Cube(Vector3Int _nodePos)
		{
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, GridBlockData.GetGridDataSetting().GridUnitSize, GridBlockData.GridUnitIBlocks).ToList();
		}
#endregion

		private void OnDrawGizmos()
		{
			if(!Application.isPlaying) return;

			foreach (var _block in GridBlockData.GridUnitIBlocks)
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
