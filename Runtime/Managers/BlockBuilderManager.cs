using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Scriptable;
using BlockBuilder.Core.Scriptable;
using MugCup_Utilities;
using MugCup_BlockBuilder.Runtime.Core.Managers;

namespace MugCup_BlockBuilder.Runtime.Core
{
	[Serializable]
	public struct BlockDataSetting
	{
		public GridDataSettingSO GridDataSetting;
		public BlockMeshData     BlockMeshDataSetting;
		public BlockMeshData     PathBlockMeshDataSetting;

		public BlockDataSetting(GridDataSettingSO _gridDataSetting, BlockMeshData _blockMeshDataSetting, BlockMeshData _pathBlockMeshDataSetting)
		{
			GridDataSetting          = _gridDataSetting;
			BlockMeshDataSetting     = _blockMeshDataSetting;
			PathBlockMeshDataSetting = _pathBlockMeshDataSetting;
		}
	}
	
	[Serializable]
	public class BlockBuilderManager : Singleton<BlockBuilderManager>
	{
		public enum ManagerMode
		{
			Default, Custom
		}

		public ManagerMode Mode = ManagerMode.Default;

		public GridDataSettingSO CustomGridDataSetting  ;
		public BlockMeshData     CustomBlockMeshData    ;
		public BlockMeshData     CustomPathBlockMeshData;

		public BlockManager BlockManager 
		{
			get
			{
				if (!blockManager)
					blockManager = GetComponent<BlockManager>();
				return blockManager;
			}

			private set => blockManager = value;
		}
		
		[SerializeField] private BlockManager blockManager;

#region Managers 
		private readonly Dictionary<Type, BaseBuilderManager> managerCollections = new Dictionary<Type, BaseBuilderManager>();

		public void Print()
		{
			managerCollections.Keys.ToList().ForEach(_t => Debug.LogWarning($"{_t}"));
		}

		public BlockManager GetBlockManager()
		{
			return blockManager != null ? blockManager : null;
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

		public void PreInit()
		{
			//BlockManager.InitializeWith();
		}

		public void Initialized()
		{
			InitializeManagers();
			InitializeBlockManager();
		}
		
		private void InitializeManagers()
		{
			var _managers = FindObjectsOfType<BaseBuilderManager>();
			
			_managers.ToList().ForEach(_o =>
			{
				_o.Init();
				AddManager(_o);
			});
			
			_managers.ToList().ForEach(Debug.LogWarning);
		}
		
		private void InitializeBlockManager()
		{
			if (!gameObject.TryGetComponent(out blockManager))
				blockManager = FindObjectOfType<BlockManager>();

			var _blockDataSetting = new BlockDataSetting
			{
				GridDataSetting          = CustomGridDataSetting,
				BlockMeshDataSetting     = CustomBlockMeshData,
				PathBlockMeshDataSetting = CustomPathBlockMeshData
			};
			
			switch (Mode)
			{
				case ManagerMode.Default:
					blockManager.DefaultInitialized();
					break;
				case ManagerMode.Custom:
					blockManager.InitializeWith(_blockDataSetting);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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
	}
}
