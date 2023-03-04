using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder
{
	public class ModuleSlotData : GridData<ModuleSlot>
	{
		private HashSet<ModuleSlot> workArea;

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

		public void Collapse(IEnumerable<Vector3Int> _targets)
		{
			workArea = new HashSet<ModuleSlot>();

			foreach (var _target in _targets)
			{
				if (TryGetNode(_target, out var _node))
					workArea.Add(_node);
			}

			while (workArea.Any())
			{
				var _minEntropy = float.PositiveInfinity;

				ModuleSlot _selectedSlot;

				foreach (var _slot in workArea)
				{
					_selectedSlot = _slot;
				}
			}
		}

		public void NotifySlotCollapsed()
		{
			
		}
	}
}
