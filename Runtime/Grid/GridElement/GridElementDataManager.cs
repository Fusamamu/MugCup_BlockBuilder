using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Scriptable;
using MugCup_PathFinder.Runtime;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class GridElementDataManager : MonoBehaviour
    {
        [field: SerializeField] public GridElementData     GridElementData     { get; private set; }
        [field: SerializeField] public DualGridElementData DualGridElementData { get; private set; }
        [field: SerializeField] public VolumePointData     VolumePointData     { get; private set; }
        [field: SerializeField] public ModuleSlotData      ModuleSlotData      { get; private set; }
        
        [field:Unity.Collections.ReadOnly, SerializeField] public GridDataSettingSO GridDataSetting { get; private set; }

        [Unity.Collections.ReadOnly, SerializeField] private GameObject GridElementParent;
        [Unity.Collections.ReadOnly, SerializeField] private GameObject DualGridElementParent;
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
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(GridElementData);
            #endif
            
            return this;
        }

        public GridElementDataManager GenerateDualGrid()
        {
            var _dualGridElementPrefab = AssetManager.AssetCollection.DualGridElement.gameObject;
            DualGridElementParent = new GameObject("[Dual Grid Elements]");
            DualGridElementData.GridNodes = GridGenerator.GenerateDualGrid<DualGridElement>(GridDataSetting.GridUnitSize, _dualGridElementPrefab, DualGridElementParent);
            DualGridElementData.Initialized();

            ModuleSlotData .GridNodes = DualGridElementData.GetModuleSlots ().ToArray();
            VolumePointData.GridNodes = DualGridElementData.GetVolumePoints().ToArray();
            
            foreach (var _element in GridElementData.ValidNodes)
            {
                var _coord  = _element.NodeGridPosition;
                var _points = GridGenerator.GetCornerNodes(_coord, GridDataSetting.GridUnitSize, VolumePointData.GridNodes);
                            
                _element.SetVolumePoints(_points);
                
                #if UNITY_EDITOR
                EditorUtility.SetDirty(_element);
                #endif
            }
            
            foreach (var _point in DualGridElementData.GetVolumePoints())
            {
                if(_point == null) continue;
                
                _point.SetAdjacentBlocks(GridElementData.GridNodes, GridDataSetting.GridUnitSize);
                _point.Init();
                
                #if UNITY_EDITOR
                EditorUtility.SetDirty(_point);
                #endif
            }
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(DualGridElementData);
            EditorUtility.SetDirty(ModuleSlotData);
            EditorUtility.SetDirty(VolumePointData);
            #endif
            
            return this;
        }
        
        public GridElementDataManager GenerateVolumePoints()
        {
            var _volumePointPrefab = AssetManager.AssetCollection.DualGridElement.gameObject;
            VolumePointParent = new GameObject("[Volume Points]");
            VolumePointData.GridNodes = GridGenerator.GenerateDualGrid<VolumePoint>(GridDataSetting.GridUnitSize, _volumePointPrefab, VolumePointParent);        

            foreach (var _element in GridElementData.ValidNodes)
            {
                var _coord  = _element.NodeGridPosition;
                var _points = GridGenerator.GetCornerNodes(_coord, GridDataSetting.GridUnitSize, VolumePointData.GridNodes);
                            
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

        public GridElementDataManager ClearGrid()
        {
            DestroyGridNodes(GridElementData);
            DestroyParentObject(ref GridElementParent);
            return this;
        }

        public GridElementDataManager ClearVolumePoints()
        {
            DestroyGridNodes(VolumePointData);
            DestroyParentObject(ref VolumePointParent);
            return this;
        }

        public GridElementDataManager ClearDualGridElements()
        {
            DestroyGridNodes(DualGridElementData);
            DestroyParentObject(ref DualGridElementParent);
            return this;
        }

        private void DestroyGridNodes<T>(GridData<T> _gridData) where T : Component, IGridCoord
        {
            foreach (var _point in _gridData.GridNodes)
            {
                if(_point == null) continue;
                DestroyObject(_point.gameObject);
            }
            _gridData.ClearData();
        }

        private void DestroyParentObject(ref GameObject _object)
        {
            if (_object != null)
            {
                DestroyObject(_object);
                _object = null;
            }
        }

        private void DestroyObject(GameObject _object)
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(_object);
                return;
            }
            #endif
                
            Destroy(_object);
        }
    }
}
