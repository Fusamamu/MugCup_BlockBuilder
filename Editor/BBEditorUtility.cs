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
		public static void RecordGridBlockManagerChanges(Action _action)
		{
			var _gridBlockDataManager = BBEditorManager.BlockManager.GetCurrentGridBlockDataManager();
			
			Undo.RecordObject(_gridBlockDataManager, "GridBlockDataManager Changed");
                            
			_action?.Invoke();
                     
			PrefabUtility.RecordPrefabInstancePropertyModifications(_gridBlockDataManager);
		}
	}
}
