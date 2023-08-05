#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

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
        
        public static void UpdateMarchingCubeEditMode(Event _currentEvent, Ray _ray)
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
                                    var _selectedFace = BlockFaceUtil.GetSelectedFace(_hit);
                                    
                                    if (_hit.collider.TryGetComponent<GridElement>(out var _gridElement))
                                    {
                                        if (_selectedFace == NormalFace.PosY)
                                        {
                                            Vector3 _targetPos = _hit.collider.transform.position;
                                            var _pos  = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
                                            var _node = BBEditorManager.GridElementDataManager.GridElementData.GetNodeUp(_pos);
                                            
                                            _node.Enable();
                                        }
                                    }
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
                        if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                        {
                            var _selectedFace = BlockFaceUtil.GetSelectedFace(_hit);
                                    
                            if (_hit.collider.TryGetComponent<GridElement>(out var _gridElement))
                            {
                                if (_selectedFace == NormalFace.PosY)
                                {
                                    Vector3 _targetPos = _hit.collider.transform.position;
                                    var _pos  = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);
                                    var _node = BBEditorManager.GridElementDataManager.GridElementData.GetNode(_pos);
                                            
                                    _node.Disable();
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public static void UpdateMarchingCubeChangeBlockTypeMode(Event _event, Ray _ray)
        {
             switch (BBEditorManager.InterfaceSetting.EditBlockTypeToolTabSelection)
            {
                case 0: /*Change Block Type*/
                    
                    switch (_event.type)
                    {
                        case EventType.MouseDown:
                            
                            if (_event.button == 0)
                            {
                                if (Physics.Raycast(_ray.origin, _ray.direction, out RaycastHit _hit, Mathf.Infinity))
                                {
                                    if (_hit.collider.TryGetComponent<GridElement>(out var _gridElement))
                                    {
                                        var _blockTypeIndex = BBEditorManager.InterfaceSetting.BlockTypeTabSelection;

                                        Char _selectedType = '\0';

                                        if (_blockTypeIndex == 0)
                                            _selectedType = '1';

                                        if (_blockTypeIndex == 1)
                                            _selectedType = '2';

                                        Vector3 _targetPos = _hit.collider.transform.position;
                                        var _pos  = new Vector3Int((int)_targetPos.x, (int)_targetPos.y, (int)_targetPos.z);

                                        _gridElement.SetMetaType(_selectedType);
                                        _gridElement.Enable();

                                        // var _node = BBEditorManager.GridElementDataManager.GridElementData.GetNode(_pos);
                                        // _node.Enable();
                                    }
                                }
                            }
                            break;
                        
                        case EventType.MouseMove:
                            break;
                    }
                    break;
                
                case 1: /*Remove Block Type*/
                    
                    if (_event.type == EventType.MouseDown && _event.button == 0)
                    {
                        
                    }
                    break;
            }
        }
    }
}
#endif
