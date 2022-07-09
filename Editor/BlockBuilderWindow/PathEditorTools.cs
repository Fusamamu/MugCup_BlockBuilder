using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;

namespace MugCup_BlockBuilder.Editor
{
    public static class PathEditorTools
    {
        //May Need to Create Utility for Mouse Event
        private static bool isPressed = false;
        private static bool isDragged = false;
        
        private static Vector3Int originPos = Vector3Int.zero;

        private static List<GameObject> tempPath = new List<GameObject>();
         
        public static void UpdateRoadBuildTools(Event _currentEvent, Ray _ray)
        {
            switch (BlockBuilderEditorManager.InterfaceSetting.RoadBuildToolTabSelection)
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

                             var _path = GetLShapePath(originPos, _targetPos);

                             if (_path.Count > 0)
                             {
                                 Visualizer.CreatePathPointsVisualizer(_path);
                             }
                         }
                     }
                     break;
                        
                 case EventType.MouseUp:

                     isPressed = false;
                            
                     Visualizer.ClearPathVisualizer();
                     
                     GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            
                     var _block = _blockPrefab.AddComponent<PathBlock>();
                            
                     _block.InjectDependency(BlockBuilderEditorManager.GetBlockManager());
                     _block.Init(originPos, originPos);
                     _block.UpdateBlockData();
                            
                     BlockBuilderEditorManager.GetBlockEditorManager().InitializeAddTable();
                            
                     BlockBuilderEditorManager.GetBlockEditorManager().RemoveBlock(originPos);
                            
                     BlockBuilderEditorManager.GetBlockEditorManager().AddBlock   (_block, originPos, NormalFace.None);
                            
                     BlockBuilderEditorManager.GetBlockManager().UpdateSurroundingBlocksData<PathBlock>(_block.NodePosition);
                            
                     Object.DestroyImmediate(_blockPrefab);

                     isDragged = false;
                            
                     break;
             }
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
