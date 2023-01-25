using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor.GUI
{
    public static class BuildPanelGUI
    {
        private static Block[] blocks;
        private static InterfaceSetting   interfaceSetting;
        private static GridDataSettingSO  gridDataSettingSo;
        
        private static void DisplayBuildPanel()
        {
            interfaceSetting.MapSettingFoldout = EditorGUILayout.Foldout(interfaceSetting.MapSettingFoldout, "Map Data Setting");
            
            if (interfaceSetting.MapSettingFoldout)
            {
                EditorGUILayout.LabelField("Column", EditorStyles.boldLabel, GUILayout.Width(45), GUILayout.ExpandWidth(false));
                
                gridDataSettingSo.Column = EditorGUILayout.IntField(gridDataSettingSo.Column, GUILayout.ExpandWidth(true));
       
                // SerializedObject _gridDataSO = new SerializedObject(gridDataSetting);
                // SerializedProperty _row = _gridDataSO.FindProperty("Row");
                // EditorGUILayout.PropertyField(_row);
                
                GUILayout.BeginVertical("HelpBox");
                    GUILayout.Label("Map Data Setting");
                    
                    GUILayout.BeginVertical("GroupBox");
                    
                    gridDataSettingSo.MapSize      = EditorGUILayout.Vector3IntField("Map Size",      gridDataSettingSo.MapSize);
                    gridDataSettingSo.GridUnitSize = EditorGUILayout.Vector3IntField("Map Unit Size", gridDataSettingSo.GridUnitSize);
                    
                    GUILayout.BeginHorizontal();

                    int _newRow    = gridDataSettingSo.MapSize.x;
                    int _newColumn = gridDataSettingSo.MapSize.z;
                    int _newHeight = gridDataSettingSo.MapSize.y;
                    
                    float originalValue = EditorGUIUtility.labelWidth;
                    
                    EditorGUIUtility.labelWidth = 40;
                    _newRow    = EditorGUILayout.IntField("Row",    _newRow   );
                    _newColumn = EditorGUILayout.IntField("Column", _newColumn);
                    _newHeight = EditorGUILayout.IntField("Height", _newHeight);
                    EditorGUIUtility.labelWidth = originalValue;

                    gridDataSettingSo.MapSize = new Vector3Int(_newRow, _newHeight, _newColumn);
                    
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical();
                    
                
                GUILayout.EndVertical();
            }

            // if (blocks.Length > 0)
            // {
            //     EditorGUILayout.ObjectField(blocks[0].gameObject, typeof(Block), true);
            // }

            GUIStyle newStylee = new GUIStyle(UnityEngine.GUI.skin.button);
            newStylee.margin   = new RectOffset(10, 10, 10, 10);
            
            Undo.RecordObject(gridDataSettingSo,"Undo");
            if (GUILayout.Button("Generate Map", newStylee, GUILayout.Height(30)))
            {
                Vector3Int _mapSize  = gridDataSettingSo.MapSize;
                Vector3Int _unitSize = gridDataSettingSo.GridUnitSize;
               // GridGenerator.GenerateMap(_mapSize, _unitSize);
            }

            if (GUILayout.Button("Generate Grid", newStylee, GUILayout.Height(30)))
            {
                Vector3Int _mapSize  = gridDataSettingSo.MapSize;
                Vector3Int _unitSize = gridDataSettingSo.GridUnitSize;
                GameObject _mainMap = new GameObject("Main Map");
                
                GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                
                // blocks = GridGenerator.GenerateGridBlocks(_unitSize, _blockPrefab, _mainMap);
                // Object.DestroyImmediate(_blockPrefab);
            }
            
            if(GUILayout.Button("Delete Blocks", newStylee, GUILayout.Height(30)))
            {
                var _blocks = GameObject.FindGameObjectsWithTag("Block");
                foreach(GameObject _block in _blocks)
                    Object.DestroyImmediate(_block);
            }

            string[] _buildingToolTabs = {"Add Block", "Subtract Block"};
            interfaceSetting.BuildToolTabSelection = GUILayout.Toolbar(interfaceSetting.BuildToolTabSelection, _buildingToolTabs, GUILayout.Height(30));
        }
    }
}
