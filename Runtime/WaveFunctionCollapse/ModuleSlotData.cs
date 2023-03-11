using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using MugCup_Utilities;
using MugCup_PathFinder.Runtime;
using UnityEditor;

namespace MugCup_BlockBuilder
{
	public class ModuleSlotData : GridData<ModuleSlot>
	{
		[field : SerializeField] public ModuleData CurrentModuleData { get; private set; }
		
		private HashSet<ModuleSlot> workArea;

		public Queue<ModuleSlot> BuildQueue;
		public QueueDictionary<Vector3Int, ModuleSet> RemovalQueue;

		[SerializeField] private int MaxIteration = 100;

		public override void Initialized()
		{
			CurrentModuleData.Initialized();

			BuildQueue   = new Queue<ModuleSlot>();
			RemovalQueue = new QueueDictionary<Vector3Int, ModuleSet>(() => new ModuleSet(CurrentModuleData.AllModuleCount));

			foreach (var _slot in GridNodes)
			{
				if(_slot == null) continue;
				
				_slot
					.SetModuleSlotData(this)
					.Initialized();
				
				#if UNITY_EDITOR
				if (!Application.isPlaying)
					EditorUtility.SetDirty(_slot);
				#endif
			}
		}

		public ModuleSlot GetNeighbor(Vector3Int _nodePos, ModuleFace _face)
		{
			switch (_face)
			{
				case ModuleFace.FORWARD:
					return GetNodeForward(_nodePos);
				case ModuleFace.RIGHT:
					return GetNodeRight  (_nodePos);
				case ModuleFace.BACK:
					return GetNodeBack   (_nodePos);
				case ModuleFace.LEFT:
					return GetNodeLeft   (_nodePos);
				case ModuleFace.UP:
					return GetNodeUp     (_nodePos);
				case ModuleFace.DOWN:
					return GetNodeDown   (_nodePos);
			}

			return null;
		}

		public void CollapseAll()
		{
			var _targetPositions = GridNodes.Select(_node => _node.NodeGridPosition);
			
			Collapse(_targetPositions);
		}

		public void Collapse(IEnumerable<Vector3Int> _targets)
		{
			try
			{
				RemovalQueue.Clear();
				
				workArea = new HashSet<ModuleSlot>();
				foreach (var _target in _targets)
				{
					if (TryGetNode(_target, out var _node))
					{
						if(!_node.IsCollapsed)
							workArea.Add(_node);
					}
				}

				int _i = 0;

				while (workArea.Any() && _i < MaxIteration)
				{
					var _minEntropy = float.PositiveInfinity;

					ModuleSlot _selectedSlot = null;

					foreach (var _slot in workArea)
					{
						var _entropy = _slot.AvailableModuleSet.Entropy;

						if (_entropy < _minEntropy)
						{
							_selectedSlot = _slot;
							_minEntropy   = _entropy;
						}
					}
					
					_i++;

					try
					{
						if (_selectedSlot)
							_selectedSlot.CollapseRandom();
					} 
					catch (Exception _exception)
					{
						Debug.LogWarning("Exception in world generation thread at" + _exception.StackTrace);
					}
				}
			} 
			catch (Exception _exception)
			{
				Debug.LogWarning("Exception in world generation thread at" + _exception.StackTrace);
			}
		}

		public void NotifySlotCollapsed(ModuleSlot _slot)
		{
			workArea?.Remove(_slot);
			BuildQueue.Enqueue(_slot);
		}
		
		public void FinishRemovalQueue() 
		{
			// while (RemovalQueue.Any()) 
			// {
			// 	var _kvp = RemovalQueue.Dequeue();
			//
			// 	var _nodePos           = _kvp.Key;
			// 	var _toRemoveModuleSet = _kvp.Value;
			//
			// 	var _slot = GetNode(_nodePos);
			//
			// 	if (!_slot.IsCollapsed)
			// 	{
			// 		_slot.RemoveModules(_toRemoveModuleSet, false);
			// 	}
			// }
		}
	}
}
