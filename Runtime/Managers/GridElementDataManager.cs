using System;
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class GridElementDataManager : MonoBehaviour
    {
        [field: SerializeField] public GridElementData GridElementData { get; private set; }
        [field: SerializeField] public VolumePointData VolumePointData { get; private set; }
        
        [field:Unity.Collections.ReadOnly, SerializeField] public GridDataSettingSO GridDataSetting { get; private set; }

        [Unity.Collections.ReadOnly, SerializeField] private GameObject GridElementParent;
        [Unity.Collections.ReadOnly, SerializeField] private GameObject VolumePointParent;

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
            VolumePointParent = new GameObject("[Volume Points]");
                    
            VolumePointData.GridNodes = VolumePointGenerator.GeneratedVolumePoints(GridDataSetting.GridUnitSize, 0.1f, VolumePointParent);

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
            }
            
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
