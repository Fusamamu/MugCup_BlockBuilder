#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Editor;
using MugCup_PathFinder.Runtime;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class BlockEditorGUI
    {
        private static bool blockEditorFoldout;

        private static Vector2 scrollPosition;
        
        public static void Display()
        {
            blockEditorFoldout = BBEditorStyling.DrawHeader(Color.clear, "Blocks Editor", blockEditorFoldout);

            if (blockEditorFoldout)
            {
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
                
                GUI.backgroundColor = Color.white;
                
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
                
                EditorGUILayout.LabelField("Edit Block Type");
                string[] _blockTypes = { "Grass", "Water", "Dirt", "Rock" };
                BBEditorManager.InterfaceSetting.BlockTypeTabSelection 
                    = GUILayout.Toolbar(BBEditorManager.InterfaceSetting.BlockTypeTabSelection, _blockTypes, GUILayout.Height(30));
                
                string[] _blockTypeTabs = { "Change block type", "Remove block type" };
                BBEditorManager.InterfaceSetting.EditBlockTypeToolTabSelection 
                    = GUILayout.Toolbar(BBEditorManager.InterfaceSetting.EditBlockTypeToolTabSelection, _blockTypeTabs, GUILayout.Height(30));
                
                EditorGUILayout.LabelField("Edit Road Path Blocks");
                string[] _pathBuildingToolTabs = { "Add Road Path", "Remove Road Path" };
                BBEditorManager.InterfaceSetting.RoadBuildToolTabSelection 
                    = GUILayout.Toolbar(BBEditorManager.InterfaceSetting.RoadBuildToolTabSelection, _pathBuildingToolTabs, GUILayout.Height(30));
                
                EditorGUILayout.EndVertical();
                
            }
        }
    }
}
#endif
