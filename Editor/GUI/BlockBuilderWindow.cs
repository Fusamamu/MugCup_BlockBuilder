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
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
namespace MugCup_BlockBuilder.Editor.GUI
{
    public class BlockBuilderWindow : EditorWindow
    {
        // private static BlockBuilderManager blockBuilderManager;
        // private static BlockManager        blockManager;
        // private static BlockEditorManager  blockEditorManager;
        //
        // private static InterfaceSetting   interfaceSetting;
        // private static GridDataSettingSO  gridDataSettingSo;
        //
        private static VolumePoint[] volumePoints;
        //
        
        private static AnimBool displayBuilderMode;
        //
        // private static bool isBlockBuilderManagerInit = false;
        
        [MenuItem("Tools/Block Builder/Open Block Builder Window", false, 16)]
        public static void ShowWindow() => GetWindow(typeof(GUI.BlockBuilderWindow), false, "Block Builder").Show();

        private static GameObject mainMap;

        // private BlockBuilderManager GetBlockBuilderManager()
        // {
        //     if (!blockBuilderManager)
        //         blockBuilderManager = FindObjectOfType<BlockBuilderManager>();
        //
        //     if (!isBlockBuilderManagerInit)
        //     {
        //         blockBuilderManager.Initialized();
        //         isBlockBuilderManagerInit = true;
        //         
        //         Debug.Log($"[Block Builder Manager Initialized]");
        //     }
        //
        //     return blockBuilderManager;
        // }
        //
        // private BlockManager GetBlockManager()
        // {
        //     if (!blockManager)
        //         blockManager = FindObjectOfType<BlockManager>();
        //
        //     return blockManager;
        // }
        //
        // private BlockEditorManager GetBlockEditorManager()
        // {
        //     if (!blockEditorManager)
        //         blockEditorManager = FindObjectOfType<BlockEditorManager>();
        //
        //     return blockEditorManager;
        // }

        private void OnEnable()
        {
            //Use "Block Builder Manager" to initialize first//
            // GetBlockBuilderManager();
            //
            // interfaceSetting  = AssetDatabase.LoadAssetAtPath<InterfaceSetting> ("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/InterfaceSetting.asset");
            // gridDataSettingSo = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/DefaultGridDataSetting.asset" );
            
            BlockBuilderEditorManager.Initialize();
            
            AssetManager.LoadAssets();
            
            SceneView.duringSceneGui += OnScene;

            displayBuilderMode = new AnimBool();
            displayBuilderMode.valueChanged.AddListener(Repaint);
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

                bool _usePrimitive = false;
                
                var _defaultBlock = AssetManager.AssetCollection.DefualtBlock.gameObject;
                if (!_defaultBlock)
                {
                    _defaultBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _usePrimitive = true;
                }
                
                //Need to Find a way to make initialization persistent
                BlockBuilderEditorManager.GetBlockBuilderManager().Initialized();
                
                BlockBuilderEditorManager.GetBlockManager().GenerateGridBlocks();
                
                if(_usePrimitive)
                    DestroyImmediate(_defaultBlock);
              
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
                
                DestroyImmediate(mainMap);

                BlockBuilderEditorManager.GetBlockManager().GetCurrentGridBlockDataManager().ClearGridUnitBlocks();

                var _textParent   = GameObject.Find("[-------Grid Position Text-------]");
                var _blocksParent = GameObject.Find("[-------------Blocks-------------]");
                
                if(_textParent)
                    DestroyImmediate(_textParent);
                if(_blocksParent)
                    DestroyImmediate(_blocksParent);
            }
            
            EditorGUILayout.HelpBox("Select desired edit mode. Use add and remove tab below to start edit blocks", MessageType.Info);
            
            BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode = (InterfaceSetting.EditMode)EditorGUILayout.EnumPopup("Edit mode selection:", BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode);

            EditorGUILayout.LabelField("Edit Blocks");
            string[] _buildingToolTabs = {"Add Block", "Subtract Block"};
            BlockBuilderEditorManager.InterfaceSetting.BuildToolTabSelection = GUILayout.Toolbar(BlockBuilderEditorManager.InterfaceSetting.BuildToolTabSelection, _buildingToolTabs, GUILayout.Height(30));
            
            EditorGUILayout.LabelField("Edit Road Path Blocks");
            string[] _pathBuildingToolTabs = {"Add Road Path", "Remove Road Path"};
            BlockBuilderEditorManager.InterfaceSetting.RoadBuildToolTabSelection = GUILayout.Toolbar(BlockBuilderEditorManager.InterfaceSetting.RoadBuildToolTabSelection, _pathBuildingToolTabs, GUILayout.Height(30));
            
            
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
            
            var _meshData     = (BlockMeshData)EditorGUILayout.ObjectField("Block Mesh Data Setting", null, typeof(BlockMeshData), true);

            var _material     = (Material)EditorGUILayout.ObjectField("Default Block Material", null, typeof(Material), true);
        }

        private void OnScene(SceneView _sceneView)
        {
            if(Application.isPlaying) return;
            
            ProcessMouseEnterLeaveSceneView();
            
            Event _currentEvent = Event.current; 
            
            Ray _ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);

            int _controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (BlockBuilderEditorManager.InterfaceSetting.CurrentMainTapSelection)
            {
                case 0: /*Build Mode*/

                    if(BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode == InterfaceSetting.EditMode.None) return;
                    
                    if (!Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity)) return;
                    
                    GetSelectedFace       (_hit);
                    //UpdateVisualizePointer(_hit);
                    
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    
                    switch (BlockBuilderEditorManager.InterfaceSetting.CurrentEditMode)
                    {
                        case InterfaceSetting.EditMode.EditBlocks:
                            
                            //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                            
                            UpdateVisualizePointer(_hit, Visualizer.PointerType.Block);
                            UpdateBlockBuildTools(_currentEvent, _ray);
                            break;
                        
                        case InterfaceSetting.EditMode.EditRoads:
                            
                            //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                            
                            UpdateVisualizePointer(_hit, Visualizer.PointerType.Path);
                            UpdateRoadBuildTools (_currentEvent, _ray);
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

        private void UpdateBlockBuildTools(Event _currentEvent, Ray _ray)
        {
            switch (BlockBuilderEditorManager.InterfaceSetting.BuildToolTabSelection)
            {
                case 0: /*Add Block*/
                    switch (_currentEvent.type)
                    {
                        case EventType.MouseDown:
                            if (_currentEvent.button == 0)
                            {
                                if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                                {
                                    Vector3 _targetPos = _hit.collider.transform.position;
                            
                                    GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);

                                    var _pos = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);

                                    var _block = _blockPrefab.AddComponent<Block>();
                            
                                    _block.InjectDependency(BlockBuilderEditorManager.GetBlockManager());
                                    _block.Init(_targetPos, _pos);
                                    _block.UpdateBlockData();
                            
                                    BlockBuilderEditorManager.GetBlockEditorManager().InitializeAddTable();
                                    BlockBuilderEditorManager.GetBlockEditorManager().AddBlock(_block, _pos, NormalFace.PosY );
                            
                                    BlockBuilderEditorManager.GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition);
                                    
                            
                                    DestroyImmediate(_blockPrefab);
                                }
                            }
                            break;
                        
                        case EventType.MouseMove:
                            break;
                    }
                    break;
                
                case 1: /*Subtract Block*/
                    if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
                    {
                        Debug.Log($"<color=yellow>[Info]:</color> <color=orange>Left Mouse Button Clicked.</color>");
                        
                        if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                        {
                            var _object = _hit.collider.gameObject;

                            if (_object.TryGetComponent<Block>(out var _block))
                            {
                                BlockBuilderEditorManager.GetBlockManager().RemoveBlock(_block);
                                BlockBuilderEditorManager.GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition);
                            }
                        }
                    }
                    break;
            }
        }
            
        //May Need to Create Utility for Mouse Event
        private bool isPressed = false;
        private bool isDragged = false;
        
        private Vector3Int originPos = Vector3Int.zero;

        private List<GameObject> tempPath = new List<GameObject>();
         
        private void UpdateRoadBuildTools(Event _currentEvent, Ray _ray)
        {
            //EditorEventManager.PollEvents();
            
            switch (BlockBuilderEditorManager.InterfaceSetting.RoadBuildToolTabSelection)
            {
                case 0: /*Add Road Block Path*/

                    switch (_currentEvent.type)
                    {
                        case EventType.MouseDown:
                            
                            if(isPressed) return;

                            //if(EditorEventManager.LeftMouseDown) return;
                            
                            if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
                            {
                                if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                                {
                                    Vector3 _targetPos = _hit.collider.transform.position;
                                    
                                    originPos = CastVec3ToVec3Int(_targetPos);
                                    
                                    
                                    isPressed = true;
                                    
                                    // GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    //
                                    // var _pos = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
                                    //
                                    // var _block = _blockPrefab.AddComponent<PathBlock>();
                                    //
                                    // _block.InjectDependency(GetBlockManager());
                                    // _block.Init(_targetPos, _pos);
                                    // _block.UpdateBlockData();
                                    //
                                    //
                                    // GetBlockEditorManager().InitializeAddTable();
                                    //
                                    // GetBlockEditorManager().RemoveBlock(_pos);
                                    //
                                    // GetBlockEditorManager().AddBlock   (_block, _pos, NormalFace.None);
                                    //
                                    // //GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition);
                                    //
                                    // DestroyImmediate(_blockPrefab);
                                }
                            }

                            break;

                        case EventType.MouseDrag:
                            
                            if(!isPressed) return;
                            
                            Debug.Log("IS Dragging ");

                            isDragged = true;
                            
                            //if(!EditorEventManager.LeftMouseDown) return;

                            if (_currentEvent.button == 0)
                            {
                                Visualizer.ClearPathVisualizer();

                                if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                                {
                                    Vector3Int _targetPos = CastVec3ToVec3Int(_hit.collider.transform.position);

                                    var _path = GetLShapePath(originPos, _targetPos);

                                    if (_path.Count > 0)
                                    {
                                        
                                        Visualizer.CreatePathPointsVisualizer(_path);
                                        
                                        // foreach (var _point in _path)
                                        // {
                                        //     GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                        //
                                        //     _blockPrefab.transform.position = _point;
                                        //
                                        //     //var _pos = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
                                        //
                                        //     var _block = _blockPrefab.AddComponent<Block>();
                                        //
                                        //     _block.InjectDependency(GetBlockManager());
                                        //     _block.Init(_targetPos, _point);
                                        //     _block.UpdateBlockData();
                                        //
                                        //     tempPath.Add(_block.gameObject);
                                        //
                                        //      GetBlockEditorManager().InitializeAddTable();
                                        //      GetBlockEditorManager().AddBlock(_block, _pos, NormalFace.PosY );
                                        //     
                                        //      GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition);
                                        //
                                        //     DestroyImmediate(_blockPrefab);
                                        // }
                                    }

                                }
                            }
                            break;
                        
                        case EventType.MouseUp:

                            isPressed = false;
                            
                            Visualizer.ClearPathVisualizer();
                            
                           // if(isDragged) return;
                            
                            GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            
                            var _block = _blockPrefab.AddComponent<PathBlock>();
                            
                            _block.InjectDependency(BlockBuilderEditorManager.GetBlockManager());
                            _block.Init(originPos, originPos);
                            _block.UpdateBlockData();
                            
                            
                            BlockBuilderEditorManager.GetBlockEditorManager().InitializeAddTable();
                            
                            BlockBuilderEditorManager.GetBlockEditorManager().RemoveBlock(originPos);
                            
                            BlockBuilderEditorManager.GetBlockEditorManager().AddBlock   (_block, originPos, NormalFace.None);
                            
                            
                            BlockBuilderEditorManager.GetBlockManager().UpdateSurroundingBlocksData<PathBlock>(_block.NodePosition);
                            
                            //GetBlockManager().UpdateMeshBlocks<PathBlock>();
                            
                            DestroyImmediate(_blockPrefab);

                            isDragged = false;
                            
                            break;

                    }
                    break;
                
                case 1: /*Remove Road Block Path*/
                  
                    break;
            }
        }

        //Might need to add to Path Finder Util//
        private List<Vector3Int> GetLShapePath(Vector3Int _originPos, Vector3Int _targetPos)
        {
            var _pathResult = new List<Vector3Int>();
            
            if (_targetPos.x > _originPos.x)
            {
                var _dif = _targetPos.x - _originPos.x;
                for (var _i = 0; _i < _dif; _i++)
                {
                    var _pathPos = _originPos;
                    _pathPos += new Vector3Int(_i, 0, 0);
                    _pathResult.Add(_pathPos);
                }
            }
            
            if (_targetPos.x < _originPos.x)
            {
                var _dif = _originPos.x - _targetPos.x;
                for (var _i = 0; _i < _dif; _i++)
                {
                    var _pathPos = _originPos;
                    _pathPos -= new Vector3Int(_i, 0, 0);
                    _pathResult.Add(_pathPos);
                }
            }
            
            if (_targetPos.z > _originPos.z)
            {
                var _dif = _targetPos.z - _originPos.z;
                for (var _j = 0; _j < _dif; _j++)
                {
                    var _pathPos = new Vector3Int(_targetPos.x, _originPos.y, _originPos.z);
   
                    _pathPos += new Vector3Int(0, 0, _j);
   
                    _pathResult.Add(_pathPos);
                }
            }
   
            if (_targetPos.z < _originPos.z)
            {
                var _dif = _originPos.z - _targetPos.z;
 
                for (var _j = 0; _j < _dif; _j++)
                {
                    var _pathPos = new Vector3Int(_targetPos.x, _originPos.y, _originPos.z);
                    
                    _pathPos -= new Vector3Int(0, 0, _j);
   
                    _pathResult.Add(_pathPos);
                }
            }
            
            return _pathResult;
        }

        private Vector3Int CastVec3ToVec3Int(Vector3 _pos)
        {
            return new Vector3Int((int)_pos.x, (int)_pos.y, (int)_pos.z);
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
        }
    }
}
#endif



