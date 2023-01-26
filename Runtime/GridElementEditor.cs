#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
	[CustomEditor(typeof(GridElement))]
	public class GridElementEditor : Editor
	{
		private GridElement gridElement;

		private void OnEnable()
		{
			gridElement = (GridElement)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
				
			if (GUILayout.Button("Enable"))
			{
				gridElement.Enable();
			}

			if (GUILayout.Button("Disable"))
			{
				gridElement.Disable();
			}
		}
	}
}
#endif
