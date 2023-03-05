using System;
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class GridElementDataManager : MonoBehaviour
    {
        [field: SerializeField] public GridElementData GridElementData { get; private set; }
        [field: SerializeField] public VolumePointData VolumePointData { get; private set; }
        [field: SerializeField] public ModuleSlotData  ModuleSlotData  { get; private set; }
        
        [field:Unity.Collections.ReadOnly, SerializeField] public GridDataSettingSO GridDataSetting { get; private set; }

        [Unity.Collections.ReadOnly, SerializeField] private GameObject GridElementParent;
        [Unity.Collections.ReadOnly, SerializeField] private GameObject VolumePointParent;
        [Unity.Collections.ReadOnly, SerializeField] private GameObject ModuleSlotParent;

        public GridElementDataManager SetGridDataSetting(GridDataSettingSO _gridDataSetting)
        {
            GridDataSetting = _gridDataSetting;
            return this;
        }
        
        public void Initialized()
        {
            GridElementData.GridSize = GridDataSetting.GridUnitSize;
            GridElementData.InitializeGridArray();

            VolumePointData.GridSize = GridDataSetting.GridUnitSize + Vector3Int.one;
            VolumePointData.InitializeGridArray();
        }

        public GridElementDataManager GenerateGrid()
        {
            var _gridElementPrefab = AssetManager.AssetCollection.GridElement.gameObject;
            GridElementParent = new GameObject("[Grid Elements]");
            GridElementData.GridNodes = GridGenerator.GenerateGridBlocks<GridElement>(GridDataSetting.GridUnitSize, _gridElementPrefab, GridElementParent);
            
            return this;
        }
        
        public GridElementDataManager GenerateVolumePoints()
        {
            var _volumePointPrefab = AssetManager.AssetCollection.VolumePoint.gameObject;
            VolumePointParent = new GameObject("[Volume Points]");
            VolumePointData.GridNodes = GridGenerator.GenerateDualGrid<VolumePoint>(GridDataSetting.GridUnitSize, _volumePointPrefab, VolumePointParent);        

            foreach (var _element in GridElementData.ValidNodes)
            {
                var _coord  = _element.NodeGridPosition;
                var _points = VolumePointGenerator.GetVolumePoints(_coord, GridDataSetting.GridUnitSize, VolumePointData.GridNodes);
                            
                _element.SetVolumePoints(_points);
            }
            
            foreach (var _point in VolumePointData.GridNodes)
            {
                if(_point == null) continue;
                
                _point.SetAdjacentBlocks(GridElementData.GridNodes, GridDataSetting.GridUnitSize);
                _point.Init();
            }
            
            return this;
        }

        public GridElementDataManager GenerateModuleSlots()
        {
            var _moduleSlotPrefab = AssetManager.AssetCollection.VolumePoint.gameObject;
            ModuleSlotParent = new GameObject("[Module Slot Parent");
            ModuleSlotData.GridNodes = GridGenerator.GenerateDualGrid<ModuleSlot>(GridDataSetting.GridUnitSize, _moduleSlotPrefab, ModuleSlotParent);     
            
            
            
            return this;
        }

        public GridElementDataManager UpdateModuleSlotData()
        {
            ModuleSlotData.GridNodes = new ModuleSlot[VolumePointData.GridNodes.Length];

            for (var _i = 0; _i < ModuleSlotData.GridNodes.Length; _i++)
            {
                var _volumePoint = VolumePointData.GridNodes[_i];
                
                if(_volumePoint == null) continue;

                if (_volumePoint.TryGetComponent<ModuleSlot>(out var _slot))
                {
                    ModuleSlotData.GridNodes[_i] = _slot;

                    _slot
                        .SetModuleSlotData(ModuleSlotData)
                        .Initialized();
                    
                    #if UNITY_EDITOR
                    EditorUtility.SetDirty(_slot);
                    #endif
                }
            }
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(ModuleSlotData);
            #endif
       
            return this;
        }

        public GridElementDataManager ClearGrid()
        {
            foreach (var _element in GridElementData.GridNodes)
            {
                if(_element == null) continue;
                
                if (!Application.isPlaying)
                    DestroyImmediate(_element.gameObject);
                else
                    Destroy(_element.gameObject);
            }

            if (GridElementParent != null)
            {
                if(!Application.isPlaying)
                    DestroyImmediate(GridElementParent);
                else
                    Destroy(GridElementParent);

                GridElementParent = null;
            }
            
            GridElementData.ClearData();
            
            return this;
        }

        public GridElementDataManager ClearVolumePoints()
        {
            foreach (var _point in VolumePointData.GridNodes)
            {
                if(_point == null) continue;
                
                if (!Application.isPlaying)
                    DestroyImmediate(_point.gameObject);
                else
                    Destroy(_point.gameObject);
            }

            if (VolumePointParent != null)
            {
                if(!Application.isPlaying)
                    DestroyImmediate(VolumePointParent);
                else
                    Destroy(VolumePointParent);

                VolumePointParent = null;
            }
            
            VolumePointData.ClearData();

            return this;
        }
    }
}
