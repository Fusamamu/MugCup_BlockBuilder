using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Core.Scriptable;
using MugCup_BlockBuilder.Runtime;
using MugCup_PathFinder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class VolumePoint : MonoBehaviour, IGridCoord
    {
	    [field: SerializeField] public Vector3Int NodeGridPosition  { get; private set; }
	    [field: SerializeField] public Vector3    NodeWorldPosition { get; private set; }
	    
	    //Temp
	    public CornerMeshData CornerMeshData;

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
	    
        [SerializeField] private Vector3Int Coord;
        
        [SerializeField] private int BitMask;

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
        
        public void Init(Vector3Int _coord)
        {
	        Coord = _coord;
	        
			name = "CE_" + Coord.x +"_" + Coord.y +"_" + Coord.z;
			
	        Mesh     = GetComponent<MeshFilter>();
	        Renderer = GetComponent<Renderer>();

	        UpperNorthEastGridElement = AdjacentGridElements[0];
	        UpperNorthWestGridElement = AdjacentGridElements[1];
	        UpperSouthWestGridElement = AdjacentGridElements[2]; 
	        UpperSouthEastGridElement = AdjacentGridElements[3];
	        
	        LowerNorthEastGridElement = AdjacentGridElements[4];
	        LowerNorthWestGridElement = AdjacentGridElements[5];
	        LowerSouthWestGridElement = AdjacentGridElements[6];
	        LowerSouthEastGridElement = AdjacentGridElements[7];
        }
	
        public void SetPosition(float _x, float _y, float _z)
        {
	        transform.position = new Vector3(_x, _y, _z);
        }

        private void SetAdjacentBlocks()
        {
	        SetAdjacentBlocks(null, Vector3Int.back);
        }

        public void SetAdjacentBlocks(GridElement[] _gridElements, Vector3Int _gridUnitSize)
        {
	        int _rowUnit    = _gridUnitSize.x;
	        int _columnUnit = _gridUnitSize.z;
	        int _levelUnit  = _gridUnitSize.y;

	        var _x = Coord.x;
	        var _y = Coord.y;
	        var _z = Coord.z;

	        if (_x < _rowUnit && _y < _levelUnit && _z < _columnUnit)
	        {
		        //Upper North East Block
		        UpperNorthEastGridElement = _gridElements[_z +     _gridUnitSize.x * (_x + _gridUnitSize.y * _y)];
	        }
	        if (_x < _rowUnit && _y < _levelUnit && _z > 0)
	        {
		        //Upper North West Block
		        UpperNorthWestGridElement = _gridElements[_z - 1 + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)];
	        }
	        
	        if (_x > 0 && _y < _levelUnit && _z > 0)
	        {
		        //Upper South West Block
		        UpperSouthWestGridElement = _gridElements[_z - 1 + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * _y)];
	        }
	        if (_x > 0 && _y < _levelUnit && _z < _columnUnit)
	        {
		        //Upper South East Block
		        UpperSouthEastGridElement  = _gridElements[_z     + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * _y)];
	        }
	        
	        if (_x < _rowUnit && _y > 0 && _z < _columnUnit)
	        {
		        //Lower North East Block
		        LowerNorthEastGridElement = _gridElements[_z +     _gridUnitSize.x * (_x +    _gridUnitSize.y * (_y - 1))];
	        }
	        
	        if (_x < _rowUnit && _y > 0 && _z > 0)
	        {
		        //Lower North West Block
		        LowerNorthWestGridElement = _gridElements[_z - 1 + _gridUnitSize.x * (_x +    _gridUnitSize.y * (_y - 1))];
	        }
	        
	        if (_x > 0 && _y > 0 && _z > 0)
	        {
		        //Lower South West Block
		        LowerSouthWestGridElement = _gridElements[_z - 1 + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * (_y - 1))];
	        }
	        
	        if (_x > 0 && _y > 0 && _z < _columnUnit)
	        {
		        //Lower South East Block
		        LowerSouthEastGridElement = _gridElements[_z     + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * (_y - 1))];
	        }
        }
        
        public void SetCornerMesh()
        {
	        var _mesh = CornerMeshData.GetCornerMesh(BitMask);

	        if (_mesh != null)
	        {
		        Mesh.mesh = _mesh;
	        }
        }

        public void SetBitMask()
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
    }
}
