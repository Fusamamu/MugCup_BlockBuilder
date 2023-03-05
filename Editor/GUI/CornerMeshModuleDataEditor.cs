#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugCup_BlockBuilder
{
	[CustomEditor(typeof(CornerMeshModuleData))]
	public class CornerMeshModuleDataEditor : UnityEditor.Editor
	{
		private CornerMeshModuleData cornerMeshModuleData;

		private void OnEnable()
		{
			cornerMeshModuleData = (CornerMeshModuleData)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Set All Module Probability"))
			{
				cornerMeshModuleData.SetAllModuleProbability(1);
				EditorUtility.SetDirty(cornerMeshModuleData);
			}
			
			if (GUILayout.Button("Calculate All PLogP"))
			{
				cornerMeshModuleData.CalculateAllPLogP();
				EditorUtility.SetDirty(cornerMeshModuleData);
			}

			if (GUILayout.Button("Clean Data"))
			{
				cornerMeshModuleData.CleanData();
				EditorUtility.SetDirty(cornerMeshModuleData);
			}
			
			if (GUILayout.Button("Store Modules Possible Neighbors"))
			{
				cornerMeshModuleData.StoreModulesPossibleNeighbors();
				EditorUtility.SetDirty(cornerMeshModuleData);
			}
		}
	}
}
#endif
