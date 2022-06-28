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
	//TODO
	//-> Block be able to change Material scheme
	//-> Block variety when added or edit
	//-> Barricade
	//-> Flag
	
	
	
	public struct BlockDataSetting
	{
		public GridDataSettingSO GridDataSetting;
		
		public BlockMeshData     BlockMeshDataSetting;
		public BlockMeshData     PathBlockMeshDataSetting;
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

		[SerializeField] private BlockManager blockManager;

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
			//Initialized();
		}
		
		public void Initialized()
		{
			InitializeBlockManager();
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

		private void InitializeBlockManager()
		{
			//Temp
			blockManager = FindObjectOfType<BlockManager>();

			BlockDataSetting _blockDataSetting = new BlockDataSetting
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
	}
}
