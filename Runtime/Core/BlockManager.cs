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
	    /// <summary>
	    /// Grid Block Data should be only managed via BlockManager
	    /// GridBlockDataManager might be able to swapped to different data when stage changed
	    /// </summary>
	    [SerializeField] private GridBlockDataManager gridBlockDataManager;
	    
	    public const string TextParentName  = "[-------Grid Position Text-------]";
	    public const string BlockParentName = "[-------------Blocks-------------]";

	    public GridBlockDataManager GetCurrentGridBlockDataManager()
	    {
		    return gridBlockDataManager;
	    }
	    
#region Initialization [Using Preset from GridBlockDataManager or Manually pass via arg]
	    public void Initialized()
	    {
		    //Block Manager is responsible for initializing Grid Block Data Manager//
		    gridBlockDataManager = FindObjectOfType<GridBlockDataManager>();
		    gridBlockDataManager.Initialized();
	    }

	    public void InitializeWith(GridDataSettingSO _gridDataSetting, BlockMeshData _meshDataSetting)
	    {
		    //Block Manager is responsible for initializing Grid Block Data Manager//
		    gridBlockDataManager = FindObjectOfType<GridBlockDataManager>();
		    gridBlockDataManager.InitializeWith(_gridDataSetting, _meshDataSetting);
	    }
#endregion

	    public void GenerateGridBlocks(Vector3Int _unitSize, GameObject _defaultBlock, GameObject _mainMap)
	    {
		    Block[] _blocks = GridBlockGenerator.GenerateGridBlocks(_unitSize, _defaultBlock, _mainMap);

		    gridBlockDataManager.LoadGridBlocksData(_blocks);
		    gridBlockDataManager.InitializeBlocksData(this);
	    }

	    public void GenerateGridBlocks()
	    {
		    gridBlockDataManager.PopulateGridBlocksByLevel();
		    
		    gridBlockDataManager.InitializeBlocksData(this); 

		    gridBlockDataManager.AvailableBlocksApplyAll(UpdateMeshBlock);
		    
		    gridBlockDataManager.AvailableBlocksApplyAll(_block => 
		    {
			    _block.GetSurroundingBlocksReference();
			    _block.SetBitMask();
		    });
		    
		    CreateTextOverlay  ();
		    GroupBlocksToParent();
	    }

	    /// <summary>
	    /// Update Surrounding Blocks around 3x3 cube and Reset Bitmasks. Then Update Meshes
	    /// (Add->Remove Based on Preset Prefab Data in MeshData)
	    /// <param name="_nodePos"></param>
	    public void UpdateSurroundBlocksBitMask(Vector3Int _nodePos)
	    {
		    List<Block> _blocks = GetBlocks3x3Cube(_nodePos);

		    Block[] _checkedBlocks = _blocks.Where(_block => _block != null).ToArray();
            
		    foreach (var _block in _checkedBlocks)
		    {
			    if (_block != null)
			    {
				    _block.GetSurroundingBlocksReference();
				    _block.SetBitMask();
			    }
		    }
		    
		    UpdateMeshBlocks(_checkedBlocks);
	    }
	    
        public void UpdateMeshBlocks(IEnumerable<Block> _blocks)
		{
			foreach (var _block in _blocks)
				UpdateMeshBlock(_block);
		}
		
		public void UpdateMeshBlock(Block _block)
		{
			Vector3Int _targetNodePos = _block.NodePosition;
			
			BlockMeshInfo _blockMeshInfo = gridBlockDataManager.GetBlockMeshData().GetBlockPrefabMiddleSection(_block.GetBitMaskMiddleSection());

			Block _blockPrefab   = _blockMeshInfo.Prefab;
			Quaternion _rotation = _blockMeshInfo.Rotation * _blockPrefab.transform.localRotation;
			
			if (_blockPrefab == null)
			{
				_blockPrefab = gridBlockDataManager.GetBlockMeshData().GetDefaultBlock();
			}
				
			RemoveBlock(_targetNodePos);
			AddBlock   (_blockPrefab, _targetNodePos, _rotation);
		}

#region Add/Remove Block by Node Position
	    public void AddBlock(Block _prefab, Vector3Int _nodePos)
	    {
		    AddBlock(_prefab, _nodePos, _prefab.transform.localRotation);
	    }
	    
		public void AddBlock(Block _prefab, Vector3Int _nodePos, Quaternion _rotation)
		{
			if (IsOccupied(_nodePos)) return;
			
			var _targetNodeWorldPos = gridBlockDataManager.GetGridDataSetting().GetGridWorldNodePosition(_nodePos);
			
			var _newBlock = Instantiate(_prefab, _targetNodeWorldPos, _rotation);
			
			_newBlock.Init(_targetNodeWorldPos, _nodePos);
			_newBlock.InjectDependency(this);
			
			AddBlockRef(_newBlock, _nodePos);
		}

		public void RemoveBlock(Block _block)
		{
			RemoveBlock(_block.NodePosition);
		}

		public void RemoveBlock(Vector3Int _nodePos)
		{
			if (!IsOccupied(_nodePos)) return;
			
			DestroyBlockObject(_nodePos);
			RemoveBlockRef    (_nodePos);
		}
#endregion
		
		public bool IsOccupied(Vector3Int _nodePos)
		{
			if (GetBlockRef(_nodePos) != null) {
				Debug.Log($"<color=red>[Warning] : </color> Grid at position {_nodePos} is occupied");
				return true;
			}

			Debug.Log($"<color=yellow>[Info] : </color> Grid at position {_nodePos} is empty");
			return false;
		}
		
		public void DestroyBlockObject(Vector3Int _nodePos)
		{
			var _blockToBeRemoved = GetBlockRef(_nodePos);
			
			if(Application.isPlaying)
				Destroy(_blockToBeRemoved.gameObject);
			else
				DestroyImmediate(_blockToBeRemoved.gameObject);
		}
		
		public void AddBlockRef(Block _newBlock, Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitBlocks();
			
			GridUtility.AddNode(_newBlock, _nodePos, _gridUnitSize, ref _gridUnitIBlocks);
		}
		
		public void RemoveBlockRef(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitBlocks();
			
			GridUtility.RemoveNode(_nodePos, _gridUnitSize, ref _gridUnitIBlocks);
		}

#region Get Blocks
		public Block GetBlock(Vector3Int _nodePos) => GetBlockRef(_nodePos) as Block;
		
		public T GetBlock<T>(Vector3Int _nodePos) where T : Block
		{
			return (T)GetBlockRef(_nodePos);
		}
#endregion

#region Get IBlocks
		public Block GetBlockRef(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitBlocks();
			
			return GridUtility.GetNode(_nodePos, _gridUnitSize, _gridUnitIBlocks);
		}

		public List<Block> GetBlocks(Vector3Int _startPos, Vector3Int _endPos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitBlocks();
			
			return GridUtility.GetNodesRectArea(_startPos, _endPos, _gridUnitSize, _gridUnitIBlocks);
		}

		public List<Block> GetBlocks3x3Cube(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitBlocks();
			
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
		}
#endregion
	    //Generate/Populat Block Related Function
	    private void GroupBlocksToParent()
	    {
		    var _blockParent = new GameObject(BlockParentName);

		    gridBlockDataManager.AvailableBlocksApplyAll(_block =>
		    {
			    _block.transform.SetParent(_blockParent.transform);
		    });
	    }

	    private void CreateTextOverlay()
	    {
		    var _textParent = new GameObject(TextParentName);

		    gridBlockDataManager.AvailableBlocksApplyAll(_block =>
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

			foreach (var _block in gridBlockDataManager.GetGridUnitBlocks())
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
