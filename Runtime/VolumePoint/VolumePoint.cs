using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockBuilder.Core.Scriptable;
using MugCup_BlockBuilder.Runtime;
using MugCup_PathFinder.Runtime;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class VolumePoint : MonoBehaviour, IGridCoord
    {
	    [field: SerializeField] public Vector3Int NodeGridPosition  { get; private set; }
	    [field: SerializeField] public Vector3    NodeWorldPosition { get; private set; }
	    
	    //Temp
	    //TODO : Need a way to load cornermeshmoduledata 
	    //Currently it is set directly in prefab in package
	    public CornerMeshModuleData CornerMeshModuleData;

	    //Temp
	    private static Dictionary<int, Module> moduleTable = new Dictionary<int, Module>();

	    public IGridCoord SetNodePosition(Vector3Int _nodePosition)
	    {
		    NodeGridPosition = _nodePosition;
		    return this;
	    }
	    public IGridCoord SetNodeWorldPosition(Vector3 _worldPosition)
	    {
		    NodeWorldPosition = _worldPosition;
		    return this;
	    }
        
        [field: SerializeField] public int    BitMask  { get; private set; }
        [field: SerializeField] public string MetaData { get; private set; }

        [SerializeField] private GridElement[] AdjacentGridElements = new GridElement[8];

        [SerializeField] private MeshFilter Mesh;
        [SerializeField] private Renderer   Renderer;

        public GridElement UpperNorthEastGridElement;
        public GridElement UpperNorthWestGridElement;
        public GridElement UpperSouthWestGridElement;
        public GridElement UpperSouthEastGridElement;
        
        public GridElement LowerNorthEastGridElement;
        public GridElement LowerNorthWestGridElement;
        public GridElement LowerSouthWestGridElement;
        public GridElement LowerSouthEastGridElement;

        [SerializeField] private bool ShowGizmos;
        [SerializeField] private bool ShowDebugText;
        [SerializeField] private bool ShowGizmosOnSelected;
        
        public void Init()
        {
			name = "CE_" + NodeGridPosition.x +"_" + NodeGridPosition.y +"_" + NodeGridPosition.z;
			
	        Mesh     = GetComponent<MeshFilter>();
	        Renderer = GetComponent<Renderer>();
        }
	
        public void SetPosition(float _x, float _y, float _z)
        {
	        transform.position = new Vector3(_x, _y, _z);
        }

        public void SetAdjacentBlocks(GridElement[] _gridElements, Vector3Int _gridUnitSize)
        {
	        int _rowUnit    = _gridUnitSize.x;
	        int _columnUnit = _gridUnitSize.z;
	        int _levelUnit  = _gridUnitSize.y;

	        var _x = NodeGridPosition.x;
	        var _y = NodeGridPosition.y;
	        var _z = NodeGridPosition.z;

	        if (_y < _levelUnit)
	        {
		        if (_x < _rowUnit && _z < _columnUnit)
			        UpperNorthEastGridElement = _gridElements[_z + _rowUnit * (_x + _columnUnit * _y)];
		        
		        if (_x > 0 && _z < _columnUnit)
			        UpperNorthWestGridElement = _gridElements[_z + _rowUnit * (_x - 1 + _columnUnit * _y)];
		        
		        if (_x > 0 && _z > 0)
			        UpperSouthWestGridElement = _gridElements[_z - 1 + _rowUnit * (_x - 1 + _columnUnit * _y)];
			       
		        if (_x < _rowUnit && _z > 0)
			        UpperSouthEastGridElement = _gridElements[_z - 1 + _rowUnit * (_x + _columnUnit * _y)];
	        }

	        if (_y > 0)
	        {
		        if (_x < _rowUnit && _z < _columnUnit)
			        LowerNorthEastGridElement = _gridElements[_z + _rowUnit * (_x + _columnUnit * (_y - 1))];
		        
		        if (_x > 0 && _z < _columnUnit)
			        LowerNorthWestGridElement = _gridElements[_z + _rowUnit * (_x - 1 + _columnUnit * (_y - 1))];
		        
		        if (_x > 0 && _z > 0)
			        LowerSouthWestGridElement = _gridElements[_z - 1 + _rowUnit * (_x - 1 + _columnUnit * (_y - 1))];
		        
		        if (_x < _rowUnit && _z > 0)
			        LowerSouthEastGridElement = _gridElements[_z - 1 + _rowUnit * (_x + _columnUnit * (_y - 1))];
	        }
        }
        
        public void SetCornerMesh()
        {
	        var _metaTable = CornerMeshModuleData.GetMetaTable();
	        if (_metaTable.TryGetValue(BitMask, out var _metaDict))
	        {
		        if (!_metaDict.TryGetValue(MetaData, out var _module))
			        _module = _metaDict.Values.ToList().First();
		        
		        var _mesh = _module.MeshPrototype;
		        if (_mesh)
			        Mesh.mesh = _mesh;
	        }
	        else
	        {
		        Mesh.mesh = null;
	        }
        }

        public void SetBitMask(int _bit)
        {
	        BitMask = _bit;
        }

        public void UpdateBitMask()
        {
	        BitMask = 0b_0000_0000;

	        if (UpperNorthEastGridElement != null && UpperNorthEastGridElement.IsEnable)
	        {
		        BitMask += 0b_0000_0001;
	        }
	        
	        if (UpperNorthWestGridElement != null && UpperNorthWestGridElement.IsEnable)
	        {
		        BitMask += 0b_0000_0010;
	        }
	        
	        if (UpperSouthWestGridElement != null && UpperSouthWestGridElement.IsEnable)
	        {
		        BitMask += 0b_0000_0100;
	        }
	        
	        if (UpperSouthEastGridElement != null && UpperSouthEastGridElement.IsEnable)
	        {
		        BitMask += 0b_0000_1000;
	        }

	        if (LowerNorthEastGridElement != null && LowerNorthEastGridElement.IsEnable)
	        {
		        BitMask += 0b_0001_0000;
	        }
	        
	        if (LowerNorthWestGridElement != null && LowerNorthWestGridElement.IsEnable)
	        {
		        BitMask += 0b_0010_0000;
	        }
	        
	        if (LowerSouthWestGridElement != null && LowerSouthWestGridElement.IsEnable)
	        {
		        BitMask += 0b_0100_0000;
	        }
	        
	        if (LowerSouthEastGridElement != null && LowerSouthEastGridElement.IsEnable)
	        {
		        BitMask += 0b_1000_0000;
	        }
        }

        public void SetMetaData(string _metaData)
        {
	        MetaData = _metaData;
        }

        public void UpdateMetaData()
        {
	        MetaData = "00000000";

	        if (UpperNorthEastGridElement != null && UpperNorthEastGridElement.IsEnable)
	        {
		        //BitMask += 0b_0000_0001;

		        var _metaType = UpperNorthEastGridElement.MetaType == '\0' ? '1' : UpperNorthEastGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 7, MetaData);
	        }
	        
	        if (UpperNorthWestGridElement != null && UpperNorthWestGridElement.IsEnable)
	        {
		        //BitMask += 0b_0000_0010;
		        var _metaType = UpperNorthWestGridElement.MetaType == '\0' ? '1' : UpperNorthWestGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 6, MetaData);
	        }

	        if (UpperSouthWestGridElement != null && UpperSouthWestGridElement.IsEnable)
	        {
		        //BitMask += 0b_0000_0100;
		        var _metaType = UpperSouthWestGridElement.MetaType == '\0' ? '1' : UpperSouthWestGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 5, MetaData);
	        }
	        
	        if (UpperSouthEastGridElement != null && UpperSouthEastGridElement.IsEnable)
	        {
		        //BitMask += 0b_0000_1000;
		        var _metaType = UpperSouthEastGridElement.MetaType == '\0' ? '1' : UpperSouthEastGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 4, MetaData);
	        }
	        
	        if (LowerNorthEastGridElement != null && LowerNorthEastGridElement.IsEnable)
	        {
		        //BitMask += 0b_0001_0000;
		        var _metaType = LowerNorthEastGridElement.MetaType == '\0' ? '1' : LowerNorthEastGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 3, MetaData);
	        }
	        
	        if (LowerNorthWestGridElement != null && LowerNorthWestGridElement.IsEnable)
	        {
		        //BitMask += 0b_0010_0000;
		        var _metaType = LowerNorthWestGridElement.MetaType == '\0' ? '1' : LowerNorthWestGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 2, MetaData);
	        }
	        
	        if (LowerSouthWestGridElement != null && LowerSouthWestGridElement.IsEnable)
	        {
		        //BitMask += 0b_0100_0000;
		        var _metaType = LowerSouthWestGridElement.MetaType == '\0' ? '1' : LowerSouthWestGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 1, MetaData);
	        }
	        
	        if (LowerSouthEastGridElement != null && LowerSouthEastGridElement.IsEnable)
	        {
		        //BitMask += 0b_1000_0000;
		        var _metaType = LowerSouthEastGridElement.MetaType == '\0' ? '1' : LowerSouthEastGridElement.MetaType;
		        MetaData = MetaDataUtil.ReplaceMetaTypeAt(_metaType, 0, MetaData);
	        }
        }
        
#region Gizmos
        public void ToggleShowDebugText()
        {
	        ShowDebugText = !ShowDebugText;
        }

        public void ToggleShowGizmos()
        {
	        ShowGizmos = !ShowGizmos;
        }

        public void SetShowGizmos(bool _value)
        {
	        ShowGizmos = _value;
        }

        private void OnDrawGizmos()
        {
	        if(ShowGizmosOnSelected || !ShowGizmos) return;
	        // var _center = transform.position;
	        // Gizmos.color = Color.yellow;
	        // Gizmos.DrawSphere(_center, 0.05f);
	        // Gizmos.color = Color.green;
	        // Gizmos.DrawWireCube(_center, Vector3.one);
	        
	        if(!ShowDebugText) return;
	        
	        // Handles.Label(_center + Vector3.right/2,   PosXSocket);
	        // Handles.Label(_center + Vector3.left/2,    NegXSocket);
	        // Handles.Label(_center + Vector3.forward/2, PosZSocket);
	        // Handles.Label(_center + Vector3.back/2,    NegZSocket);
	        // Handles.Label(_center + Vector3.up/2,      PosYSocket);
	        // Handles.Label(_center + Vector3.down/2,    NegYSocket);

	        // var _guiStyle = new GUIStyle
	        // {
		       //  normal = { textColor = Color.yellow },
		       //  alignment = TextAnchor.MiddleCenter
	        // };
	        //
	        // Handles.Label(_center + Vector3.up,  gameObject.name, _guiStyle);
        }

        private void OnDrawGizmosSelected()
        {
	        // if(!ShowGizmosOnSelected || !ShowGizmos) return;
	        //
	        // var _center = transform.position;
	        // Gizmos.color = Color.yellow;
	        // Gizmos.DrawSphere(_center, 0.05f);
	        // Gizmos.color = Color.green;
	        // Gizmos.DrawWireCube(_center, Vector3.one);
	        //
	        // Handles.Label(_center + Vector3.right/2,   PosXSocket);
	        // Handles.Label(_center + Vector3.left/2,    NegXSocket);
	        // Handles.Label(_center + Vector3.forward/2, PosZSocket);
	        // Handles.Label(_center + Vector3.back/2,    NegZSocket);
	        // Handles.Label(_center + Vector3.up/2,      PosYSocket);
	        // Handles.Label(_center + Vector3.down/2,    NegYSocket);
	        //
	        // Handles.Label(_center + Vector3.up /1.5f,  gameObject.name);
        }
#endregion
    }
}
