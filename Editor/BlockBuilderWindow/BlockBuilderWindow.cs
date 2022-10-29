using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using BlockBuilder.Core.Scriptable;
using UnityEditor.AnimatedValues;
#endif
using BlockBuilder.Core;
using BlockBuilder.Scriptable;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;

#if UNITY_EDITOR
namespace MugCup_BlockBuilder.Editor
{
    public class BlockBuilderWindow : EditorWindow
    {
        private static VolumePoint[] volumePoints;
        
        private static AnimBool displayBuilderMode;
        
        [MenuItem("Tools/Block Builder/Open Block Builder Window", false, 16)]
        public static void ShowWindow() => GetWindow(typeof(BlockBuilderWindow), false, "Block Builder").Show();

        private void OnEnable()
        {
            BlockBuilderEditorManager.Initialize();
            
            AssetManager.LoadAssets();
            
            SceneView.duringSceneGui += OnScene;

            displayBuilderMode = new AnimBool();
            displayBuilderMode.valueChanged.AddListener(Repaint);
            
            BlockPreviewEditor.Init();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(15, 15, EditorGUIUtility.currentViewWidth - 30, 800));
            
            GUILayout.Label("BlockBuilder", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            string[] _tabCaptions = {"Build", "Paint", "Setting", "Tools"};
            
            BlockBuilderEditorManager.InterfaceSetting.CurrentMainTapSelection 
                = GUILayout.Toolbar(BlockBuilderEditorManager.InterfaceSetting.CurrentMainTapSelection, _tabCaptions, GUILayout.Height(50), GUILayout.ExpandWidth(true));
            
            GUILayout.Space(10);
           
            switch (BlockBuilderEditorManager.InterfaceSetting.CurrentMainTapSelection)
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
                    break;;
            }
            
            GUILayout.EndArea();
        }

        private void DisplayBuildPanel()
        {
            BlockBuilderEditorManager.InterfaceSetting.MapSettingFoldout 
                = EditorGUILayout.Foldout(BlockBuilderEditorManager.InterfaceSetting.MapSettingFoldout, "Map Data Setting");
            
            if (BlockBuilderEditorManager.InterfaceSetting.MapSettingFoldout)
            {
                GUILayout.BeginVertical("HelpBox");
                
                    GUILayout.Label("Map Data Setting");
                    
                    GUILayout.BeginVertical("GroupBox");
                    
                    BlockBuilderEditorManager.GridDataSettingSo.MapSize      = EditorGUILayout.Vector3IntField("Map Size",      BlockBuilderEditorManager.GridDataSettingSo.MapSize);
                    BlockBuilderEditorManager.GridDataSettingSo.GridUnitSize = EditorGUILayout.Vector3IntField("Map Unit Size", BlockBuilderEditorManager.GridDataSettingSo.GridUnitSize);
                    
                    GUILayout.BeginHorizontal();

                    int _newRow    = BlockBuilderEditorManager.GridDataSettingSo.MapSize.x;
                    int _newColumn = BlockBuilderEditorManager.GridDataSettingSo.MapSize.z;
                    int _newHeight = BlockBuilderEditorManager.GridDataSettingSo.MapSize.y;

                    BlockBuilderEditorManager.GridDataSettingSo.MapSize = new Vector3Int(_newRow, _newHeight, _newColumn);
                    
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                
                GUILayout.EndVertical();
            }

            var _newStyle = new GUIStyle(UnityEngine.GUI.skin.button);

            Undo.RecordObject(BlockBuilderEditorManager.GridDataSettingSo,"Undo");
            
            if (GUILayout.Button("Generate Map", _newStyle, GUILayout.Height(30)))
            {
                Vector3Int _mapSize  = BlockBuilderEditorManager.GridDataSettingSo.MapSize;
                Vector3Int _unitSize = BlockBuilderEditorManager.GridDataSettingSo.GridUnitSize;
                GridBlockGenerator.GenerateMap(_mapSize, _unitSize);
            }

            if (GUILayout.Button("Generate Grid", _newStyle, GUILayout.Height(30)))
            {
                var _gridBlockDataManager = BlockBuilderEditorManager.GetBlockManager().GetCurrentGridBlockDataManager();
                Undo.RecordObject(_gridBlockDataManager, "GridBlockDataManager Changed");
                
                Vector3Int _mapSize  = BlockBuilderEditorManager.GridDataSettingSo.MapSize;
                Vector3Int _unitSize = BlockBuilderEditorManager.GridDataSettingSo.GridUnitSize;

                //Need to Find a way to make initialization persistent
                BlockBuilderEditorManager.GetBlockBuilderManager().Initialized();
                BlockBuilderEditorManager.GetBlockManager().GenerateGridBlocks();
              
                PrefabUtility.RecordPrefabInstancePropertyModifications(_gridBlockDataManager);
            }

            if (GUILayout.Button("Generate Volume Points", _newStyle, GUILayout.Height(30)))
            {
                Vector3Int _gridUnitSize  = BlockBuilderEditorManager.GridDataSettingSo.GridUnitSize;
                GameObject _volumePoints  = new GameObject("[Volume Points]");
                
                volumePoints = VolumePointGenerator.GeneratedVolumePoints(_gridUnitSize, 0.1f, _volumePoints);

                var _blocks = BlockBuilderEditorManager.GetBlockManager().GetCurrentGridBlockDataManager().GetAvailableBlocks().ToArray();

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
            
            if(GUILayout.Button("Delete Blocks", _newStyle, GUILayout.Height(30)))
            {
                var _blocks = GameObject.FindGameObjectsWithTag("Block");
                
                foreach(var _block in _blocks)
                    DestroyImmediate(_block);

                BlockBuilderEditorManager.GetBlockManager().GetCurrentGridBlockDataManager().ClearGridUnitNodeBases();

                var _textParent   = GameObject.Find("[-------Grid Position Text-------]");
                var _blocksParent = GameObject.Find("[-------------Blocks-------------]");
                
                if(_textParent)
                    DestroyImmediate(_textParent);
                if(_blocksParent)
                    DestroyImmediate(_blocksParent);
            }
            
            EditorGUILayout.HelpBox("Select desired edit mode. Use add and remove tab below to start edit blocks", MessageType.Info);
            
            BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode = (InterfaceSetting.EditMode)EditorGUILayout.EnumPopup("Edit mode selection:", BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode);

            
            var _previewWidth = 100f;
            
            var _lastRect         = GUILayoutUtility.GetLastRect();
            var _singleLineHeight = EditorGUIUtility.singleLineHeight;
            
            
            var _targetRect = new Rect(_lastRect.x, _lastRect.y + _singleLineHeight + 5, _previewWidth, _previewWidth);
            
            var _cube = Resources.Load<GameObject>("Prefabs/Towers/Tower_Castle");
            
            if(_cube == null)
                Debug.LogWarning("Missing Cube");

            
            var _tex = BlockPreviewEditor.CreatePreviewTexture(new Rect(0, 0, _previewWidth, _previewWidth), _cube);
            
            EditorGUI.DrawPreviewTexture(_targetRect, _tex);
            
            var _tex1 = BlockPreviewEditor.CreatePreviewTexture(new Rect(0, 0, _previewWidth, _previewWidth), _cube);

            var _nextRect = new Rect(_targetRect.x + _previewWidth + 5, _targetRect.y, _previewWidth, _previewWidth);
            
            EditorGUI.DrawPreviewTexture(_nextRect, _tex1);
            
            EditorGUILayout.Space(_previewWidth);
            
            
            
            
            
            
            EditorGUILayout.LabelField("Block Element Placement");
            string[] _blockPlacementTools = { "Place Block Element", "Remove Block Element" };
            BlockBuilderEditorManager.InterfaceSetting.BlockPlacementToolTabSelection 
                = GUILayout.Toolbar(BlockBuilderEditorManager.InterfaceSetting.BlockPlacementToolTabSelection, _blockPlacementTools, GUILayout.Height(30));

            EditorGUILayout.LabelField("Edit Blocks");
            string[] _buildingToolTabs = { "Add Block", "Subtract Block" };
            BlockBuilderEditorManager.InterfaceSetting.BuildToolTabSelection 
                = GUILayout.Toolbar(BlockBuilderEditorManager.InterfaceSetting.BuildToolTabSelection, _buildingToolTabs, GUILayout.Height(30));
            
            EditorGUILayout.LabelField("Edit Road Path Blocks");
            string[] _pathBuildingToolTabs = { "Add Road Path", "Remove Road Path" };
            BlockBuilderEditorManager.InterfaceSetting.RoadBuildToolTabSelection 
                = GUILayout.Toolbar(BlockBuilderEditorManager.InterfaceSetting.RoadBuildToolTabSelection, _pathBuildingToolTabs, GUILayout.Height(30));
            
            DisplayBuilderModeSelectionInApplication();
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
            BlockBuilderEditorManager.GridDataSettingSo = (GridDataSettingSO)EditorGUILayout.ObjectField("Grid Data Setting", BlockBuilderEditorManager.GridDataSettingSo, typeof(GridDataSettingSO), true);
            
            var _meshData = (BlockMeshData)EditorGUILayout.ObjectField("Block Mesh Data Setting", null, typeof(BlockMeshData), true);
            var _material = (Material)     EditorGUILayout.ObjectField("Default Block Material" , null, typeof(Material)     , true);
        }

        private void OnScene(SceneView _sceneView)
        {
            if(Application.isPlaying) return;
            
            ProcessMouseEnterLeaveSceneView();
            
            Event _currentEvent = Event.current; 
            Ray _ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);

            switch (BlockBuilderEditorManager.InterfaceSetting.CurrentMainTapSelection)
            {
                case 0: /*Build Mode*/

                    if(BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode == InterfaceSetting.EditMode.None) return;
                    
                    if (!Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity)) return;
                    
                    GetSelectedFace (_hit);
                    
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    
                    switch (BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode)
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
        
        void ProcessMouseEnterLeaveSceneView()
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
            EditorUtility.SetDirty(BlockBuilderEditorManager.InterfaceSetting);
            
            BlockPreviewEditor.Clean();
        }
    }
}
#endif



