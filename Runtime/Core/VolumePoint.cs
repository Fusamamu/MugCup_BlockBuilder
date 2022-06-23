using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class VolumePoint : MonoBehaviour
    {
        [SerializeField] private Vector3Int coord;
        
        [SerializeField] private int bitMask;

        [SerializeField] private Block[] adjacentBlocks = new Block[8];

        private MeshFilter mesh;
        private Renderer   renderer;

        public Block UpperNorthEastBlock;
        public Block UpperNorthWestBlock;
        public Block UpperSouthWestBlock;
        public Block UpperSouthEastBlock;
        
        public Block LowerNorthEastBlock;
        public Block LowerNorthWestBlock;
        public Block LowerSouthWestBlock;
        public Block LowerSouthEastBlock;
        
        public void Init(Vector3Int _coord)
        {
	        coord = _coord;
	        
			name = "CE_" + coord.x +"_" + coord.y +"_" + coord.z;
			
	        mesh     = GetComponent<MeshFilter>();
	        renderer = GetComponent<Renderer>();

	        UpperNorthEastBlock = adjacentBlocks[0];
	        UpperNorthWestBlock = adjacentBlocks[1];
	        UpperSouthWestBlock = adjacentBlocks[2]; 
	        UpperSouthEastBlock = adjacentBlocks[3];
	        LowerNorthEastBlock = adjacentBlocks[4];
	        LowerNorthWestBlock = adjacentBlocks[5];
	        LowerSouthWestBlock = adjacentBlocks[6];
	        LowerSouthEastBlock = adjacentBlocks[7];
        }
	
        public void SetPosition(float _x, float _y, float _z)
        {
	        transform.position = new Vector3(_x, _y, _z);
        }

        private void SetAdjacentBlocks()
        {
	        SetAdjacentBlocks(null, Vector3Int.back);
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
		        UpperNorthEastBlock = _blocks[_z +     _gridUnitSize.x * (_x + _gridUnitSize.y * _y)];
	        }
	        if (_x < _rowUnit && _y < _levelUnit && _z > 0)
	        {
		        //Upper North West Block
		        UpperNorthWestBlock = _blocks[_z - 1 + _gridUnitSize.x * (_x + _gridUnitSize.y * _y)];
	        }
	        
	        if (_x > 0 && _y < _levelUnit && _z > 0)
	        {
		        //Upper South West Block
		        UpperSouthWestBlock = _blocks[_z - 1 + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * _y)];
	        }
	        if (_x > 0 && _y < _levelUnit && _z < _columnUnit)
	        {
		        //Upper South East Block
		        UpperSouthEastBlock  = _blocks[_z     + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * _y)];
	        }
	        
	        if (_x < _rowUnit && _y > 0 && _z < _columnUnit)
	        {
		        //Lower North East Block
		        LowerNorthEastBlock = _blocks[_z +     _gridUnitSize.x * (_x +    _gridUnitSize.y * (_y - 1))];
	        }
	        if (_x < _rowUnit && _y > 0 && _z > 0)
	        {
		        //Lower North West Block
		        LowerNorthWestBlock = _blocks[_z - 1 + _gridUnitSize.x * (_x +    _gridUnitSize.y * (_y - 1))];
	        }
	        
	        if (_x > 0 && _y > 0 && _z > 0)
	        {
		        //Lower South West Block
		        LowerSouthWestBlock = _blocks[_z - 1 + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * (_y - 1))];
	        }
	        if (_x > 0 && _y > 0 && _z < _columnUnit)
	        {
		        //Lower South East Block
		        LowerSouthEastBlock = _blocks[_z     + _gridUnitSize.x * (_x - 1 + _gridUnitSize.y * (_y - 1))];
	        }
        }
        
        public void SetCornerMesh()
        {	
	        //mesh.mesh = cornerMeshes.instance.GetCornerMesh(bitMaskValue, coord.y);
        }

        public void SetBitMask()
        { 
	        bitMask = 0b_0000_0000;

	        if (UpperNorthEastBlock != null && UpperNorthEastBlock.IsEnable)
	        {
		        bitMask += 0b_0000_0001;
	        }
	        if (UpperNorthWestBlock != null && UpperNorthWestBlock.IsEnable)
	        {
		        bitMask += 0b_0000_0010;
	        }
	        if (UpperSouthWestBlock != null && UpperSouthWestBlock.IsEnable)
	        {
		        bitMask += 0b_0000_0100;
	        }
	        if (UpperSouthEastBlock != null && UpperSouthEastBlock.IsEnable)
	        {
		        bitMask += 0b_0000_1000;
	        }

	        if (LowerNorthEastBlock != null && LowerNorthEastBlock.IsEnable)
	        {
		        bitMask += 0b_0001_0000;
	        }
	        if (LowerNorthWestBlock != null && LowerNorthWestBlock.IsEnable)
	        {
		        bitMask += 0b_0010_0000;
	        }
	        if (LowerSouthWestBlock != null && LowerSouthWestBlock.IsEnable)
	        {
		        bitMask += 0b_0100_0000;
	        }
	        if (LowerSouthEastBlock != null && LowerSouthEastBlock.IsEnable)
	        {
		        bitMask += 0b_1000_0000;
	        }
        }
    }
}
