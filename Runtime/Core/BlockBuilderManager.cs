using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Scriptable;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Core.Scriptable;
using MugCup_Utilities;
using MugCup_Utilities.Runtime;
using MugCup_PathFinder.Runtime;
using MugCup_BlockBuilder.Runtime.Core.Managers;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace MugCup_BlockBuilder.Runtime.Core
{
	public class BlockBuilderManager : Singleton<BlockBuilderManager>
	{
		public enum ManagerMode
		{
			Default, Custom
		}

		public ManagerMode Mode = ManagerMode.Default;

		/// <summary>
		/// This will replace static class gridblcokdata
		/// </summary>
		[SerializeField] private GridBlockDataManager gridBlockDataManager;
		[SerializeField] private BlockManager         blockManager;
		
		public GridDataSettingSO CustomGridDataSetting;
		public BlockMeshData     CustomBlockMeshData;

		private const string TextParentName  = "[-------Grid Position Text-------]";
		private const string BlockParentName = "[-------------Blocks-------------]";

#region Managers 
		private readonly Dictionary<Type, BaseBuilderManager> managerCollections = new Dictionary<Type, BaseBuilderManager>();

		[SerializeField] private List<BaseBuilderManager> bbb = new List<BaseBuilderManager>();

		public void Print()
		{
			managerCollections.Keys.ToList().ForEach(_t => Debug.LogWarning($"{_t}"));
		}

		public T GetManager<T>() where T : BaseBuilderManager
		{
			if (managerCollections.ContainsKey(typeof(T)))
			{
				return managerCollections[typeof(T)] as T;
			}
			return default;
		}

		public void AddManager(BaseBuilderManager _manager)
		{
			var _type = _manager.GetType();
			
			if (!managerCollections.ContainsKey(_type))
			{
				managerCollections.Add(_type, _manager);
				return;
			}
			managerCollections[_type] = _manager;
		}
#endregion

		/// <summary>
		/// BlockBuilder Package Initialization start from here.
		/// </summary>
		protected override void Awake()
		{
			Initialize();
		}
		
		private void Start()
		{
			AddRequiredComponents();
			InitializeManagers();
		}
		
		private void Initialize()
		{
			InjectGridBlockDataManager();

			var _gridBlocks   = GridBlockData.GridUnitIBlocks;
			var _gridUnitSize = GridBlockData.GridUnitSize;
			var _gridLevel    = 0;
			var _blockPrefab  = AssetManager.AssetCollection.DefualtBlock.gameObject;

			GridBlockGenerator.PopulateGridIBlocksByLevel<Block>(_gridBlocks, _gridUnitSize, _gridLevel, _blockPrefab);

			GridBlockData.InitializeBlocksData(); //-->1 Same

			GridBlockData.AvailableBlocksApplyAll(blockManager.UpdateMeshBlock);
			
			GridBlockData.AvailableBlocksApplyAll(_block => //--2 Same
			{
				_block.GetSurroundingIBlocksReference();
				_block.SetBitMask();
			});

			CreateTextOverlay  ();
			GroupBlocksToParent();
		}
		
		/// <summary>
		/// Initialize Grid Block Data. Need to be done first before do anything.
		/// </summary>
		private void InjectGridBlockDataManager()
		{
			gridBlockDataManager = FindObjectOfType<GridBlockDataManager>();
			
			gridBlockDataManager.Initialized();
			
			switch (Mode)
			{
				case ManagerMode.Default:
					gridBlockDataManager.Initialized();
					break;
				case ManagerMode.Custom:
					gridBlockDataManager.InitializeWith(CustomGridDataSetting, CustomBlockMeshData);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		// private void GetBlockData()
		// {
		// 	switch (Mode)
		// 	{
		// 		case ManagerMode.Default:
		// 			GridBlockData.Initialized();
		// 			break;
		// 		case ManagerMode.Custom:
		// 			GridBlockData.InitializeWith(CustomGridDataSetting, CustomBlockMeshData);
		// 			break;
		// 		default:
		// 			throw new ArgumentOutOfRangeException();
		// 	}
		// }
		
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


		private void AddRequiredComponents()
		{
			gameObject.AddComponent<InputManager>         ();
			gameObject.AddComponent<GridBlockSelection>   ();
			gameObject.AddComponent<BlockEditorManager>   ();
			gameObject.AddComponent<BlockSelectionManager>();
			gameObject.AddComponent<PointerVisualizer>    ();

			gameObject.AddComponent<StateManager>();//Testing//
		}

		private void InitializeManagers()
		{
			var _managers = FindObjectsOfType<BaseBuilderManager>();
			
			_managers.ToList().ForEach(_o =>
			{
				_o.Init();
				AddManager(_o);
			});
			
			
			
			_managers.ToList().ForEach(_o => Debug.LogWarning(_o));
		}

		
		//Might need to move all code below into another class(BlockManager)//
		
// 		public void UpdateMeshBlocks(IEnumerable<Block> _blocks)
// 		{
// 			foreach (var _block in _blocks)
// 				UpdateMeshBlock(_block);
// 		}
// 		
// 		private void UpdateMeshBlock(Block _block)
// 		{
// 			Vector3Int _targetNodePos = _block.NodePosition;
// 				
// 			Block _blockPrefab = GridBlockData.GetBlockMeshData().GetBlockPrefab(_block.BitMask);
// 				
// 			if(_blockPrefab == null) return;
// 				
// 			RemoveBlock(_targetNodePos);
// 			AddBlock   (_blockPrefab, _targetNodePos);
// 		}
//
// #region Add/Remove Block by Node Position
// 		public void AddBlock(Block _prefab, Vector3Int _nodePos)
// 		{
// 			if (IsOccupied(_nodePos)) return;
// 			
// 			var _targetNodeWorldPos = GridBlockData.GetGridDataSetting().GetGridWorldNodePosition(_nodePos);
// 			
// 			var _newBlock = Instantiate(_prefab, _targetNodeWorldPos, _prefab.transform.localRotation);
// 			_newBlock.Init(_targetNodeWorldPos, _nodePos);
// 			
// 			AddIBlockRef(_newBlock, _nodePos);
// 		}
//
// 		public void RemoveBlock(Vector3Int _nodePos)
// 		{
// 			if (!IsOccupied(_nodePos)) return;
// 			
// 			DestroyIBlockObject(_nodePos);
// 			RemoveIBlockRef    (_nodePos);
// 		}
// #endregion
// 		
// 		public bool IsOccupied(Vector3Int _nodePos)
// 		{
// 			if (GetIBlock(_nodePos) != null) {
// 				Debug.Log($"<color=red>[Warning] : </color> Grid at position {_nodePos} is occupied");
// 				return true;
// 			}
//
// 			Debug.Log($"<color=yellow>[Info] : </color> Grid at position {_nodePos} is empty");
// 			return false;
// 		}
// 		
// 		public void DestroyIBlockObject(Vector3Int _nodePos)
// 		{
// 			var _blockToBeRemoved = GetIBlock(_nodePos);
// 			Destroy(_blockToBeRemoved.gameObject);
// 		}
// 		
// 		public void AddIBlockRef(IBlock _newBlock, Vector3Int _nodePos)
// 		{
// 			var _gridUnitSize    = GridBlockData.GetGridDataSetting().GridUnitSize;
// 			var _gridUnitIBlocks = GridBlockData.GridUnitIBlocks;
// 			
// 			GridUtility.AddNode(_newBlock, _nodePos, _gridUnitSize, ref _gridUnitIBlocks);
// 		}
// 		
// 		public void RemoveIBlockRef(Vector3Int _nodePos)
// 		{
// 			var _gridUnitSize    = GridBlockData.GetGridDataSetting().GridUnitSize;
// 			var _gridUnitIBlocks = GridBlockData.GridUnitIBlocks;
// 			
// 			GridUtility.RemoveNode(_nodePos, _gridUnitSize, ref _gridUnitIBlocks);
// 		}
//
// #region Get Blocks
// 		public Block GetBlock(Vector3Int _nodePos) => GetIBlock(_nodePos) as Block;
// 		
// 		public T GetBlock<T>(Vector3Int _nodePos) where T : IBlock
// 		{
// 			return (T)GetIBlock(_nodePos);
// 		}
// #endregion
//
// #region Get IBlocks
// 		public IBlock GetIBlock(Vector3Int _nodePos)
// 		{
// 			var _gridUnitSize    = GridBlockData.GetGridDataSetting().GridUnitSize;
// 			var _gridUnitIBlocks = GridBlockData.GridUnitIBlocks;
// 			
// 			return GridUtility.GetNode(_nodePos, _gridUnitSize, _gridUnitIBlocks);
// 		}
//
// 		public List<IBlock> GetIBlocks(Vector3Int _startPos, Vector3Int _endPos)
// 		{
// 			var _gridUnitSize    = GridBlockData.GetGridDataSetting().GridUnitSize;
// 			var _gridUnitIBlocks = GridBlockData.GridUnitIBlocks;
// 			
// 			return GridUtility.GetNodesRectArea(_startPos, _endPos, _gridUnitSize, _gridUnitIBlocks);
// 		}
//
// 		public List<IBlock> GetIBlocks3x3Cube(Vector3Int _nodePos)
// 		{
// 			var _gridUnitSize    = GridBlockData.GetGridDataSetting().GridUnitSize;
// 			var _gridUnitIBlocks = GridBlockData.GridUnitIBlocks;
// 			
// 			return GridUtility.GetNodesFrom3x3Cubes(_nodePos, _gridUnitSize, _gridUnitIBlocks).ToList();
// 		}
// #endregion
//
// 		private void OnDrawGizmos()
// 		{
// 			if(!Application.isPlaying) return;
//
// 			foreach (var _block in GridBlockData.GridUnitIBlocks)
// 			{
// 				if (_block != null)
// 				{
// 					Gizmos.color = Color.red;
// 					Gizmos.DrawSphere(_block.transform.position, 0.05f);
// 				}
// 			}
// 		}
	}
}