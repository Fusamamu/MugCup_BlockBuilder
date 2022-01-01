using System;
using UnityEngine;
using BlockBuilder.Core;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace BlockBuilder.Editor.GUI
{
    public class BlockBuilderWindow : EditorWindow
    {
        private static InterfaceSetting interfaceSetting;
        private static GridDataSettingSO  gridDataSettingSo;

        private static Block[] blocks;
        
        [MenuItem("Tools/Block Builder/Open Block Builder Window", false, 16)]
        public static void ShowWindow() => GetWindow(typeof(GUI.BlockBuilderWindow), false, "Block Builder").Show();

        private void OnEnable()
        {
            interfaceSetting   = AssetDatabase.LoadAssetAtPath<InterfaceSetting>("Assets/Scripts/BlockBuilder/Editor Resources/Setting/InterfaceSetting.asset");
            gridDataSettingSo  = Resources.Load<GridDataSettingSO>("BlockBuilder/Setting/GridDataSetting");
            SceneView.duringSceneGui += OnScene;

            blocks = new Block[0];
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(15, 15, EditorGUIUtility.currentViewWidth - 30, 500));
            
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
                    break;
                case 3:
                    break;;
            }
            
            GUILayout.EndArea();
        }

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

                    int _newRow = gridDataSettingSo.MapSize.x;
                    int _newColumn = gridDataSettingSo.MapSize.z;
                    int _newHeight = gridDataSettingSo.MapSize.y;
                    
                    float originalValue = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 40;
                    _newRow    = EditorGUILayout.IntField("Row", _newRow);
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
            newStylee.margin = new RectOffset(10, 10, 10, 10);
            
            Undo.RecordObject(gridDataSettingSo,"Undo");
            if (GUILayout.Button("Generate Map", newStylee, GUILayout.Height(30)))
            {
                Vector3Int _mapSize  = gridDataSettingSo.MapSize;
                Vector3Int _unitSize = gridDataSettingSo.GridUnitSize;
                GridBlockGenerator.GenerateMap(_mapSize, _unitSize);
            }

            if (GUILayout.Button("Generate Grid", newStylee, GUILayout.Height(30)))
            {
                Vector3Int _mapSize  = gridDataSettingSo.MapSize;
                Vector3Int _unitSize = gridDataSettingSo.GridUnitSize;
                GameObject _mainMap = new GameObject("Main Map");
                
                GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                
                blocks = GridBlockGenerator.GenerateGridBlocks(_unitSize, _blockPrefab, _mainMap);
                DestroyImmediate(_blockPrefab);
              
            }
            
            if(GUILayout.Button("Delete Blocks", newStylee, GUILayout.Height(30)))
            {
                var _blocks = GameObject.FindGameObjectsWithTag("Block");
                foreach(GameObject _block in _blocks)
                    DestroyImmediate(_block);
            }

            string[] _buildingToolTabs = {"Add Block", "Subtract Block"};
            interfaceSetting.BuildToolTabSelection = GUILayout.Toolbar(interfaceSetting.BuildToolTabSelection, _buildingToolTabs, GUILayout.Height(30));
        }

        private void OnScene(SceneView _sceneView)
        {
            if(Application.isPlaying) return;
            
            ProcessMouseEnterLeaveSceneview();
            
            Event _currentEvent = Event.current; 
            Ray _ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

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



