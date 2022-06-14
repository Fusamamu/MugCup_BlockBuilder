using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core.Interfaces
{
	public interface IBlockManager
	{
		public bool IsOccupied(Vector3Int _nodePos);
		
		public void AddBlockRef    (Block _newBlock, Vector3Int _nodePos);
		public void RemoveBlockRef (Vector3Int _nodePos);

		public void DestroyBlockObject(Vector3Int _nodePos);
		
		public Block       GetBlockRef  (Vector3Int _nodePos);
		public List<Block> GetBlocks (Vector3Int _startPos, Vector3Int _endPos);
		public List<Block> GetBlocks3x3Cube (Vector3Int _nodePos);
	}
}
