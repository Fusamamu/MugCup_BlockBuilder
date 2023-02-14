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
			DEFAULT, 
			CUSTOM
		}
		
		public ManagerMode Mode = ManagerMode.DEFAULT;

		[field: SerializeField] public BlockManager       BlockManager       { get; private set; }
		[field: SerializeField] public GridElementManager GridElementManager { get; private set; }

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (BlockManager == null)
				BlockManager = GetComponent<BlockManager>();

			if (GridElementManager == null)
				GridElementManager = GetComponent<GridElementManager>();
		}
#endif

#region Managers 
		private readonly Dictionary<Type, BaseBuilderManager> managerCollections = new Dictionary<Type, BaseBuilderManager>();

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

		public void Initialized()
		{
			InitializeManagers();
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
	}
}
