using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Editor;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_PathFinder.Runtime;
using BlockBuilder.Scriptable;

namespace MugCup_BlockBuilder
{
    public static class BuildPanelGUI
    {
        private static Block[] blocks;
        private static InterfaceSetting   interfaceSetting;
        private static GridDataSettingSO  gridDataSettingSo;
        
        private static bool blockGeneratorFoldout;
        private static bool gridElementGeneratorFoldout;
        
        private static Vector2 scrollPosition;
        
        private static AnimBool displayBuilderMode;
        
        private static GUIContent[] contents = new GUIContent[]
        {
            new GUIContent("Row"), new GUIContent("Column"), new GUIContent("Height")
        };

        public static void Init()
        {
         
        }
        
        public static void Display()
        {
            BBEditorManager.InterfaceSetting.MapSettingFoldout 
                = BBEditorStyling.DrawHeader(Color.yellow, "Grid Data Setting", BBEditorManager.InterfaceSetting.MapSettingFoldout);

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

            var _newStyle = new GUIStyle(UnityEngine.GUI.skin.button);

            Undo.RecordObject(BBEditorManager.GridDataSettingSo,"Undo");
            
            blockGeneratorFoldout = BBEditorStyling.DrawHeader(Color.cyan, "Blocks Generator", blockGeneratorFoldout);

            if (blockGeneratorFoldout)
            {
                GUILayout.BeginHorizontal("GroupBox");
                
                EditorGUILayout.LabelField("Generate Map");

                GUILayout.BeginVertical();
             
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
                
                // if (GUILayout.Button("Generate Grid Elements", _newStyle, GUILayout.Height(30)))
                // {
                //     var _gridBlockDataManager = BBEditorManager.BlockDataManager;
                //     Undo.RecordObject(_gridBlockDataManager, "GridBlockDataManager Changed");
                //     
                //     
                //     PrefabUtility.RecordPrefabInstancePropertyModifications(_gridBlockDataManager);
                // }
                //
                // if (GUILayout.Button("Generate Volume Points", _newStyle, GUILayout.Height(30)))
                // {
                //     Vector3Int _gridUnitSize  = BBEditorManager.GridDataSettingSo.GridUnitSize;
                //     GameObject _volumePoints  = new GameObject("[Volume Points]");
                //     
                //     volumePoints = VolumePointGenerator.GeneratedVolumePoints(_gridUnitSize, 0.1f, _volumePoints);
                //
                //     var _blocks = BBEditorManager.BlockManager.GridBlockDataManager.GridNodeData.AvailableNodes<Block>().ToArray();
                //
                //     if (_blocks.Length > 0)
                //     {
                //         foreach (var _block in _blocks)
                //         {
                //             var _coord  = _block.NodeGridPosition;
                //             var _points = VolumePointGenerator.GetVolumePoint(_coord, _gridUnitSize, volumePoints);
                //             
                //             _block.SetVolumePoints(_points);
                //         }
                //
                //         foreach (var _point in volumePoints)
                //         {
                //             _point.SetAdjacentBlocks(_blocks, _gridUnitSize);
                //         }
                //     }
                // }
                
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
            
            gridElementGeneratorFoldout = BBEditorStyling.DrawHeader(new Color(30, 30, 30), "Grid Elements Generator", gridElementGeneratorFoldout);

            if (gridElementGeneratorFoldout)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                
                GUILayout.BeginHorizontal("GroupBox");
                
                EditorGUILayout.LabelField("Grid Element");
                
                if (GUILayout.Button("Generate", _newStyle, GUILayout.Height(30)))
                {
                    var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                    Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                    _gridElementDataManager.Initialized();
                    _gridElementDataManager.GenerateGrid();
                    
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("GroupBox");
                
                EditorGUILayout.LabelField("Volume Points");
                
                if (GUILayout.Button("Generate", _newStyle, GUILayout.Height(30)))
                {
                    var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                    Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                    _gridElementDataManager.GenerateVolumePoints();
                    
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                    
                    
                    // Vector3Int _gridUnitSize  = BBEditorManager.GridDataSettingSo.GridUnitSize;
                    //
                    // var _volumePoints  = new GameObject("[Volume Points]");
                    //
                    // volumePoints = VolumePointGenerator.GeneratedVolumePoints(_gridUnitSize, 0.1f, _volumePoints);
                    //
                    // var _blocks = BBEditorManager.BlockManager.GridBlockDataManager.GridNodeData.AvailableNodes<Block>().ToArray();
                    //
                    // if (_blocks.Length > 0)
                    // {
                    //     foreach (var _block in _blocks)
                    //     {
                    //         var _coord  = _block.NodeGridPosition;
                    //         var _points = VolumePointGenerator.GetVolumePoints(_coord, _gridUnitSize, volumePoints);
                    //         
                    //         _block.SetVolumePoints(_points);
                    //     }
                    //
                    //     foreach (var _point in volumePoints)
                    //     {
                    //         //_point.SetAdjacentBlocks(_blocks, _gridUnitSize);
                    //     }
                    // }
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(7);
            
                if(GUILayout.Button("Clear Grid", EditorStyles.miniButton, GUILayout.Width(80)))
                {
                    var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                    Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                    _gridElementDataManager.ClearGrid();
                    
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                }  
                
                if(GUILayout.Button("Clear Volume Points", EditorStyles.miniButton, GUILayout.Width(80)))
                {
                    var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                    Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                    _gridElementDataManager.ClearVolumePoints();
                    
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                }  
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            
            blockGeneratorFoldout = BBEditorStyling.DrawHeader(Color.magenta, "Blocks Editor", blockGeneratorFoldout);

            EditorGUILayout.BeginVertical("GroupBox");
            
            EditorGUILayout.HelpBox("Select desired edit mode. Use add and remove tab below to start edit blocks", MessageType.Info);
            
            BBEditorManager.InterfaceSetting.CurrentEditMode = (InterfaceSetting.EditMode)EditorGUILayout.EnumPopup("Edit mode selection:", BBEditorManager.InterfaceSetting.CurrentEditMode);


            EditorGUILayout.BeginVertical("GroupBox");
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true);

            var _allAssets = Resources.LoadAll<GridNode>("Prefabs/Grass");
            
            var _previewWidth = 100f;
            var _newRect = new Rect(0, 0, _previewWidth, _previewWidth);
            
            for (var _i = 0; _i < _allAssets.Length; _i++)
            {
                var _cube = _allAssets[_i].gameObject;
                
                if(_cube == null) continue;
                
                _newRect.x = _i * (_previewWidth + 5);

                if (_i > 3)
                {
                    _newRect.x = (_i - 4) * (_previewWidth + 5);
                    _newRect.y = _previewWidth + 20;
                }
                
                var _tex = BlockPreviewEditor.CreatePreviewTexture(_newRect, _cube);
            
                if (_newRect.Contains(Event.current.mousePosition)) 
                {
                    EditorGUI.DrawPreviewTexture(_newRect, _tex);
                    EditorGUI.DrawRect(_newRect, new Color(1f, 1f, 1f, 0.5f));
                    UnityEngine.GUI.backgroundColor = Color.blue;

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        BlockPlacementTools.SelectedIndex = _i;
                        BlockPlacementTools.SelectedBlock = _cube;
                    }
                }
                else
                {
                    if (!BlockPlacementTools.IsSlotSelected(_i))
                    {
                        EditorGUI.DrawPreviewTexture(_newRect, _tex);
                        UnityEngine.GUI.backgroundColor = Color.white;
                    }
                    else
                    {
                        EditorGUI.DrawPreviewTexture(_newRect, _tex);
                        EditorGUI.DrawRect(_newRect, new Color(1f, 1f, 1f, 0.5f));
                        UnityEngine.GUI.backgroundColor = Color.blue;
                    }
                }

                var _labelRect = _newRect;
                _labelRect.y += _previewWidth + 3f;
                _labelRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(_labelRect, _cube.name);
                
            }
            
            UnityEngine.GUI.backgroundColor = Color.white;

            
            // var _cube = Resources.Load<GameObject>("Prefabs/Towers/Tower_Castle");
            // if(_cube == null)
            //     Debug.LogWarning("Missing Cube");
            //
            // var _previewWidth = 100f;
            // var _newRect = new Rect(0, 0, _previewWidth, _previewWidth);
            //
            // for (var _i = 0; _i < 10; _i++)
            // {
            //     _newRect.x = _i * (_previewWidth + 5);
            //
            //     if (_i > 3)
            //     {
            //         _newRect.x = (_i - 4) * (_previewWidth + 5);
            //         _newRect.y = _previewWidth + 20;
            //     }
            //     
            //     var _tex = BlockPreviewEditor.CreatePreviewTexture(_newRect, _cube);
            //
            //     if (_newRect.Contains(Event.current.mousePosition)) 
            //     {
            //         EditorGUI.DrawPreviewTexture(_newRect, _tex);
            //         EditorGUI.DrawRect(_newRect, new Color(1f, 1f, 1f, 0.5f));
            //         UnityEngine.GUI.backgroundColor = Color.blue;
            //
            //         if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            //         {
            //             BlockPlacementTools.SelectedIndex = _i;
            //             BlockPlacementTools.SelectedBlock = _cube;
            //         }
            //     }
            //     else
            //     {
            //         if (!BlockPlacementTools.IsSlotSelected(_i))
            //         {
            //             EditorGUI.DrawPreviewTexture(_newRect, _tex);
            //             UnityEngine.GUI.backgroundColor = Color.white;
            //         }
            //         else
            //         {
            //             EditorGUI.DrawPreviewTexture(_newRect, _tex);
            //             EditorGUI.DrawRect(_newRect, new Color(1f, 1f, 1f, 0.5f));
            //             UnityEngine.GUI.backgroundColor = Color.blue;
            //         }
            //     }
            //
            //     var _labelRect = _newRect;
            //     _labelRect.y += _previewWidth + 3f;
            //     _labelRect.height = EditorGUIUtility.singleLineHeight;
            //     EditorGUI.LabelField(_labelRect, _cube.name);
            //     
            // }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            
            BBResource.Initialized();
            BBEditorManager.InterfaceSetting.CurrentEditMode 
                = (InterfaceSetting.EditMode)GUILayout.Toolbar((int)BBEditorManager.InterfaceSetting.CurrentEditMode, BBResource.Tabs, GUILayout.Height(30), GUILayout.Width(100));
            
            
            EditorGUILayout.LabelField("Block Element Placement");
            string[] _blockPlacementTools = { "Place Block Element", "Remove Block Element" };
            BBEditorManager.InterfaceSetting.BlockPlacementToolTabSelection 
                = GUILayout.Toolbar(BBEditorManager.InterfaceSetting.BlockPlacementToolTabSelection, _blockPlacementTools, GUILayout.Height(30));

            EditorGUILayout.LabelField("Edit Blocks");
            string[] _buildingToolTabs = { "Add Block", "Subtract Block" };
            BBEditorManager.InterfaceSetting.BuildToolTabSelection 
                = GUILayout.Toolbar(BBEditorManager.InterfaceSetting.BuildToolTabSelection, _buildingToolTabs, GUILayout.Height(30));
            
            EditorGUILayout.LabelField("Edit Road Path Blocks");
            string[] _pathBuildingToolTabs = { "Add Road Path", "Remove Road Path" };
            BBEditorManager.InterfaceSetting.RoadBuildToolTabSelection 
                = GUILayout.Toolbar(BBEditorManager.InterfaceSetting.RoadBuildToolTabSelection, _pathBuildingToolTabs, GUILayout.Height(30));
            
            EditorGUILayout.EndVertical();
        }
        
        private static void DisplayBuilderModeSelectionInApplication()
        {
            displayBuilderMode.target = Application.isPlaying;

            if (EditorGUILayout.BeginFadeGroup(displayBuilderMode.faded))
            {
                GUILayout.Label("Builder Mode", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Edit"   , EditorStyles.miniButton))
                {
                    var _stateManager = BlockBuilderManager.Instance.GetManager<StateManager>();
                    _stateManager.RequestChangeState(BuilderMode.EditMode);
                }

                if (GUILayout.Button("Handler", EditorStyles.miniButton))
                {
                    var _stateManager = BlockBuilderManager.Instance.GetManager<StateManager>();
                    _stateManager.RequestChangeState(BuilderMode.HandlerMode);
                }
                    
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
        }
    }
}
