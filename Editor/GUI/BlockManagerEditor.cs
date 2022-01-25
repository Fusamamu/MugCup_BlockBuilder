using System;
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Core.Scriptable;
using UnityEngine;
using UnityEditor;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;

namespace MugCup_BlockBuilder.Editor.GUI
{
    [CustomEditor(typeof(BlockManager))]
    [CanEditMultipleObjects]
    public class BlockManagerEditor : UnityEditor.Editor
    {
        private BlockManager blockManager;
        
        private SerializedProperty mode;

        private SerializedProperty gridDataSetting;
        private SerializedProperty meshBlockDataSetting;

        private void OnEnable()
        {
            blockManager = (BlockManager)target;
            
            mode                 = serializedObject.FindProperty("Mode");
            gridDataSetting      = serializedObject.FindProperty("CustomGridDataSetting");
            meshBlockDataSetting = serializedObject.FindProperty("CustomBlockMeshData");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox
               ($"Set Mode to Custom if you want to provide data with "    +
                $"your own custom grid data setting and mesh block data."  +
                $"Remain Default, Block Manager will use default setting " +
                $"that come with the package.", MessageType.None, true);
            
            EditorGUILayout.Space();
            blockManager.Mode = (BlockManager.ManagerMode)EditorGUILayout.EnumPopup("Mode Selection", blockManager.Mode);

            if (blockManager.Mode  == BlockManager.ManagerMode.Custom)
            {
              
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Custom Data Fields", EditorStyles.boldLabel);
                
                blockManager.CustomGridDataSetting = (GridDataSettingSO)EditorGUILayout
                    .ObjectField(blockManager.CustomGridDataSetting, typeof(GridDataSettingSO), true);

                blockManager.CustomBlockMeshData   = (BlockMeshData)    EditorGUILayout
                    .ObjectField(blockManager.CustomBlockMeshData,    typeof(BlockMeshData),    true);
                
                EditorGUILayout.Space();
                if (blockManager.CustomGridDataSetting == null)
                {
                    EditorGUILayout.HelpBox("Missing Custom Grid Data Setting Scriptable Object.", MessageType.Warning);
                }
                if (blockManager.CustomBlockMeshData == null)
                {
                    EditorGUILayout.HelpBox("Missing Custom Mesh Block Data Scriptable Object.", MessageType.Warning);
                }

            }
                
            if (GUILayout.Button("Block Builder Window"))
            {
                    
            }
        }
    }
}
