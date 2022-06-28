// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace MugCup_BlockBuilder.Editor
// {
//     public static class PathEditorTools
//     {
//         
//          //May Need to Create Utility for Mouse Event
//         private bool isPressed = false;
//         private bool isDragged = false;
//         
//         private Vector3Int originPos = Vector3Int.zero;
//
//         private List<GameObject> tempPath = new List<GameObject>();
//          
//         private void UpdateRoadBuildTools(Event _currentEvent, Ray _ray)
//         {
//             //EditorEventManager.PollEvents();
//             
//             switch (interfaceSetting.RoadBuildToolTabSelection)
//             {
//                 case 0: /*Add Road Block Path*/
//
//                     switch (_currentEvent.type)
//                     {
//                         case EventType.MouseDown:
//                             
//                             if(isPressed) return;
//
//                             //if(EditorEventManager.LeftMouseDown) return;
//                             
//                             if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
//                             {
//                                 if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
//                                 {
//                                     Vector3 _targetPos = _hit.collider.transform.position;
//                                     
//                                     originPos = CastVec3ToVec3Int(_targetPos);
//                                     
//                                     
//                                     isPressed = true;
//                                     
//                                     // GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                                     //
//                                     // var _pos = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
//                                     //
//                                     // var _block = _blockPrefab.AddComponent<PathBlock>();
//                                     //
//                                     // _block.InjectDependency(GetBlockManager());
//                                     // _block.Init(_targetPos, _pos);
//                                     // _block.UpdateBlockData();
//                                     //
//                                     //
//                                     // GetBlockEditorManager().InitializeAddTable();
//                                     //
//                                     // GetBlockEditorManager().RemoveBlock(_pos);
//                                     //
//                                     // GetBlockEditorManager().AddBlock   (_block, _pos, NormalFace.None);
//                                     //
//                                     // //GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition);
//                                     //
//                                     // DestroyImmediate(_blockPrefab);
//                                 }
//                             }
//
//                             break;
//
//                         case EventType.MouseDrag:
//                             
//                             if(!isPressed) return;
//                             
//                             Debug.Log("IS Dragging ");
//
//                             isDragged = true;
//                             
//                             //if(!EditorEventManager.LeftMouseDown) return;
//
//                             if (_currentEvent.button == 0)
//                             {
//                                 Visualizer.ClearPathVisualizer();
//
//                                 if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
//                                 {
//                                     Vector3Int _targetPos = CastVec3ToVec3Int(_hit.collider.transform.position);
//
//                                     var _path = GetLShapePath(originPos, _targetPos);
//
//                                     if (_path.Count > 0)
//                                     {
//                                         
//                                         Visualizer.CreatePathPointsVisualizer(_path);
//                                         
//                                         // foreach (var _point in _path)
//                                         // {
//                                         //     GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                                         //
//                                         //     _blockPrefab.transform.position = _point;
//                                         //
//                                         //     //var _pos = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
//                                         //
//                                         //     var _block = _blockPrefab.AddComponent<Block>();
//                                         //
//                                         //     _block.InjectDependency(GetBlockManager());
//                                         //     _block.Init(_targetPos, _point);
//                                         //     _block.UpdateBlockData();
//                                         //
//                                         //     tempPath.Add(_block.gameObject);
//                                         //
//                                         //      GetBlockEditorManager().InitializeAddTable();
//                                         //      GetBlockEditorManager().AddBlock(_block, _pos, NormalFace.PosY );
//                                         //     
//                                         //      GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition);
//                                         //
//                                         //     DestroyImmediate(_blockPrefab);
//                                         // }
//                                     }
//
//                                 }
//                             }
//                             break;
//                         
//                         case EventType.MouseUp:
//
//                             isPressed = false;
//                             
//                             Visualizer.ClearPathVisualizer();
//                             
//                            // if(isDragged) return;
//                             
//                             GameObject _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                             
//                             var _block = _blockPrefab.AddComponent<PathBlock>();
//                             
//                             _block.InjectDependency(GetBlockManager());
//                             _block.Init(originPos, originPos);
//                             _block.UpdateBlockData();
//                             
//                             
//                             GetBlockEditorManager().InitializeAddTable();
//                             
//                             GetBlockEditorManager().RemoveBlock(originPos);
//                             
//                             GetBlockEditorManager().AddBlock   (_block, originPos, NormalFace.None);
//                             
//                             
//                             GetBlockManager().UpdateSurroundingBlocksData<PathBlock>(_block.NodePosition);
//                             
//                             //GetBlockManager().UpdateMeshBlocks<PathBlock>();
//                             
//                             DestroyImmediate(_blockPrefab);
//
//                             isDragged = false;
//                             
//                             break;
//
//                     }
//                     break;
//                 
//                 case 1: /*Remove Road Block Path*/
//                   
//                     break;
//             }
//         }
//       
//     }
// }
