using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using BlockBuilder.Core;
using BlockBuilder.Scriptable;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Editor;

#if UNITY_EDITOR
namespace MugCup_BlockBuilder
{
    public class BlockBuilderWindow : EditorWindow
    {
        [MenuItem("Tools/Block Builder/Open Block Builder Window", false, 16)]
        public static void ShowWindow()
        {
            GetWindow(typeof(BlockBuilderWindow), false, "Block Builder").Show();
        }

        private void OnEnable()
        {
            BBEditorManager.Initialize();
            
            AssetManager.LoadAssets();

            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;
            
            BlockPreviewEditor.Init();
        }
        
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnScene;
            
            EditorUtility.SetDirty(BBEditorManager.InterfaceSetting);
            BlockPreviewEditor.Clean();
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
                    BuildPanelGUI.Display();
                    break;
                case 1:
                    break;
                case 2:
                    SettingPanelGUI.Display();
                    break;
                case 3:
                    ToolPanelGUI.Display();
                    break;;
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

                    if(BBEditorManager.InterfaceSetting.CurrentEditMode == InterfaceSetting.EditMode.NONE) return;
                    
                    if (!Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity)) return;
                    
                    GetSelectedFace(_hit);
                    
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    
                    switch (BBEditorManager.InterfaceSetting.CurrentEditMode)
                    {
                        case InterfaceSetting.EditMode.BLOCK_PLACEMENT:
                            
                            UpdateVisualizePointer(_hit, Visualizer.PointerType.Block);
                            BlockPlacementTools.UpdateBlockBuildTools(_currentEvent, _ray);
                            break;
                        
                        case InterfaceSetting.EditMode.EDIT_BLOCKS:
                            
                            UpdateVisualizePointer(_hit, Visualizer.PointerType.Block);
                            BlockEditorTools.UpdateBlockBuildTools(_currentEvent, _ray);
                            break;
                        
                        case InterfaceSetting.EditMode.EDIT_ROADS:
                            
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
            GridGenerator.SelectedFace = BlockFaceUtil.GetSelectedFace(_hit);
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



