#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Editor;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
	public static class GridDataSettingGUI
	{
		private static GUIContent[] contents = new GUIContent[]
		{
			new GUIContent("Row"), new GUIContent("Column"), new GUIContent("Height")
		};
	   
		public static void Display()
        {
            BBEditorManager.InterfaceSetting.MapSettingFoldout 
                = BBEditorStyling.DrawHeader(Color.clear, "Grid Data Setting", BBEditorManager.InterfaceSetting.MapSettingFoldout);

            if (BBEditorManager.InterfaceSetting.MapSettingFoldout)
            {
                GUILayout.BeginVertical("GroupBox");
                    
                EditorGUILayout.LabelField("Map size setting");
                    
                GUILayout.BeginVertical("GroupBox");
                    
                GUILayout.BeginHorizontal();
                
                var _rect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
                var _fieldRect = _rect;
                _fieldRect.xMin = 150;
                    
                EditorGUI.LabelField(_rect, new GUIContent("Map Size", "Set map size here"));
                EditorGUI.MultiIntField(_fieldRect, contents, BBEditorManager.GridDataSettingSo.MapSizeArray);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                    
                var _rect1 = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
                var _fieldRect1 = _rect1;
                _fieldRect1.xMin = 150;
                    
                EditorGUI.LabelField(_rect1, new GUIContent("Grid Unit Size", "Set grid unit size here"));
                EditorGUI.MultiIntField(_fieldRect1, contents, BBEditorManager.GridDataSettingSo.GridUnitSizeArray);
                    
                GUILayout.EndHorizontal();
                
                BBEditorManager.GridDataSettingSo.GridOffset = EditorGUILayout.IntField("Grid Offset", BBEditorManager.GridDataSettingSo.GridOffset);
                        
                GUILayout.EndVertical();
                GUILayout.EndVertical();
            }

            Undo.RecordObject(BBEditorManager.GridDataSettingSo,"Undo");
        }
	}
}
#endif
