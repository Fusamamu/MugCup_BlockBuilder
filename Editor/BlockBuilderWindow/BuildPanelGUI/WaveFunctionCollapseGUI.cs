#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Editor;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class WaveFunctionCollapseGUI 
    {
        private static bool waveFunctionCollapseFoldout;
        
        public static void Display()
        {
            waveFunctionCollapseFoldout = BBEditorStyling.DrawHeader(Color.clear, "Wave Function Collapse Generator", waveFunctionCollapseFoldout);

            if (waveFunctionCollapseFoldout)
            {
                var _generateButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 100,
                };
                
                var _clearButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 100,
                };
                
                EditorGUILayout.BeginVertical("GroupBox");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Module Slots");
                                
                        if (GUILayout.Button("Generate", _generateButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.GenerateDualGrid();
                                    
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                        
                        if (GUILayout.Button("Clear", _generateButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.ClearDualGridElements();
                                    
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Start Collapse"))
                    {
                        BBEditorManager.GridElementDataManager.ModuleSlotData.Initialized();
                        BBEditorManager.GridElementDataManager.ModuleSlotData.CollapseAll();
                    }
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(15);
                    if(GUILayout.Button("Clear All", EditorStyles.miniButton, GUILayout.Width(80)))
                    {
                        var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                        Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                        _gridElementDataManager.ClearGrid();
                        _gridElementDataManager.ClearDualGridElements();
                        
                        PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                    }  
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
    }
}
#endif
