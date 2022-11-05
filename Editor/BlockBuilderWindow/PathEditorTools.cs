using System.Collections.Generic;
using UnityEngine;

using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;
using UnityEditor;

namespace MugCup_BlockBuilder.Editor
{
    public static class PathEditorTools
    {
        //May Need to Create Utility for Mouse Event
        private static bool isPressed = false;
        private static bool isDragged = false;
        
        private static Vector3Int originPos = Vector3Int.zero;

        private static List<Vector3Int> tempPathPositions;
        private static List<Path> modifiedPaths = new List<Path>();
         
        public static void UpdateRoadBuildTools(Event _currentEvent, Ray _ray)
        {
            switch (BBEditorManager.InterfaceSetting.RoadBuildToolTabSelection)
            {
                case 0: /*Add Road Block Path*/
                    UpdateAddPathProcess(_currentEvent, _ray);
                    break;
                case 1: /*Remove Road Block Path*/
                    break;
            }
        }

        private static void UpdateAddPathProcess(Event _currentEvent, Ray _ray)
        {
             switch (_currentEvent.type)
             {
                 case EventType.MouseDown:
                            
                     if(isPressed) return;
                            
                     if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
                     {
                         if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                         {
                             Vector3 _targetPos = _hit.collider.transform.position;
                                    
                             originPos = Utilities.CastVec3ToVec3Int(_targetPos);
                                    
                             isPressed = true;
                         }
                     }
                     break;
                        
                 case EventType.MouseDrag:
                            
                     if(!isPressed) return;
                            
                     isDragged = true;

                     if (_currentEvent.button == 0)
                     {
                         Visualizer.ClearPathVisualizer();

                         if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                         {
                             Vector3Int _targetPos = Utilities.CastVec3ToVec3Int(_hit.collider.transform.position);

                             tempPathPositions = GetLShapePath(originPos, _targetPos);

                             if (tempPathPositions.Count > 0)
                             {
                                 Visualizer.CreatePathPointsVisualizer(tempPathPositions);
                             }
                         }
                     }
                     break;
                        
                 case EventType.MouseUp:

                     isPressed = false;
                     isDragged = false;
                     Visualizer.ClearPathVisualizer();
                     
                     BBEditorUtility.RecordGridBlockManagerChanges(() =>
                     {
                         modifiedPaths = new List<Path>();
                         
                         if (tempPathPositions.Count > 1)
                         {
                             foreach (var _pos in tempPathPositions)
                             {
                                 AddPath(_pos);
                             }
                             
                             tempPathPositions.Clear();
                         }
                         else
                         {
                             AddPath(originPos);
                         }
                         
                     });
                       
                     var _gridBlockDataManager = BBEditorManager.BlockDataManager;
                     Undo.RecordObject(_gridBlockDataManager, "GridBlockDataManager Changed");

                     foreach (var _path in BBEditorManager.BlockDataManager.AvailableBlocks<Path>())
                     {
                         _path.GetSurroundingBlocksReference();
                         _path.SetBitMask();
                     }
                     
                     foreach (var _path in BBEditorManager.BlockDataManager.AvailableBlocks<Path>())
                     {
                         BBEditorManager.BlockManager.UpdateMeshBlockComposite(_path);
                     }
                  
                     PrefabUtility.RecordPrefabInstancePropertyModifications(_gridBlockDataManager);
                     break;
             }
        }

        private static void AddPath(Vector3Int _pos)
        {
            var _tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var _path     = _tempCube.AddComponent<Path>();
                            
            _path.InjectDependency(BBEditorManager.BlockManager);
            _path.Init(_pos, _pos);
            _path.UpdateBlockData();
                     
            BBEditorManager.BlockEditorManager.InitializeAddTable();
            BBEditorManager.BlockEditorManager.RemoveBlock(_pos);
            BBEditorManager.BlockEditorManager.AddBlock   (_path, _pos, NormalFace.None);
                            
            BBEditorManager.BlockManager.UpdateSurroundingBlocksData<Path>(_path.NodePosition);
                            
            Object.DestroyImmediate(_tempCube);
        }
        
        private static List<Vector3Int> GetLShapePath(Vector3Int _originPos, Vector3Int _targetPos)
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
      
    }
}
