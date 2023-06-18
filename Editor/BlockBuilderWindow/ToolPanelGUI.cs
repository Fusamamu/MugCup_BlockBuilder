#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Scriptable;
using UnityEditor;
using UnityEngine;

using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Editor;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder
{
	public static class ToolPanelGUI
	{
		public static void Display()
		{
			if (GUILayout.Button("Update Nodes Position"))
			{
				var _gridNodes = Object.FindObjectsOfType<GridNode>();

				foreach (var _node in _gridNodes)
				{
					_node.SetNodeWorldPosition(_node.transform.position);
					_node.SetNodePosition(MugCup_BlockBuilder.Runtime.Utilities.CastVec3ToVec3Int(_node.NodeWorldPosition));
					EditorUtility.SetDirty(_node);
				}
			}

			if (GUILayout.Button("Update Node Reference in Grid"))
			{
				var _gridNodes = Object.FindObjectsOfType<GridNode>();
				foreach (var _node in _gridNodes)
				{
					var _gridData = Object.FindObjectOfType<GridNodeData>();
					if (_gridData != null)
					{
						_node.AddSelfToGrid(_gridData);
						EditorUtility.SetDirty(_gridData);
					}
				}
			}
		}
	}
}
#endif
