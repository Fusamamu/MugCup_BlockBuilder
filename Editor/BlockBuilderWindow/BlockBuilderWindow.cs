using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using BlockBuilder.Core.Scriptable;
using MugCup_PathFinder.Runtime;
using UnityEditor.AnimatedValues;
#endif
using BlockBuilder.Core;
using BlockBuilder.Scriptable;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using Utilities = MugCup_PathFinder.Runtime.Utilities;

#if UNITY_EDITOR
namespace MugCup_BlockBuilder.Editor
{
    public class BlockBuilderWindow : EditorWindow
    {
        private static Vector2 scrollPosition;
        
        private static VolumePoint[] volumePoints;
        
        private static AnimBool displayBuilderMode;
        
        [MenuItem("Tools/Block Builder/Open Block Builder Window", false, 16)]
        public static void ShowWindow() => GetWindow(typeof(BlockBuilderWindow), false, "Block Builder").Show();

        private void OnEnable()
        {
            BBEditorManager.Initialize();
            
            AssetManager.LoadAssets();

            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;

            displayBuilderMode = new AnimBool();
            displayBuilderMode.valueChanged.AddListener(Repaint);
            
            BlockPreviewEditor.Init();
        }

        private void OnGUI()
        {
            GUILayout.Label("BlockBuilder", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            string[] _tabCaptions = {"Build", "Paint", "Setting", "Tools"};
                
            BBEditorManager.InterfaceSetting.CurrentMainTapSelection 
                = GUILayout.Toolbar(BBEditorManager.InterfaceSetting.CurrentMainTapSelection, _tabCaptions, new GUIStyle(EditorStyles.toolbarButton), UnityEngine.GUI.ToolbarButtonSize.Fixed);
            
            GUILayout.Space(10);
           
            switch (BBEditorManager.InterfaceSetting.CurrentMainTapSelection)
            {
                case 0:
                    DisplayBuildPanel();
                    break;
                case 1:
                    break;
                case 2:
                    DisplaySettingPanel();
                    break;
                case 3:
                    DisplayToolPanel();
                    break;;
            }
        }

        private static GUIContent[] contents = new GUIContent[]
        {
            new GUIContent("Row"), new GUIContent("Column"), new GUIContent("Height")
        };

        private static bool foldout;

        private void DisplayBuildPanel()
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
            
            foldout = BBEditorStyling.DrawHeader(Color.cyan, "Blocks Initialization", foldout);

            if (foldout)
            {
                GUILayout.BeginHorizontal("GroupBox");
                
                EditorGUILayout.LabelField("Generate Map");

                GUILayout.BeginVertical();
             
                if (GUILayout.Button("Generate Map", _newStyle, GUILayout.Height(30)))
                {
                    Vector3Int _mapSize  = BBEditorManager.GridDataSettingSo.MapSize;
                    Vector3Int _unitSize = BBEditorManager.GridDataSettingSo.GridUnitSize;
                    GridBlockGenerator.GenerateMap(_mapSize, _unitSize);
                }

                if (GUILayout.Button("Generate Grid", _newStyle, GUILayout.Height(30)))
                {
                    var _gridBlockDataManager = BBEditorManager.BlockDataManager;
                    Undo.RecordObject(_gridBlockDataManager, "GridBlockDataManager Changed");
                    
                    Vector3Int _mapSize  = BBEditorManager.GridDataSettingSo.MapSize;
                    Vector3Int _unitSize = BBEditorManager.GridDataSettingSo.GridUnitSize;

                    BBEditorManager.BlockManager.GenerateGridBlocks();
                  
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_gridBlockDataManager);
                }

                if (GUILayout.Button("Generate Volume Points", _newStyle, GUILayout.Height(30)))
                {
                    Vector3Int _gridUnitSize  = BBEditorManager.GridDataSettingSo.GridUnitSize;
                    GameObject _volumePoints  = new GameObject("[Volume Points]");
                    
                    volumePoints = VolumePointGenerator.GeneratedVolumePoints(_gridUnitSize, 0.1f, _volumePoints);

                    var _blocks = BBEditorManager.BlockManager.CurrentGridBlockBlockData.AvailableNodes<Block>().ToArray();

                    if (_blocks.Length > 0)
                    {
                        foreach (var _block in _blocks)
                        {
                            var _coord  = _block.NodePosition;
                            var _points = VolumePointGenerator.GetVolumePoint(_coord, _gridUnitSize, volumePoints);
                            
                            _block.SetVolumePoints(_points);
                        }

                        foreach (var _point in volumePoints)
                        {
                            _point.SetAdjacentBlocks(_blocks, _gridUnitSize);
                        }
                    }
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
                        DestroyImmediate(_block);

                    BBEditorManager.BlockManager.CurrentGridBlockBlockData.ClearGridUnitNodeBases();

                    var _textParent   = GameObject.Find("[-------Grid Position Text-------]");
                    var _blocksParent = GameObject.Find("[-------------Blocks-------------]");
                    
                    if(_textParent)
                        DestroyImmediate(_textParent);
                    if(_blocksParent)
                        DestroyImmediate(_blocksParent);
                }  
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                
            }
            
            foldout = BBEditorStyling.DrawHeader(Color.magenta, "Blocks Editor", foldout);

            EditorGUILayout.BeginVertical("GroupBox");
            
            EditorGUILayout.HelpBox("Select desired edit mode. Use add and remove tab below to start edit blocks", MessageType.Info);
            
            BBEditorManager.InterfaceSetting.CurrentEditMode = (InterfaceSetting.EditMode)EditorGUILayout.EnumPopup("Edit mode selection:", BBEditorManager.InterfaceSetting.CurrentEditMode);


            EditorGUILayout.BeginVertical("GroupBox");
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true);

            var _previewWidth = 100f;
            
            var _cube = Resources.Load<GameObject>("Prefabs/Towers/Tower_Castle");
            if(_cube == null)
                Debug.LogWarning("Missing Cube");

            var _newRect = new Rect(0, 0, _previewWidth, _previewWidth);

            for (var _i = 0; _i < 10; _i++)
            {
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
            
            DisplayBuilderModeSelectionInApplication();
            
            
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

        private static void DisplaySettingPanel()
        {
            BBEditorManager.GridDataSettingSo = (GridDataSettingSO)EditorGUILayout.ObjectField("Grid Data Setting", BBEditorManager.GridDataSettingSo, typeof(GridDataSettingSO), true);
            
            var _meshData = (BlockMeshData)EditorGUILayout.ObjectField("Block Mesh Data Setting", null, typeof(BlockMeshData), true);
            var _material = (Material)     EditorGUILayout.ObjectField("Default Block Material" , null, typeof(Material)     , true);
        }

        private static void DisplayToolPanel()
        {
            if (GUILayout.Button("Update Nodes Position"))
            {
                var _gridNodes = FindObjectsOfType<GridNode>();

                foreach (var _node in _gridNodes)
                {
                    _node.SetNodeWorldPosition(_node.transform.position);
                    _node.SetNodePosition(MugCup_BlockBuilder.Runtime.Utilities.CastVec3ToVec3Int(_node.NodeWorldPosition));
                    EditorUtility.SetDirty(_node);
                }
            }

            if (GUILayout.Button("Update Node Reference in Grid"))
            {
                var _gridNodes = FindObjectsOfType<GridNode>();
                foreach (var _node in _gridNodes)
                {
                    var _gridData = FindObjectOfType<GridNodeData>();
                    if (_gridData != null)
                    {
                        _node.AddSelfToGrid(_gridData);
                        EditorUtility.SetDirty(_gridData);
                    }
                }
            }
        }

        private void OnScene(SceneView _sceneView)
        {
            if(Application.isPlaying) return;
            
            ProcessMouseEnterLeaveSceneView();
            
            Event _currentEvent = Event.current; 
            Ray _ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);

            switch (BBEditorManager.InterfaceSetting.CurrentMainTapSelection)
            {
                case 0: /*Build Mode*/

                    if(BBEditorManager.InterfaceSetting.CurrentEditMode == InterfaceSetting.EditMode.None) return;
                    
                    if (!Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity)) return;
                    
                    GetSelectedFace(_hit);
                    
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    
                    switch (BBEditorManager.InterfaceSetting.CurrentEditMode)
                    {
                        case InterfaceSetting.EditMode.BlockPlacement:
                            
                            UpdateVisualizePointer(_hit, Visualizer.PointerType.Block);
                            BlockPlacementTools.UpdateBlockBuildTools(_currentEvent, _ray);
                            break;
                        
                        case InterfaceSetting.EditMode.EditBlocks:
                            
                            UpdateVisualizePointer(_hit, Visualizer.PointerType.Block);
                            BlockEditorTools.UpdateBlockBuildTools(_currentEvent, _ray);
                            break;
                        
                        case InterfaceSetting.EditMode.EditRoads:
                            
                            UpdateVisualizePointer(_hit, Visualizer.PointerType.Path);
                            PathEditorTools.UpdateRoadBuildTools(_currentEvent, _ray);
                            break;
                    }
                    break;
                case 1:
                    Visualizer.ClearPointer();
                    break;
                case 2:
                    Visualizer.ClearPointer();
                    break;
                case 3:
                    Visualizer.ClearPointer();
                    break;
            }
        }
        
        private static void GetSelectedFace(RaycastHit _hit)
        {
            GridBlockGenerator.SelectedFace = BlockFaceUtil.GetSelectedFace(_hit);
        }
        
        private static void UpdateVisualizePointer(RaycastHit _hit, Visualizer.PointerType _pointerType)
        {
            Vector3 _centerPos  = _hit.collider.gameObject.transform.position;

            Visualizer.GetPointerReference(_pointerType).transform.position = _centerPos;

            Vector3 _faceCenterPos   = _centerPos     + _hit.normal / 2; //Should be propotion to Cube Size;
            Vector3 _extendedLinePos = _faceCenterPos + _hit.normal * 2f;
            
            Visualizer.DrawLine(_faceCenterPos, _extendedLinePos, Color.yellow, 2f);
            Handles.DrawWireDisc(_faceCenterPos, _hit.normal, 0.3f, 2f);
        }
        
        private void ProcessMouseEnterLeaveSceneView()
        {
            // If mouse enters SceneView window, show visualizer
            if (Event.current.type == EventType.MouseEnterWindow)
            {
               
            }
                
            // If mouse leaves SceneView window
            if (Event.current.type == EventType.MouseLeaveWindow)
            {
                Visualizer.ClearPointer();
            }
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnScene;
            
            EditorUtility.SetDirty(BBEditorManager.InterfaceSetting);
            BlockPreviewEditor.Clean();
        }
        
        private void OnInspectorUpdate()
        {
            if (focusedWindow == this && mouseOverWindow == this)
            { 
                Repaint();
            }
        }
    }
}
#endif



