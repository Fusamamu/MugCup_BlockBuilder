using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class VolumePoint : MonoBehaviour
    {
        private Vector3Int coord;

        [SerializeField] private Block[] adjacentBlocks = new Block[8];

        private int bitMask;

        private MeshFilter mesh;
        private Renderer   renderer;
        
        public void Init(Vector3Int _coord)
        {
	        coord = _coord;
	        
			name = "CE_" + coord.x +"_" + coord.y +"_" + coord.z;
			
	        mesh     = GetComponent<MeshFilter>();
	        renderer = GetComponent<Renderer>();
        }
	
        public void SetPosition(float _x, float _y, float _z)
        {
	        transform.position = new Vector3(_x, _y, _z);
        }

        public void SetCornerElement()
        {	
	        // int _nextBitMaskValue = bitMask.GetBitMask(nearGridElements);
	        //
	        // if((_nextBitMaskValue == 0) && (bitMaskValue != 0))
	        // {
	        // 	bitMask = _nextBitMaskValue;
	        // }
	        // else
	        // {
	        // 	bitMask = _nextBitMaskValue;
	        // 	mesh.mesh = cornerMeshes.instance.GetCornerMesh(bitMaskValue, coord.y);
	        // }
        }

        public void SetAdjacentBlocks(Block[] _blocks, Vector3Int _gridUnitSize)
        {
	        int _rowUnit    = _gridUnitSize.x;
	        int _columnUnit = _gridUnitSize.z;
	        int _levelUnit  = _gridUnitSize.y;

	        var _x = coord.x;
	        var _y = coord.y;
	        var _z = coord.z;

	        if (_x < _rowUnit && _y < _levelUnit && _z < _columnUnit)
	        {
		        //Upper North East Block
		        adjacentBlocks[0] = _blocks[_z +     _gridUnitSize.x * (_x + _gridUnitSize.y * _y)];
	        }
	        if (_x < _rowUnit && _y < _levelUnit && _z > 0)
	        {
		        //Upper North West Block
		        adjacentBlocks[1] = _blocks[_z - 1 + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)];
	        }
	        
	        if (_x > 0 && _y < _levelUnit && _z > 0)
	        {
		        //Upper South West Block
		        adjacentBlocks[2] = _blocks[_z - 1 + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * _y)];
	        }
	        if (_x > 0 && _y < _levelUnit && _z < _columnUnit)
	        {
		        //Upper South East Block
		        adjacentBlocks[3] = _blocks[_z     + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * _y)];
	        }
	        
	        if (_x < _rowUnit && _y > 0 && _z < _columnUnit)
	        {
		        //Bottom North East Block
		        adjacentBlocks[4] = _blocks[_z +     _gridUnitSize.x * (_x +    _gridUnitSize.y * (_y - 1))];
	        }
	        if (_x < _rowUnit && _y > 0 && _z > 0)
	        {
		        //Bottom North West Block
		        adjacentBlocks[5] = _blocks[_z - 1 + _gridUnitSize.x * (_x +    _gridUnitSize.y * (_y - 1))];
	        }
	        
	        if (_x > 0 && _y > 0 && _z > 0)
	        {
		        //Bottom South West Block
		        adjacentBlocks[6] = _blocks[_z - 1 + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * (_y - 1))];
	        }
	        if (_x > 0 && _y > 0 && _z < _columnUnit)
	        {
		        //Bottom South East Block
		        adjacentBlocks[7] = _blocks[_z     + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * (_y - 1))];
	        }
	        
	    
        }


	// public void SetNearGridElements()
	// {
	// 	int width = levelGenerator.instance.width;
	// 	int height = levelGenerator.instance.height;
	//
	// 	if(coord.x < width && coord.y < height && coord.z < width)
	// 	{
	// 		//UpperNorthEast
	// 		nearGridElements[0] = levelGenerator.instance.gridElements[coord.x + width * (coord.z + width * coord.y)];
	// 	}
	// 	if(coord.x > 0 && coord.y < height & coord.z < width)
	// 	{
	// 		//UpperNorthWest
	// 		nearGridElements[1] = levelGenerator.instance.gridElements[coord.x - 1 + width * (coord.z + width * coord.y)];
	// 	}
	// 	if(coord.x > 0 && coord.y < height & coord.z > 0)
	// 	{
	// 		//UpperSouthWest
	// 		nearGridElements[2] = levelGenerator.instance.gridElements[coord.x - 1 + width * (coord.z - 1 + width * coord.y)];
	// 	}
	// 	if(coord.x < width && coord.y < height && coord.z > 0)
	// 	{
	// 		//UpperSouthEast
	// 		nearGridElements[3] = levelGenerator.instance.gridElements[coord.x + width * (coord.z - 1 + width * coord.y)];
	// 	}
	//
	//
	// 	if(coord.x < width && coord.y > 0 && coord.z < width)
	// 	{
	// 		//LowerNorthEast
	// 		nearGridElements[4] = levelGenerator.instance.gridElements[coord.x + width * (coord.z + width * (coord.y - 1))];
	// 	}
	// 	if(coord.x > 0 && coord.y > 0 & coord.z < width)
	// 	{
	// 		//LowerNorthWest
	// 		nearGridElements[5] = levelGenerator.instance.gridElements[coord.x - 1 + width * (coord.z + width * (coord.y - 1))];
	// 	}
	// 	if(coord.x > 0 && coord.y > 0 & coord.z > 0)
	// 	{
	// 		//LowerSouthWest
	// 		nearGridElements[6] = levelGenerator.instance.gridElements[coord.x - 1 + width * (coord.z - 1 + width * (coord.y - 1))];
	// 	}
	// 	if(coord.x < width && coord.y > 0 && coord.z > 0)
	// 	{
	// 		//LowerSouthEast
	// 		nearGridElements[7] = levelGenerator.instance.gridElements[coord.x + width * (coord.z - 1 + width * (coord.y - 1))];
	// 	}
	// }
        

    }
}
