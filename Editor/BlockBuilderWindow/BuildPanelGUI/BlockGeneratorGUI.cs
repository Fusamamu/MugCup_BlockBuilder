#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MugCup_BlockBuilder.Editor;

namespace MugCup_BlockBuilder
{
	public static class BlockGeneratorGUI
	{
        private static bool blockGeneratorFoldout;
        
        public static void Display()
        {
            blockGeneratorFoldout = BBEditorStyling.DrawHeader(Color.clear, "Blocks Generator", blockGeneratorFoldout);

            if (blockGeneratorFoldout)
            {
                GUILayout.BeginHorizontal("GroupBox");
                
                EditorGUILayout.LabelField("Generate Map");

                GUILayout.BeginVertical();
                
                var _newStyle = new GUIStyle(GUI.skin.button);
             
                if (GUILayout.Button("Generate Map", _newStyle, GUILayout.Height(30)))
                {
                    Vector3Int _mapSize  = BBEditorManager.GridDataSettingSo.MapSize;
                    Vector3Int _unitSize = BBEditorManager.GridDataSettingSo.GridUnitSize;
                    //GridGenerator.GenerateMap(_mapSize, _unitSize);
                }

                if (GUILayout.Button("Generate Grid", _newStyle, GUILayout.Height(30)))
                {
                    var _gridBlockDataManager = BBEditorManager.BlockDataManager;
                    Undo.RecordObject(_gridBlockDataManager, "GridBlockDataManager Changed");

                    BBEditorManager.BlockManager.GenerateGrid();
                  
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_gridBlockDataManager);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(7);
                if(GUILayout.Button("Save", EditorStyles.miniButton, GUILayout.Width(80)))
                {
                    
                }
                if(GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(80)))
                {
                    var _blocks = GameObject.FindGameObjectsWithTag("Block");
                    
                    foreach(var _block in _blocks)
                        Object.DestroyImmediate(_block);

                    BBEditorManager.BlockManager.GridBlockDataManager.GridNodeData.ClearData();

                    var _textParent   = GameObject.Find("[-------Grid Position Text-------]");
                    var _blocksParent = GameObject.Find("[-------------Blocks-------------]");
                    
                    if(_textParent)
                        Object.DestroyImmediate(_textParent);
                    if(_blocksParent)
                        Object.DestroyImmediate(_blocksParent);
                }  
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
	}
}
#endif
