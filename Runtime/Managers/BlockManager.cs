using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Debug = UnityEngine.Debug;

using BlockBuilder.Core.Scriptable;

using MugCup_Utilities.Runtime;
using MugCup_PathFinder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using UnityEditor;

namespace MugCup_BlockBuilder.Runtime
{
	public static class BlockBuilderPathName
	{
		public static string TextParentName  = "[-------Grid Position Text-------]";
		public static string BlockParentName = "[-------------Blocks-------------]";
	}
	
	public enum CubeBlockSection
	{
		All, Top, Middle, Bottom 
	}
	
	/// <summary>
	/// Class to keep track of blocks when blocks get added, removed and updated.
	/// This class modify blocks data [Block Array] via GridBlockDataManager.
	/// </summary>
    public class BlockManager : MonoBehaviour, IGridManager
	{
	    /// <summary>
	    /// Grid Block Data should be only managed via BlockManager
	    /// GridBlockDataManager might be able to swapped to different data when stage changed
	    /// </summary>
		[field: SerializeField] public GridBlockDataManager CurrentGridBlockBlockData { get; private set; }
	    
#region Initialization [Using Preset from GridBlockDataManager or Manually pass via arg]
	    public void DefaultInitialized()
	    {
		    //Block Manager is responsible for initializing Grid Block Data Manager//
		    CurrentGridBlockBlockData = FindObjectOfType<GridBlockDataManager>();
		    CurrentGridBlockBlockData.DefaultInitialized();
	    }  

	    public void InitializeWith(BlockDataSetting _blockDataSetting)
	    {
		    //Block Manager is responsible for initializing Grid Block Data Manager//
		    CurrentGridBlockBlockData = FindObjectOfType<GridBlockDataManager>();
		    CurrentGridBlockBlockData.InitializedWith(_blockDataSetting);
	    }
#endregion
	    
	    public void GenerateGridBlocks()
	    {
		    CurrentGridBlockBlockData.GridNodeData.InitializeGridArray();
		    
		    //Need fix!!!
		    //CurrentGridBlockBlockData.GridNodeData.PopulateGridBlocksByLevel(0);
		    
		    CurrentGridBlockBlockData.InitializeBlocksData(this); 
		     
		    CurrentGridBlockBlockData.GridNodeData.ApplyAllNodes<Block>(UpdateMeshBlock);
		    
		    CurrentGridBlockBlockData.GridNodeData.ApplyAllNodes<Block>(_block => 
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
	    /// <summary>
	    /// Update Surrounding Blocks around 3x3 cube and Reset Bitmasks. Then Update Meshes
	    /// (Add->Remove Based on Preset Prefab Data in MeshData)
	    /// </summary>
	    public void UpdateSurroundingBlocksData<T>(Vector3Int _nodePos, CubeBlockSection _section = CubeBlockSection.All) where T: Block
	    {
		    List<T> _blocks = GetNodeBases3x3Cube<T>(_nodePos);

		    foreach (var _block in _blocks)
		    {
			    if (_block == null) continue;
			    
			    _block.GetSurroundingBlocksReference();
			    _block.SetBitMask();
		    }
		    
		    var _selectedSection = new List<T>();
		    
		    switch(_section)
		    {
			    case CubeBlockSection.All:
				    _selectedSection = _blocks;
				    break;
			    case CubeBlockSection.Top:
				    _selectedSection = GetNodeBasesTopSection<T>(_nodePos);
				    break;
			    case CubeBlockSection.Middle:
				    _selectedSection = GetNodeBasesMiddleSection<T>(_nodePos);
				    break;
			    case CubeBlockSection.Bottom:
				    _selectedSection = GetNodeBasesBottomSection<T>(_nodePos);
				    break;
		    }
		    
		    UpdateMeshBlocks(_selectedSection);
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
		    Vector3Int _targetNodePos = _block.NodeGridPosition;

		    int _bitMaskMiddleSection = _block.GetBitMaskMiddleSection();
			
		    BlockMeshInfo _blockMeshInfo = CurrentGridBlockBlockData
			    .GetBlockMeshData<T>()
			    .SetUseComposite(false)
			    .GetBlockPrefabMiddleSection(_bitMaskMiddleSection);

		    Block      _blockPrefab  = _blockMeshInfo.Prefab;
		    Quaternion _rotation     = _blockMeshInfo.Rotation * _blockPrefab.transform.localRotation;
			
		    if (_blockPrefab == null)
		    {
			    _blockPrefab = CurrentGridBlockBlockData.BlockMeshData.GetDefaultBlock();
		    }
				
		    RemoveBlock(_targetNodePos);
		    AddBlock   (_blockPrefab, _targetNodePos, _rotation);
	    }

	    public void UpdateMeshBlockComposite<T>(T _block) where T : Block
	    {
		    Vector3Int _targetNodePos = _block.NodeGridPosition;

		    int _bitMaskMiddleSection = _block.GetBitMaskCompositeMiddleSection();
			
		    BlockMeshInfo _blockMeshInfo = CurrentGridBlockBlockData
			    .GetBlockMeshData<T>()
			    .SetUseComposite(true)
			    .GetBlockPrefabMiddleSection(_bitMaskMiddleSection);

		    if (_blockMeshInfo.Prefab)
		    {
			    Block      _blockPrefab  = _blockMeshInfo.Prefab;
			    Quaternion _rotation     = _blockMeshInfo.Rotation * _blockPrefab.transform.localRotation;
				
			    AddCompositeBlock(_blockPrefab, _targetNodePos, _rotation);
		    }
	    }
#endregion

#region Add/Remove Block by Node Position
	    private void AddCompositeBlock(Block _prefab, Vector3Int _nodePos, Quaternion _rotation)
	    {
		    var _block = GetNodeRef<Block>(_nodePos);

		    var _compositeBlock = Instantiate(_prefab, Vector3.zero, _rotation, _block.transform);
		    
		    _compositeBlock.transform.localPosition = Vector3.zero;
	    }
	    
	    public void AddBlock(Block _prefab, Vector3Int _nodePos)
	    {
		    var _newBlock = CreateNodeAt(_prefab, _nodePos, _prefab.transform.localRotation);
		    
		    _newBlock.Init();
		    _newBlock.InjectDependency(this);
	    }
	    
	    //Will Try to Use Generic Version of this Method instead. This will be removed
		private void AddBlock(Block _prefab, Vector3Int _nodePos, Quaternion _rotation)
		{
			if (IsOccupied(_nodePos)) return;
			
			var _targetNodeWorldPos = CurrentGridBlockBlockData.GridDataSetting.GetGridWorldNodePosition(_nodePos);

			Block _newBlock;
			
			var _blockParent = GameObject.Find(BlockBuilderPathName.BlockParentName);
			
			if(_blockParent)
				_newBlock = Instantiate(_prefab, _targetNodeWorldPos, _rotation, _blockParent.transform);
			else
				_newBlock = Instantiate(_prefab, _targetNodeWorldPos, _rotation);

			_newBlock
				.InjectDependency(this)
				.SetPosition(_targetNodeWorldPos, _nodePos)
				.Init();
			
			AddBlockRef(_newBlock, _nodePos);
		}

		public void RemoveBlock(Block _block)
		{
			RemoveBlock(_block.NodeGridPosition);
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
			var _gridUnitSize      = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnitNodeBases = CurrentGridBlockBlockData.GridNodeData.GridNodes;
			
			GridUtility.AddNode(_newBlock, _nodePos, _gridUnitSize, ref _gridUnitNodeBases);
		}
		
		public void RemoveBlockRef(Vector3Int _nodePos)
		{
			var _gridUnitSize      = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnitNodeBases = CurrentGridBlockBlockData.GridNodeData.GridNodes;
			
			GridUtility.RemoveNode(_nodePos, _gridUnitSize, ref _gridUnitNodeBases);
		}
		
#region Add/Remove NodeBase GameObject
	    public T AddNodeAt<T>(T _prefab, Vector3Int _nodePos) where T : GridNode
	    {
		    var _newBlock = CreateNodeAt(_prefab, _nodePos, _prefab.transform.localRotation);

		    return _newBlock;
	    }
	    
	    public void RemoveNode<T>(T _node) where T : GridNode
	    {
		    RemoveNode<T>(_node.NodeGridPosition);
	    }

	    public void RemoveNode<T>(Vector3Int _nodePos) where T : GridNode
	    {
		    if (!IsOccupiedBy<T>(_nodePos)) return;
		    
		    DestroyNode  <T>(_nodePos);
		    RemoveNodeRef<T>(_nodePos);
	    }
#endregion
		
#region Add/Remove NodeBase References in Grid Unit [Generic]
		private bool IsOccupiedBy<T>(Vector3Int _nodePos) where T : GridNode
		{
			if (GetNodeRef<T>(_nodePos) != null)
			{
				Debug.Log($"<color=red>[Warning] : </color> Grid at position {_nodePos} is occupied by {typeof(T)}");
				return true;
			}

			Debug.Log($"<color=yellow>[Info] : </color> Grid at position {_nodePos} is empty");
			return false;
		}
		
		public void DestroyNode<T>(Vector3Int _nodePos) where T : GridNode
		{
			var _nodeToBeRemoved = GetNodeRef<T>(_nodePos);

			if (Application.isPlaying)
			{
				Destroy(_nodeToBeRemoved.gameObject);
			}
			else
			{
				#if UNITY_EDITOR
				Undo.DestroyObjectImmediate(_nodeToBeRemoved.gameObject);
				#endif
			}
		}
		
		private T CreateNodeAt<T>(T _node, Vector3Int _nodePos, Quaternion _rotation) where T : GridNode
		{
			if (IsOccupiedBy<T>(_nodePos)) return null;
			
			var _targetNodeWorldPos = CurrentGridBlockBlockData.GridDataSetting.GetGridWorldNodePosition(_nodePos);

			T _newNode;
			
			var _blockParent = GameObject.Find(BlockBuilderPathName.BlockParentName);
			
			if(_blockParent)
				_newNode = Instantiate(_node, _targetNodeWorldPos, _rotation, _blockParent.transform);
			else
				_newNode = Instantiate(_node, _targetNodeWorldPos, _rotation);

			_newNode.SetNodePosition     (_nodePos);
			_newNode.SetNodeWorldPosition(_targetNodeWorldPos);
			
			AddNodeRef(_newNode, _nodePos);

			return _newNode;
		}
		
		public void AddNodeRef<T>(T _newNode, Vector3Int _nodePos) where T : GridNode
		{
			var _gridUnitSize      = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnitNodeBases = CurrentGridBlockBlockData.GridNodeData.GridNodes;
			
			GridUtility.AddNode(_newNode, _nodePos, _gridUnitSize, ref _gridUnitNodeBases);
		}
		
		public void RemoveNodeRef<T>(Vector3Int _nodePos) where T : GridNode
		{
			var _gridUnitSize      = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnitNodeBases = CurrentGridBlockBlockData.GridNodeData.GridNodes;
			
			GridUtility.RemoveNode(_nodePos, _gridUnitSize, ref _gridUnitNodeBases);
		}
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
			var _gridUnitSize    = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnitIBlocks = CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<Block>();
			
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
		}
#endregion
	    
#region Get NodeBases Generic
		//Might need to move to new NodeBase Manager
		public T GetNodeRef<T>(Vector3Int _nodePos) where T : GridNode
		{
			var _gridUnitSize = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnit     = CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<T>();
			
			return GridUtility.GetNode(_nodePos, _gridUnitSize, _gridUnit);
		}
		
		public List<T> GetNodes<T>(Vector3Int _startPos, Vector3Int _endPos) where T : GridNode
		{
			var _gridUnitSize  = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnit      = CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<T>();
			
			return GridUtility.GetNodesRectArea(_startPos, _endPos, _gridUnitSize, _gridUnit);
		}

		public List<T> GetNodeBases3x3Cube<T>(Vector3Int _nodePos) where T : GridNode
		{
			var _gridUnitSize  = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnit      = CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<T>();
			
			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}

		public List<T> GetNodeBasesTopSection<T>(Vector3Int _nodePos) where T : GridNode
		{
			var _gridUnitSize  = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnit      = CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<T>();
			
			return GridUtility.GetTopSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}

		public List<T> GetNodeBasesMiddleSection<T>(Vector3Int _nodePos) where T : GridNode
		{
			var _gridUnitSize  = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnit      = CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<T>();
			
			return GridUtility.GetMiddleSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}
		
		public List<T> GetNodeBasesBottomSection<T>(Vector3Int _nodePos) where T : GridNode
		{
			var _gridUnitSize  = CurrentGridBlockBlockData.GridDataSetting.GridUnitSize;
			var _gridUnit      = CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<T>();
			
			return GridUtility.GetBottomSectionNodesFrom3x3Cube(_nodePos, _gridUnitSize, _gridUnit).ToList();
		}
#endregion
	    
	    //Generate/Populate Block Related Function
	    private void GroupBlocksToParent()
	    {
		    var _blockParent = new GameObject(BlockBuilderPathName.BlockParentName);

		    CurrentGridBlockBlockData.GridNodeData.ApplyAllNodes<Block>(_block =>
		    {
			    _block.transform.SetParent(_blockParent.transform);
		    });
	    }

	    private void GroupBlocksToOwnLevel()
	    {
		    var _levels = CurrentGridBlockBlockData.GridNodeData.LevelUnit;
	    
		    for (var _i = 0; _i < _levels; _i++)
		    {
			    var _levelParent = new GameObject($"Block Level : {_i}");
	    
			    var _blocksLevel = CurrentGridBlockBlockData.GridNodeData.GetAllNodeBasesAtLevel<Block>(_i);
	    
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
	    
		    CurrentGridBlockBlockData.GridNodeData.ApplyAllNodes<Block>(_block =>
		    {
			    Vector3Int _nodePos = ((INode)_block).NodeGridPosition;
	    
			    var _posText   = _nodePos.ToString();
			    var _targetPos = _block.transform.position + Vector3.up * 1.5f;
	    
			    Utility.CreateWorldTextPro(_posText, _targetPos, _textParent.transform);
		    });
	    }

		private void OnDrawGizmos()
		{
			if(!Application.isPlaying) return;

			foreach (var _block in CurrentGridBlockBlockData.GridNodeData.GetGridUnitArray<Block>())
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
