using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;
using UnityEditor;

namespace MugCup_BlockBuilder.Editor
{
    public static class BlockPlacementTools 
    {
        public static void UpdateBlockBuildTools(Event _currentEvent, Ray _ray)
        {
            switch (BlockBuilderEditorManager.InterfaceSetting.BlockPlacementToolTabSelection)
            {
                case 0: 
                    PlaceBlockElement(_currentEvent, _ray);
                    break;
                
                case 1: /*Subtract Block*/
                    if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
                    {
                        // Debug.Log($"<color=yellow>[Info]:</color> <color=orange>Left Mouse Button Clicked.</color>");
                        //
                        // if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                        // {
                        //     var _object = _hit.collider.gameObject;
                        //
                        //     if (_object.TryGetComponent<Block>(out var _block))
                        //     {
                        //         BBEditorUtility.RecordGridBlockManagerChanges(() =>
                        //         {
                        //             BlockBuilderEditorManager.GetBlockManager().RemoveBlock(_block);
                        //             BlockBuilderEditorManager.GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition, CubeBlockSection.Middle);
                        //         });
                        //     }
                        // }
                    }
                    break;
            }
        }
        private static void PlaceBlockElement(Event _currentEvent, Ray _ray)
        {
            switch (_currentEvent.type)
            {
                case EventType.MouseDown:

                    if (_currentEvent.button == 0)
                    {
                        if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                        {
                            Vector3 _targetPos = _hit.collider.transform.position;

                            var _blockPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var _pos   = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
                            var _block = _blockPrefab.AddComponent<Block>();

                            _block.InjectDependency(BlockBuilderEditorManager.GetBlockManager());
                            _block.Init(_targetPos, _pos);
                            _block.UpdateBlockData();

                            BBEditorUtility.RecordGridBlockManagerChanges(() =>
                            {
                                BlockBuilderEditorManager.GetBlockEditorManager().InitializeAddTable();
                                BlockBuilderEditorManager.GetBlockEditorManager().AddBlock(_block, _pos, NormalFace.PosY);

                                BlockBuilderEditorManager.GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition, CubeBlockSection.Top);
                            });

                            Object.DestroyImmediate(_blockPrefab);
                        }
                    }
                    break;

                case EventType.MouseMove:
                    break;
            }
        }
    }
}
