using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;
using MugCup_PathFinder.Runtime;
using UnityEditor;

namespace MugCup_BlockBuilder.Editor
{
    public static class BlockPlacementTools
    {
        public static int SelectedIndex;
        public static GameObject SelectedBlock;

        public static bool IsSlotSelected(int _index)
        {
            return _index == SelectedIndex && _index != -1;
        }

        public static void UpdateBlockBuildTools(Event _currentEvent, Ray _ray)
        {
            switch (BlockBuilderEditorManager.InterfaceSetting.BlockPlacementToolTabSelection)
            {
                case 0: 
                    PlaceBlockElement(_currentEvent, _ray);
                    break;
                
                case 1: 
                    
                    if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
                    {
                        if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                        {
                           
                            var _object = _hit.collider.gameObject;
                            
                            if (_object.TryGetComponent<NodeBase>(out var _nodeBase))
                            {
                                BBEditorUtility.RecordGridBlockManagerChanges(() =>
                                {
                                    BlockBuilderEditorManager.GetBlockManager().RemoveNode<NodeBase>(_nodeBase);
                                    
                                    // BlockBuilderEditorManager.GetBlockManager().RemoveBlock(_block);
                                    // BlockBuilderEditorManager.GetBlockManager().UpdateSurroundBlocksBitMask(_block.NodePosition, CubeBlockSection.Middle);
                                });
                            }
                        }
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
                            var _object = _hit.collider.gameObject;
                        
                            if (_object.TryGetComponent<Block>(out var _block))
                            {
                                BBEditorUtility.RecordGridBlockManagerChanges(() =>
                                {
                                    if (SelectedBlock != null)
                                    {
                                        if (SelectedBlock.TryGetComponent<NodeBase>(out var _node))
                                        {
                                            var _targetPos = _hit.collider.transform.position;
                                            var _pos       = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
                                            var _newBlock  = BlockBuilderEditorManager.GetBlockEditorManager().AddNodeOnTop(_node, _pos);
                                            
                                            Undo.RegisterCreatedObjectUndo(_newBlock.gameObject, "New Block");
                                        }
                                    }
                                });
                            }
                        }
                    }
                    break;

                case EventType.MouseMove:
                    break;
            }
        }
    }
}
