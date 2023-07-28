#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Editor;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class GridElementGeneratorGUI
    {
        private static bool showGizmos;
        private static bool showTilePivot;
        private static bool showCornerPivot;
        private static bool showBoundBox;
        
        private static int gridLevel;
        
        private static bool gridElementGeneratorFoldout;
         
        public static void Display()
        {
            gridElementGeneratorFoldout = BBEditorStyling.DrawHeader(Color.clear, "Grid Elements Generator", gridElementGeneratorFoldout);

            if (gridElementGeneratorFoldout)
            {
                var _generateButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 100,
                };
                
                var _clearButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 100,
                };
                
                EditorGUILayout.BeginVertical(new GUIStyle {padding = new RectOffset(20, 20, 8, 3)});
                {
                    EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
                      
                    showGizmos            = EditorGUILayout.Toggle("Display gizmos"   , showGizmos);
                    showTilePivot         = EditorGUILayout.Toggle("Show Tile Pivot", showTilePivot);
                    showCornerPivot       = EditorGUILayout.Toggle("Show Corner Pivot", showCornerPivot);
                    showBoundBox          = EditorGUILayout.Toggle("Show Bound Box"   , showBoundBox);

                    var _allGridElements = BBEditorManager.GridElementDataManager.GridElementData.GridNodes;
                    if (_allGridElements is { Length: > 0 })
                    {
                        foreach (var _element in _allGridElements)
                        {
                            if(_element == null) continue;
                            _element.SetShowGizmos(showGizmos);
                            _element.SetShowPivot (showTilePivot);
                        }
                    }

                    var _allVolumePoints = BBEditorManager.GridElementDataManager.VolumePointData.GridNodes;
                    if (_allVolumePoints is { Length: > 0 })
                    {
                        foreach (var _element in _allVolumePoints)
                        {
                            if(_element == null) continue;
                            
                            if (_element.TryGetComponent<ModulePrototype>(out var _prototype))
                            {
                                _prototype.SetShowGizmos  (showGizmos);
                                _prototype.SetShowPivot   (showCornerPivot);
                                _prototype.SetShowBoundBox(showBoundBox);
                            }
                        }
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        gridLevel = EditorGUILayout.IntField("Target Level", gridLevel);
                        
                        if (GUILayout.Button("Enable", _clearButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            var _gridElements = _gridElementDataManager.GridElementData.GetAllNodeBasesAtLevel(gridLevel);

                            foreach (var _element in _gridElements)
                            {
                                if(_element == null) continue;
                                _element.Enable();
                            }
                            
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                        
                        if (GUILayout.Button("Disable", _clearButtonStyle))
                        {
                           
                        }
                        
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                
                
                EditorGUILayout.BeginVertical("GroupBox");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Grid Element");
                                
                        if (GUILayout.Button("Generate", _generateButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.Initialized();
                            _gridElementDataManager.GenerateGrid();
                                    
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                                
                        if (GUILayout.Button("Clear", _clearButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.ClearGrid();
                    
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Volume Points");
                                
                        if (GUILayout.Button("Generate", _generateButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.GenerateVolumePoints();
                                    
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                        
                        if (GUILayout.Button("Clear", _generateButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.ClearVolumePoints();
                    
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    
                    //Will Modify to use Terrain Texture
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Use Terrain Texture");
                                
                        if (GUILayout.Button("Generate", _generateButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.Initialized();
                            _gridElementDataManager.GenerateGrid();
                            _gridElementDataManager.GenerateVolumePoints();
                            
                            var _allTargetPos = new List<Vector3Int>();
                            
                            for (var _i = 0; _i < BBEditorManager.MapTextureDataSettingSo.TerrainLevel - 1; _i++)
                            {
                                var _targetGridPos = TextureGenerator.GetSolidGridPosFromTexture(
                                    BBEditorManager.MapTextureDataSettingSo.GeneratedTexture, _i,
                                    BBEditorManager.MapTextureDataSettingSo.TerrainLevel);

                                _allTargetPos.AddRange(_targetGridPos);
                            }

                            foreach (var _element in _gridElementDataManager.GridElementData.GridNodes)
                            {
                                if (_element == null) continue;
                                
                                if(_allTargetPos.Contains(_element.NodeGridPosition))
                                    _element.Enable();
                            }

                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                        
                        if (GUILayout.Button("Clear", _generateButtonStyle))
                        {
                            var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                            Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                            _gridElementDataManager.ClearGrid();
                            _gridElementDataManager.ClearVolumePoints();
                    
                            PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    // EditorGUILayout.BeginHorizontal();
                    // {
                    //     EditorGUILayout.LabelField("Module Slots");
                    //             
                    //     if (GUILayout.Button("Update", _generateButtonStyle))
                    //     {
                    //         var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                    //         Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");
                    //
                    //         _gridElementDataManager.UpdateModuleSlotData();
                    //                 
                    //         PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                    //     }
                    //     
                    //     if (GUILayout.Button("Clear", _generateButtonStyle))
                    //     {
                    //         // var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                    //         // Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");
                    //         //
                    //         // _gridElementDataManager.ClearVolumePoints();
                    //         //
                    //         // PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                    //     }
                    // }
                    // EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(15);
                    if(GUILayout.Button("Clear All", EditorStyles.miniButton, GUILayout.Width(80)))
                    {
                        var _gridElementDataManager = BBEditorManager.GridElementDataManager;
                        Undo.RecordObject(_gridElementDataManager, "GridElementDataManager Changed");

                        _gridElementDataManager.ClearGrid();
                        _gridElementDataManager.ClearVolumePoints();
                        
                        PrefabUtility.RecordPrefabInstancePropertyModifications(_gridElementDataManager);
                    }  
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
    }
}
#endif
