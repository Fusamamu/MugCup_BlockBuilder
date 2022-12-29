using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;

using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Scriptable;

namespace MugCup_BlockBuilder.Editor.GUI
{
    [CustomEditor(typeof(BlockBuilderManager))]
    [CanEditMultipleObjects]
    public class BlockBuilderManagerEditor : UnityEditor.Editor
    {
        private BlockBuilderManager blockBuilderManager;
        
        private SerializedProperty mode;

        private SerializedProperty gridDataSetting;
        private SerializedProperty meshBlockDataSetting;
        private SerializedProperty pathBlockDataSetting;

        private AnimBool displayCustomDataFields;
        
        private static GUIStyle toggleButtonStyleNormal  = null;
        private static GUIStyle toggleButtonStyleToggled = null;

        private static bool useDebug;

        private void OnEnable()
        {
            blockBuilderManager = (BlockBuilderManager)target;
            
            mode                 = serializedObject.FindProperty("Mode");
            gridDataSetting      = serializedObject.FindProperty("CustomGridDataSetting");
            meshBlockDataSetting = serializedObject.FindProperty("CustomBlockMeshData");
            pathBlockDataSetting = serializedObject.FindProperty("CustomPathBlockMeshData");

            displayCustomDataFields = new AnimBool();
            displayCustomDataFields.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            var _myGUIStyle = new GUIStyle {
                normal    =
                {
                    textColor  = Color.white,
                    background = CreateColorTexture(50, 50, new Color(0.7f, 0.51f, 0.2f))
                },
                
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            
            //Make Logo For Block Builder Packager
            //Make MugCup Logo
            EditorGUILayout.LabelField("Block Builder Package Manager", _myGUIStyle, GUILayout.Height(23));
            
            EditorGUILayout.Space(0.5f);
            EditorGUILayout.LabelField("Information", EditorStyles.boldLabel);
            
            EditorGUILayout.Space(0.5f);
            EditorGUILayout.HelpBox
               ($"Set Mode to Custom if you want to provide data with "    +
                $"your own custom grid data setting and mesh block data."  +
                $"Remain Default, Block Manager will use default setting " +
                $"that come with the package.", MessageType.None, true);
            
            EditorGUILayout.Space();
            blockBuilderManager.Mode = (BlockBuilderManager.ManagerMode)EditorGUILayout.EnumPopup("Mode Selection", blockBuilderManager.Mode);

            displayCustomDataFields.target = blockBuilderManager.Mode == BlockBuilderManager.ManagerMode.Custom;

            if (EditorGUILayout.BeginFadeGroup(displayCustomDataFields.faded))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Custom Data Fields", EditorStyles.boldLabel);
                
                blockBuilderManager.CustomGridDataSetting = (GridDataSettingSO)EditorGUILayout
                    .ObjectField(blockBuilderManager.CustomGridDataSetting, typeof(GridDataSettingSO), true);

                blockBuilderManager.CustomBlockMeshData   = (BlockMeshData)    EditorGUILayout
                    .ObjectField(blockBuilderManager.CustomBlockMeshData,    typeof(BlockMeshData),    true);
                
                blockBuilderManager.CustomPathBlockMeshData = (BlockMeshData)    EditorGUILayout
                    .ObjectField(blockBuilderManager.CustomPathBlockMeshData, typeof(BlockMeshData),    true);

                var _blockDataSetting = new BlockDataSetting
                (
                    blockBuilderManager.CustomGridDataSetting, 
                    blockBuilderManager.CustomBlockMeshData, 
                    blockBuilderManager.CustomPathBlockMeshData
                );

                blockBuilderManager.BlockManager.CurrentGridBlockBlockData.CacheData(_blockDataSetting);
                EditorUtility.SetDirty(blockBuilderManager);
                
                EditorGUILayout.Space();
                if (blockBuilderManager.CustomGridDataSetting == null)
                {
                    EditorGUILayout.HelpBox("Missing Custom Grid Data Setting Scriptable Object.", MessageType.Warning);
                }
                if (blockBuilderManager.CustomBlockMeshData == null)
                {
                    EditorGUILayout.HelpBox("Missing Custom Mesh Block Data Scriptable Object.", MessageType.Warning);
                }
            }
            EditorGUILayout.EndFadeGroup();

            if (UnityEngine.GUI.changed)
            {
                EditorUtility.SetDirty(blockBuilderManager);
                EditorSceneManager.MarkSceneDirty(blockBuilderManager.gameObject.scene);
            }
            
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Current Grid Data Setting", EditorStyles.boldLabel);
            
            var _gridDataSetting = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>(DataPath.GridDataSettingPath     );
            var _meshDataSetting = AssetDatabase.LoadAssetAtPath<BlockMeshData>    (DataPath.DefaultMeshBlockDataPath);

            EditorGUILayout.Space(0.5f);
            
            EditorGUILayout.BeginHorizontal(_myGUIStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Grid Unit Size", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Map Unit Size", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            DisplayInAdjacentTwoColumns("Unit Row",    _gridDataSetting.UnitRow   .ToString(), "Row",    _gridDataSetting.Row.   ToString());
            DisplayInAdjacentTwoColumns("Unit Column", _gridDataSetting.UnitColumn.ToString(), "Column", _gridDataSetting.Column.ToString());
            DisplayInAdjacentTwoColumns("Unit Height", _gridDataSetting.UnitHeight.ToString(), "Height", _gridDataSetting.Height.ToString());
           
            EditorGUILayout.Space();    
            if (GUILayout.Button("Block Builder Window"))
            {
                EditorWindow.GetWindow(typeof(BlockBuilderWindow), false, "Block Builder").Show();
            }
            
            if (toggleButtonStyleNormal == null)
            {
                toggleButtonStyleNormal  = "Button";
                toggleButtonStyleToggled = new GUIStyle(toggleButtonStyleNormal);
                toggleButtonStyleToggled.normal.background = toggleButtonStyleToggled.active.background;
            }
            
            // useDebug = GUILayout.Toggle(useDebug, "Toggle me !", "Button");
            //
            // if (useDebug)
            // {
            //     SceneView.duringSceneGui += OnScene;
            // }
            // else
            // {
            //     SceneView.duringSceneGui -= OnScene;
            // }

            if (GUILayout.Button("Init Blocks Position"))
            {
                var _blocks = GameObject.FindObjectsOfType<Block>();

                foreach (var _block in _blocks)
                {
                    _block.InitNodePosition();
                    EditorUtility.SetDirty(_block);
                }
            }
            
            //ToDo
            //Debug Mode
            //Save Data
        }
        
        private static void OnScene(SceneView _sceneview)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(20, 20, 300, 100));
            
            if (GUILayout.Button("Generate Grid Unit")) 
            {
                if (Application.isPlaying)
                {
                    
                }
            }

            // selectedBlock = EditorGUILayout.ObjectField("Selected Block", selectedBlock, typeof(GameObject), true) as GameObject;
            // blockPosition = EditorGUILayout.Vector3IntField("Block Position", blockPosition);
        
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private static void DisplayInAdjacentTwoColumns(string _c1, string _cc1, string _c2, string _cc2)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(_c1, _cc1);
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(_c2, _cc2);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        private static Texture2D CreateColorTexture(int _width, int _height, Color _color)
        {
            var _pixel = new Color[_width *_height];
 
            for(var _i = 0; _i < _pixel.Length; _i++)
                _pixel[_i] = _color;
 
            var _resultTexture = new Texture2D(_width, _height);
            
            _resultTexture.SetPixels(_pixel);
            _resultTexture.Apply();
 
            return _resultTexture;
        }
    }
}
