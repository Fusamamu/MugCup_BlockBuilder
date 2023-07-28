#if UNITY_EDITOR
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
        private BlockBuilderManager  blockBuilderManager;

        private BlockManager         blockManager;
        private GridBlockDataManager gridBlockDataManager;

        private GridElementManager     gridElementManager;
        private GridElementDataManager gridElementDataManager;
        
        private AnimBool displayCustomDataSetting;
        private AnimBool displayBlobTileSetting;
        private AnimBool displayCornerTileSetting;
        
        private static GUIStyle toggleButtonStyleNormal ;
        private static GUIStyle toggleButtonStyleToggled;

        private static bool useDebug;

        private void OnEnable()
        {
            blockBuilderManager  = (BlockBuilderManager)target;

            blockManager         = blockBuilderManager.BlockManager;
            gridBlockDataManager = blockManager.GridBlockDataManager;
            
            gridElementManager     = blockBuilderManager.GridElementManager;
            gridElementDataManager = gridElementManager.GridElementDataManager;

            displayCustomDataSetting = new AnimBool();
            displayCustomDataSetting.valueChanged.AddListener(Repaint);

            displayBlobTileSetting = new AnimBool();
            displayBlobTileSetting.valueChanged.AddListener(Repaint);

            displayCornerTileSetting = new AnimBool();
            displayCornerTileSetting.valueChanged.AddListener(Repaint);
            
            BBEditorManager.Initialize();
        }

        private void OnDisable()
        {
            displayCustomDataSetting.valueChanged.RemoveAllListeners();
            displayBlobTileSetting  .valueChanged.RemoveAllListeners();
            displayCornerTileSetting.valueChanged.RemoveAllListeners();
        }

        public override void OnInspectorGUI()
        {
            var _myGUIStyle = new GUIStyle {
                normal    =
                {
                    textColor  = Color.white,
                    background = CreateColorTexture(50, 50, new Color(0.3f, 0.3f, 0.3f))
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
            //blockBuilderManager.SelectedBuildType = (BlockBuilderManager.BuildType  )EditorGUILayout.EnumPopup("Build Type", blockBuilderManager.SelectedBuildType);
            blockBuilderManager.Mode = (BlockBuilderManager.ManagerMode)EditorGUILayout.EnumPopup("Mode Selection", blockBuilderManager.Mode);

            displayCustomDataSetting.target = blockBuilderManager.Mode == BlockBuilderManager.ManagerMode.CUSTOM;

            if (EditorGUILayout.BeginFadeGroup(displayCustomDataSetting.faded))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Custom Grid Data Setting", EditorStyles.boldLabel);

                switch (BBEditorManager.InterfaceSetting.SelectedBuildType)
                {
                    case InterfaceSetting.BuildType.BLOB_TILE:
                        gridBlockDataManager.SetGridDataSetting
                        (
                            (GridDataSettingSO)EditorGUILayout.ObjectField(gridBlockDataManager.GridDataSetting, typeof(GridDataSettingSO), true)
                        );
                        
                        EditorUtility.SetDirty(gridBlockDataManager);
                        
                        EditorGUILayout.Space();
                        if (gridBlockDataManager.GridDataSetting == null)
                        {
                            EditorGUILayout.HelpBox("Missing Custom Grid Data Setting Scriptable Object.", MessageType.Warning);
                        }
                        if (gridBlockDataManager.BlockMeshData == null)
                        {
                            EditorGUILayout.HelpBox("Missing Custom Mesh Block Data Scriptable Object.", MessageType.Warning);
                        }
                        break;
                    
                    case InterfaceSetting.BuildType.MARCHING_CUBE:

                        gridElementDataManager.SetGridDataSetting
                        (
                            (GridDataSettingSO)EditorGUILayout.ObjectField(gridElementDataManager.GridDataSetting, typeof(GridDataSettingSO), true)
                        );

                        EditorUtility.SetDirty(gridElementDataManager);
                        
                        EditorGUILayout.Space();
                        if (gridElementDataManager.GridDataSetting == null)
                        {
                            EditorGUILayout.HelpBox("Missing Custom Grid Data Setting Scriptable Object.", MessageType.Warning);
                        }
                        break;
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

            if (blockBuilderManager.Mode == BlockBuilderManager.ManagerMode.CUSTOM)
            {
                _gridDataSetting = blockBuilderManager.BlockManager.GridBlockDataManager.GridDataSetting;
            }

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

            displayBlobTileSetting.target = BBEditorManager.InterfaceSetting.SelectedBuildType == InterfaceSetting.BuildType.BLOB_TILE;
            
            if (EditorGUILayout.BeginFadeGroup(displayBlobTileSetting.faded))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Blob Mesh Data", EditorStyles.boldLabel);
                
                var _customBlockMeshData   = (BlockMeshData)    EditorGUILayout.ObjectField(gridBlockDataManager.BlockMeshData  , typeof(BlockMeshData)    , true);
                var _customPathMeshData    = (BlockMeshData)    EditorGUILayout.ObjectField(gridBlockDataManager.PathMeshData   , typeof(BlockMeshData)    , true);
                gridBlockDataManager.SetBlockMeshData  (_customBlockMeshData);
                gridBlockDataManager.SetPathMeshData   (_customPathMeshData);
                
                EditorUtility.SetDirty(gridBlockDataManager);
            }
            EditorGUILayout.EndFadeGroup();
            
            displayCornerTileSetting.target = BBEditorManager.InterfaceSetting.SelectedBuildType == InterfaceSetting.BuildType.MARCHING_CUBE;
            
            if (EditorGUILayout.BeginFadeGroup(displayCornerTileSetting.faded))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Corner Mesh Data", EditorStyles.boldLabel);
               
            }
            EditorGUILayout.EndFadeGroup();
           
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

            if (GUILayout.Button("Init Blocks Position"))
            {
                var _blocks = GameObject.FindObjectsOfType<Block>();

                foreach (var _block in _blocks)
                {
                    _block.InitNodePosition();
                    EditorUtility.SetDirty(_block);
                }
            }
            
            EditorGUILayout.Space();
            if (gridElementManager == null)
            {
                EditorGUILayout.HelpBox("Missing Grid Element Manager!", MessageType.Warning);
            }
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
#endif
