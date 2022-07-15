using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_PathFinder.Runtime;
using MugCup_Utilities.Runtime;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MugCup_BlockBuilder.Runtime
{
	public static class BlockBuilderPathName
	{
		public static string TextParentName  = "[-------Grid Position Text-------]";
		public static string BlockParentName = "[-------------Blocks-------------]";
	}
	
	public enum CubeBlockSection
	{
		Top, Middle, Bottom 
	}
	
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
	    
	    public GridBlockDataManager GetCurrentGridBlockDataManager()
	    {
		    return gridBlockDataManager;
	    }
	    
#region Initialization [Using Preset from GridBlockDataManager or Manually pass via arg]
	    public void DefaultInitialized()
	    {
		    //Block Manager is responsible for initializing Grid Block Data Manager//
		    gridBlockDataManager = FindObjectOfType<GridBlockDataManager>();
		    gridBlockDataManager.DefaultInitialized();
	    }

	    public void InitializeWith(BlockDataSetting _blockDataSetting)
	    {
		    //Block Manager is responsible for initializing Grid Block Data Manager//
		    gridBlockDataManager = FindObjectOfType<GridBlockDataManager>();
		    gridBlockDataManager.InitializedWith(_blockDataSetting);
	    }
#endregion
	    
	    public void GenerateGridBlocks()
	    {
		    gridBlockDataManager.InitializeGridArray();
		    gridBlockDataManager.PopulateGridBlocksByLevel(0);
		    
		    gridBlockDataManager.InitializeBlocksData(this); 
		     
		    gridBlockDataManager.AvailableBlocksApplyAll(UpdateMeshBlock);
		    
		    gridBlockDataManager.AvailableBlocksApplyAll(_block => 
		    {
			    _block.GetSurroundingBlocksReference();
			    _block.SetBitMask();
		    });
		    
		    GroupBlocksToParent();
		    
		    //GroupBlocksToOwnLevel();
		    // Need to add option whether to create text overlay
		    // CreateTextOverlay  ();
	    }
	    
#region Update Surrounding Blocks
	    public void UpdateSurroundingBlocksData<T>(Vector3Int _nodePos) where T: Block
	    {
		    List<Block> _blocks = GetBlocks3x3Cube(_nodePos);

		    var _castBlocks = new List<T>();
		    
		    foreach (var _block in _blocks)
		    {
			    if(_block == null) continue;
			    
			    var _castBlock = _block as T;
			    _castBlocks.Add(_castBlock);
		    }

		    foreach (var _block in _castBlocks)
		    {
			    if (_block == null) continue;
			    
			    Debug.Log($"{_block.name} : {_block.GetType()}");
			    
			    _block.GetSurroundingBlocksReference();
			    _block.SetBitMask();
		    }
		    
		    UpdateMeshBlocks(_castBlocks);
	    }
	    
	    private void UpdateMeshBlocks<T>(IEnumerable<T> _blocks) where T : Block
	    {
		    foreach (var _block in _blocks)
		    {
			    if(_block == null) continue;
			    
			    UpdateMeshBlock(_block);
		    }
	    }
	    
	    private void UpdateMeshBlock<T>(T _block) where T : Block
	    {
		    Vector3Int _targetNodePos = _block.NodePosition;

		    int _bitMaskMiddleSection = _block.GetBitMaskMiddleSection();
			
		    BlockMeshInfo _blockMeshInfo = gridBlockDataManager.GetBlockMeshData<Path>().GetBlockPrefabMiddleSection(_bitMaskMiddleSection);

		    Block      _blockPrefab  = _blockMeshInfo.Prefab;
		    Quaternion _rotation     = _blockMeshInfo.Rotation * _blockPrefab.transform.localRotation;
			
		    if (_blockPrefab == null)
		    {
			    _blockPrefab = gridBlockDataManager.GetBlockMeshData().GetDefaultBlock();
		    }
				
		    RemoveBlock(_targetNodePos);
		    AddBlock   (_blockPrefab, _targetNodePos, _rotation);
	    }
	    
	    
	    
	    
	    
	    
	    
	    /// <summary>
	    /// Update Surrounding Blocks around 3x3 cube and Reset Bitmasks. Then Update Meshes
	    /// (Add->Remove Based on Preset Prefab Data in MeshData)
	    /// <param name="_nodePos"></param>
	    public void UpdateSurroundBlocksBitMask(Vector3Int _nodePos, CubeBlockSection _section)
	    {
		    List<Block> _blocks = GetBlocks3x3Cube(_nodePos);
            
		    foreach (var _block in _blocks)
		    {
			    if(_block == null) continue;
				    
			    _block.GetSurroundingBlocksReference();
			    _block.SetBitMask();
		    }

		    var _selectedSection = new List<Block>();
		    
		    switch(_section)
		    {
			    case CubeBlockSection.Top:
				    _selectedSection = GetBlocksTopSection(_nodePos);
				    break;
			    case CubeBlockSection.Middle:
				    _selectedSection = GetBlocksMiddleSection(_nodePos);
				    break;
			    case CubeBlockSection.Bottom:
				    _selectedSection = GetBlocksBottomSection(_nodePos);
				    break;
		    }
		    
		    UpdateMeshBlocks(_selectedSection);
	    }
	    
        private void UpdateMeshBlocks(IEnumerable<Block> _blocks)
		{
			foreach (var _block in _blocks)
			{
				if(_block == null) continue;
				
				UpdateMeshBlock(_block);
			}
		}
		
		private void UpdateMeshBlock(Block _block)
		{
			Vector3Int _targetNodePos = _block.NodePosition;

			int _bitMaskMiddleSection = _block.GetBitMaskMiddleSection();
			
			BlockMeshInfo _blockMeshInfo = gridBlockDataManager.GetBlockMeshData().GetBlockPrefabMiddleSection(_bitMaskMiddleSection);

			Block      _blockPrefab = _blockMeshInfo.Prefab;
			Quaternion _rotation    = _blockMeshInfo.Rotation * _blockPrefab.transform.localRotation;
			
			if (_blockPrefab == null)
			{
				_blockPrefab = gridBlockDataManager.GetBlockMeshData().GetDefaultBlock();
			}
				
			RemoveBlock(_targetNodePos);
			AddBlock   (_blockPrefab, _targetNodePos, _rotation);
		}
#endregion

#region Add/Remove Block by Node Position
	    public void AddBlock(Block _prefab, Vector3Int _nodePos)
	    {
		    var _newBlock = Add(_prefab, _nodePos, _prefab.transform.localRotation);
		    
		    _newBlock.Init();
		    _newBlock.InjectDependency(this);
	    }
	    
	    //Will Try to Use Generic Version of this Method instead. This will be removed
		private void AddBlock(Block _prefab, Vector3Int _nodePos, Quaternion _rotation)
		{
			if (IsOccupied(_nodePos)) return;
			
			var _targetNodeWorldPos = gridBlockDataManager.GetGridDataSetting().GetGridWorldNodePosition(_nodePos);

			Block _newBlock;
			
			var _blockParent = GameObject.Find(BlockBuilderPathName.BlockParentName);
			
			if(_blockParent)
				_newBlock = Instantiate(_prefab, _targetNodeWorldPos, _rotation, _blockParent.transform);
			else
				_newBlock = Instantiate(_prefab, _targetNodeWorldPos, _rotation);
			
			_newBlock.Init(_targetNodeWorldPos, _nodePos);
			_newBlock.InjectDependency(this);
			
			AddBlockRef(_newBlock, _nodePos);
		}

		public T Add<T>(T _node, Vector3Int _nodePos, Quaternion _rotation) where T : NodeBase
		{
			if (IsOccupiedBy<T>(_nodePos)) return null;
			
			var _targetNodeWorldPos = gridBlockDataManager.GetGridDataSetting().GetGridWorldNodePosition(_nodePos);

			T _newNode;
			
			var _blockParent = GameObject.Find(BlockBuilderPathName.BlockParentName);
			
			if(_blockParent)
				_newNode = Instantiate(_node, _targetNodeWorldPos, _rotation, _blockParent.transform);
			else
				_newNode = Instantiate(_node, _targetNodeWorldPos, _rotation);

			_newNode.NodePosition      = _nodePos;
			_newNode.SetNodeWorldPosition(_targetNodeWorldPos);
			
			AddNodeRef(_newNode, _nodePos);

			return _newNode;
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
			if (GetBlockRef(_nodePos) != null) 
			{
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
			var _gridUnitSize      = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitNodeBases = gridBlockDataManager.GetGridUnitNodeBases;
			
			GridUtility.AddNode(_newBlock, _nodePos, _gridUnitSize, ref _gridUnitNodeBases);
		}

		
		public void RemoveBlockRef(Vector3Int _nodePos)
		{
			var _gridUnitSize      = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitNodeBases = gridBlockDataManager.GetGridUnitNodeBases;
			
			GridUtility.RemoveNode(_nodePos, _gridUnitSize, ref _gridUnitNodeBases);
		}

		
#region Add/Remove Node Generic
	    
		public bool IsOccupiedBy<T>(Vector3Int _nodePos) where T : NodeBase
		{
			if (GetNodeRef<T>(_nodePos) != null)
			{
				Debug.Log($"<color=red>[Warning] : </color> Grid at position {_nodePos} is occupied by {typeof(T)}");
				return true;
			}

			Debug.Log($"<color=yellow>[Info] : </color> Grid at position {_nodePos} is empty");
			return false;
		}
		
		public void AddNodeRef<T>(T _newNode, Vector3Int _nodePos) where T : NodeBase
		{
			var _gridUnitSize      = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitNodeBases = gridBlockDataManager.GetGridUnitNodeBases;
			
			GridUtility.AddNode(_newNode, _nodePos, _gridUnitSize, ref _gridUnitNodeBases);
		}
#endregion
		
		
#region Get Blocks
		// public Block GetBlock(Vector3Int _nodePos) => GetBlockRef(_nodePos) as Block;
		//
		// public T GetBlock<T>(Vector3Int _nodePos) where T : Block
		// {
		// 	return (T)GetBlockRef(_nodePos);
		// }
#endregion

#region Get Blocks
	    //Will Try to use Generic Version of this instead
		public Block GetBlockRef(Vector3Int _nodePos)
		{
			return GetNodeRef<Block>(_nodePos);
		}

		public List<Block> GetBlocks(Vector3Int _startPos, Vector3Int _endPos)
		{
			return GetNodes<Block>(_startPos, _endPos);
		}

		public List<Block> GetBlocks3x3Cube(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitArray<Block>();
			
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
		}

		public List<Block> GetBlocksTopSection(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitArray<Block>();
			
			return GridUtility.GetTopSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
		}

		public List<Block> GetBlocksMiddleSection(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitArray<Block>();
			
			return GridUtility.GetMiddleSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
		}
		
		public List<Block> GetBlocksBottomSection(Vector3Int _nodePos)
		{
			var _gridUnitSize    = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnitIBlocks = gridBlockDataManager.GetGridUnitArray<Block>();
			
			return GridUtility.GetBottomSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
		}
#endregion
	    
	    
	    
#region Get NodeBases Generic
		//Might need to move to new NodeBase Manager
		public T GetNodeRef<T>(Vector3Int _nodePos) where T : NodeBase
		{
			var _gridUnitSize = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnit     = gridBlockDataManager.GetGridUnitArray<T>();
			
			return GridUtility.GetNode(_nodePos, _gridUnitSize, _gridUnit);
		}
		
		public List<T> GetNodes<T>(Vector3Int _startPos, Vector3Int _endPos) where T : NodeBase
		{
			var _gridUnitSize  = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnit      = gridBlockDataManager.GetGridUnitArray<T>();
			
			return GridUtility.GetNodesRectArea(_startPos, _endPos, _gridUnitSize, _gridUnit);
		}

		public List<T> GetNodeBases3x3Cube<T>(Vector3Int _nodePos) where T : NodeBase
		{
			var _gridUnitSize  = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnit      = gridBlockDataManager.GetGridUnitArray<T>();
			
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}

		public List<T> GetNodeBasesTopSection<T>(Vector3Int _nodePos) where T : NodeBase
		{
			var _gridUnitSize  = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnit      = gridBlockDataManager.GetGridUnitArray<T>();
			
			return GridUtility.GetTopSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}

		public List<T> GetNodeBasesMiddleSection<T>(Vector3Int _nodePos) where T : NodeBase
		{
			var _gridUnitSize  = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnit      = gridBlockDataManager.GetGridUnitArray<T>();
			
			return GridUtility.GetMiddleSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}
		
		public List<T> GetNodeBasesBottomSection<T>(Vector3Int _nodePos) where T : NodeBase
		{
			var _gridUnitSize  = gridBlockDataManager.GetGridDataSetting().GridUnitSize;
			var _gridUnit      = gridBlockDataManager.GetGridUnitArray<T>();
			
			return GridUtility.GetBottomSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}
#endregion
	    
	    
	    
	    
	    
	    
	    
	    
	    //Generate/Populate Block Related Function
	    private void GroupBlocksToParent()
	    {
		    var _blockParent = new GameObject(BlockBuilderPathName.BlockParentName);

		    gridBlockDataManager.AvailableBlocksApplyAll(_block =>
		    {
			    _block.transform.SetParent(_blockParent.transform);
		    });
	    }

	    private void GroupBlocksToOwnLevel()
	    {
		    var _levels = gridBlockDataManager.LevelUnit;

		    for (var _i = 0; _i < _levels; _i++)
		    {
			    var _levelParent = new GameObject($"Block Level : {_i}");

			    var _blocksLevel = gridBlockDataManager.GetAllNodeBasesAtLevel<Block>(_i);

			    foreach (var _block in _blocksLevel)
			    {
				    if(_block == null) continue;
				    
				    _block.transform.SetParent(_levelParent.transform);
			    }
		    }
	    }

	    private void CreateTextOverlay()
	    {
		    var _textParent = new GameObject(BlockBuilderPathName.TextParentName);

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

			foreach (var _block in gridBlockDataManager.GetGridUnitArray<Block>()) 
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
