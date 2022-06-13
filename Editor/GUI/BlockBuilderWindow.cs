using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Runtime.Scriptable;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using UnityEditor.AnimatedValues;
#endif
using BlockBuilder.Core;
using BlockBuilder.Scriptable;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core;

#if UNITY_EDITOR
namespace MugCup_BlockBuilder.Editor.GUI
{
    public class BlockBuilderWindow : EditorWindow
    {
        private static BlockManager blockManager;
        
        private static InterfaceSetting   interfaceSetting;
        private static GridDataSettingSO  gridDataSettingSo;

        //private static Block[]       blocks;
        private static VolumePoint[] volumePoints;

        private static AnimBool displayBuilderMode;
        
        [MenuItem("Tools/Block Builder/Open Block Builder Window", false, 16)]
        public static void ShowWindow() => GetWindow(typeof(GUI.BlockBuilderWindow), false, "Block Builder").Show();

        private BlockManager GetBlockManager()
        {
            if (!blockManager)
                blockManager = FindObjectOfType<BlockManager>();

            return blockManager;
        }

        private void OnEnable()
        {
            blockManager = FindObjectOfType<BlockManager>();
            
            interfaceSetting  = AssetDatabase.LoadAssetAtPath<InterfaceSetting> ("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/InterfaceSetting.asset");
            gridDataSettingSo = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/DefaultGridDataSetting.asset" );
            
            AssetManager.LoadAssets();
            
            SceneView.duringSceneGui += OnScene;

            //blocks = Array.Empty<Block>();

            displayBuilderMode = new AnimBool();
            displayBuilderMode.valueChanged.AddListener(Repaint);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(15, 15, EditorGUIUtility.currentViewWidth - 30, 800));
            
            GUILayout.Label("BlockBuilder", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            string[] _tabCaptions = {"Build", "Paint", "Setting", "Tools"};
            
            interfaceSetting.CurrentMainTapSelection = GUILayout.Toolbar(interfaceSetting.CurrentMainTapSelection, _tabCaptions, GUILayout.Height(50), GUILayout.ExpandWidth(true));
            GUILayout.Space(10);
           
            switch (interfaceSetting.CurrentMainTapSelection)
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
            interfaceSetting.MapSettingFoldout = EditorGUILayout.Foldout(interfaceSetting.MapSettingFoldout, "Map Data Setting");
            
            if (interfaceSetting.MapSettingFoldout)
            {
                GUILayout.BeginVertical("HelpBox");
                
                    GUILayout.Label("Map Data Setting");
                    
                    GUILayout.BeginVertical("GroupBox");
                    
                    gridDataSettingSo.MapSize      = EditorGUILayout.Vector3IntField("Map Size",      gridDataSettingSo.MapSize);
                    gridDataSettingSo.GridUnitSize = EditorGUILayout.Vector3IntField("Map Unit Size", gridDataSettingSo.GridUnitSize);
                    
                    GUILayout.BeginHorizontal();

                    int _newRow    = gridDataSettingSo.MapSize.x;
                    int _newColumn = gridDataSettingSo.MapSize.z;
                    int _newHeight = gridDataSettingSo.MapSize.y;

                    gridDataSettingSo.MapSize = new Vector3Int(_newRow, _newHeight, _newColumn);
                    
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                
                GUILayout.EndVertical();
            }

            var _newStyle = new GUIStyle(UnityEngine.GUI.skin.button);

            Undo.RecordObject(gridDataSettingSo,"Undo");
            if (GUILayout.Button("Generate Map", _newStyle, GUILayout.Height(30)))
            {
                Vector3Int _mapSize  = gridDataSettingSo.MapSize;
                Vector3Int _unitSize = gridDataSettingSo.GridUnitSize;
                GridBlockGenerator.GenerateMap(_mapSize, _unitSize);
            }

            if (GUILayout.Button("Generate Grid", _newStyle, GUILayout.Height(30)))
            {
                Vector3Int _mapSize  = gridDataSettingSo.MapSize;
                Vector3Int _unitSize = gridDataSettingSo.GridUnitSize;
                
                GameObject _mainMap = new GameObject("Main Map");

                bool _usePrimitive = false;
                
                var _defaultBlock = AssetManager.AssetCollection.DefualtBlock.gameObject;
                if (!_defaultBlock)
                {
                    _defaultBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _usePrimitive = true;
                }
                
                GetBlockManager().Initialized();
                GetBlockManager().GenerateGridBlocks(_unitSize, _defaultBlock, _mainMap);
                
                if(_usePrimitive)
                    DestroyImmediate(_defaultBlock);
            }
            
            if (GUILayout.Button("Initialize Blocks Data", _newStyle, GUILayout.Height(30)))
            {
                FindObjectOfType<GridBlockDataManager>().InitializeBlocksData();
            }
            
            

            if (GUILayout.Button("Generate Volume Points", _newStyle, GUILayout.Height(30)))
            {
                Vector3Int _gridUnitSize  = gridDataSettingSo.GridUnitSize;
                GameObject _volumePoints  = new GameObject("[Volume Points]");
                
                volumePoints = VolumePointGenerator.GeneratedVolumePoints(_gridUnitSize, 0.1f, _volumePoints);

                var _blocks = blockManager.GetCurrentGridBlockDataManager().GetAvailableBlocks().ToArray();

                if (_blocks != null && _blocks.Length > 0)
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
                foreach(GameObject _block in _blocks)
                    DestroyImmediate(_block);
            }

            string[] _buildingToolTabs = {"Add Block", "Subtract Block"};
            interfaceSetting.BuildToolTabSelection = GUILayout.Toolbar(interfaceSetting.BuildToolTabSelection, _buildingToolTabs, GUILayout.Height(30));
            
            
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
            gridDataSettingSo = (GridDataSettingSO)EditorGUILayout.ObjectField("Grid Data Setting", gridDataSettingSo, typeof(GridDataSettingSO), true);
            var _meshData     = (BlockMeshData)EditorGUILayout.ObjectField("Block Mesh Data Setting", null, typeof(BlockMeshData), true);

            var _material     = (Material)EditorGUILayout.ObjectField("Default Block Material", null, typeof(Material), true);
        }

        private void OnScene(SceneView _sceneView)
        {
            if(Application.isPlaying) return;
            
            ProcessMouseEnterLeaveSceneview();
            
            Event _currentEvent = Event.current; 
            Ray _ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);

            int _controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (interfaceSetting.CurrentMainTapSelection)
            {
                case 0: /*Build Mode*/

                    if (!Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity)) return;
                    
                    GetSelectedFace(_hit);
                    UpdateVisualizePointer(_hit);
                    UpdateBuildTools(_currentEvent, _ray);
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
        private static void UpdateVisualizePointer(RaycastHit _hit)
        {
            Vector3    _centerPos  = _hit.collider.gameObject.transform.position;

            Visualizer.GetPointerReference().transform.position = _centerPos;

            Vector3 _faceCenterPos   = _centerPos     + _hit.normal / 2; //Should be propotion to Cube Size;
            Vector3 _extendedLinePos = _faceCenterPos + _hit.normal * 2f;
            
            Visualizer.DrawLine(_faceCenterPos, _extendedLinePos, Color.yellow, 2f);
            Handles.DrawWireDisc(_faceCenterPos, _hit.normal, 0.3f, 2f);
        }

        private static void GetSelectedFace(RaycastHit _hit)
        {
            GridBlockGenerator.SelectedFace = BlockFaceUtil.GetSelectedFace(_hit);
        }

        private static void UpdateBuildTools(Event _currentEvent, Ray _ray)
        {
            switch (interfaceSetting.BuildToolTabSelection)
            {
                case 0: /*Add Block*/
                    if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
                    {
                        if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                        {
                            Vector3 _targetPos = new Vector3();
                            
                            switch (GridBlockGenerator.SelectedFace)
                            {
                                case NormalFace.PosY:
                                    _targetPos = _hit.collider.transform.position + Vector3.up;
                                    break;
                            }
                            
                            GameObject _block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            GridBlockGenerator.AddBlock(_targetPos, _hit.collider.gameObject.transform.parent, _block);
                            DestroyImmediate(_block);
                        }
                    }
                    break;
                case 1: /*Subtract Block*/
                    if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
                    {
                        Debug.Log($"<color=yellow>[Info]:</color> <color=orange>Left Mouse Button Clicked.</color>");
                        if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                        {
                            GameObject _object = _hit.collider.gameObject;
                            DestroyImmediate(_object);
                        }
                    }
                    break;
            }
        }
        
        void ProcessMouseEnterLeaveSceneview()
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
            EditorUtility.SetDirty(interfaceSetting);
        }
    }
}
#endif



