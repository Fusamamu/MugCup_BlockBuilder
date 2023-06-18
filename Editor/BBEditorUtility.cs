#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
	public static class BBEditorUtility
	{
		// public static void VerticalLayout(string _style, Action _action)
		// {
		// 	EditorGUILayout.BeginVertical(_style);
		// 	_action?.Invoke();
		// 	EditorGUILayout.EndVertical();
		// }
		
		public static void RecordGridBlockManagerChanges(Action _action)
		{
			var _gridBlockDataManager = BBEditorManager.BlockManager.GridBlockDataManager;
			
			Undo.RecordObject(_gridBlockDataManager, "GridBlockDataManager Changed");
                            
			_action?.Invoke();
                     
			PrefabUtility.RecordPrefabInstancePropertyModifications(_gridBlockDataManager);
		}
	}
}
#endif
