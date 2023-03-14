#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Editor;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
	public static class PaintPanelGUI
	{
		private static Material material;

		private static bool displayMaterialSetting;
        
		public static void Display()
		{
			displayMaterialSetting = BBEditorStyling.DrawHeader(Color.clear, "Material Setting", displayMaterialSetting);

			if (displayMaterialSetting)
			{
				GUILayout.BeginVertical("GroupBox");
				material = (Material)EditorGUILayout.ObjectField("Block Material", material, typeof(Material));

				if (GUILayout.Button("Apply material to all blocks"))
				{
					if (material != null)
					{
						var _gridElementDataManager = BBEditorManager.GridElementDataManager;
						
						_gridElementDataManager.ApplyMaterialToVolumePoints(material);
					}
				}
				GUILayout.EndHorizontal();
			}
		}
	}
}
#endif
