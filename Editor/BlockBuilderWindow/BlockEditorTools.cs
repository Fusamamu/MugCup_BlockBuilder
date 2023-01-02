using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
    public static class BlockEditorTools 
    {
        public static void UpdateBlockBuildTools(Event _currentEvent, Ray _ray)
        {
            switch (BBEditorManager.InterfaceSetting.BuildToolTabSelection)
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
                            
                                    var _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    var _pos         = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
                                    var _block       = _blockPrefab.AddComponent<Block>();
                            
                                    _block
                                        .InjectDependency(BBEditorManager.BlockManager)
                                        .SetPosition(_targetPos, _pos)
                                        .Init();
                                    
                                    _block.UpdateBlockData();
                                    
                                    BBEditorUtility.RecordGridBlockManagerChanges(() =>
                                    {
                                        BBEditorManager.BlockEditorManager.InitializeAddTable();
                                        BBEditorManager.BlockEditorManager.AddBlock(_block, _pos, NormalFace.PosY );
                            
                                        BBEditorManager.BlockManager.UpdateSurroundingBlocksData<Block>(_block.NodeGridPosition, CubeBlockSection.Top);
                                    });
                                    
                                    Object.DestroyImmediate(_blockPrefab);
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
                                BBEditorUtility.RecordGridBlockManagerChanges(() =>
                                {
                                    BBEditorManager.BlockManager.RemoveBlock(_block);
                                    BBEditorManager.BlockManager.UpdateSurroundingBlocksData<Block>(_block.NodeGridPosition, CubeBlockSection.Middle);
                                });
                            }
                        }
                    }
                    break;
            }
        }
    }
}
