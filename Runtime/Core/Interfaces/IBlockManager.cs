using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core.Interfaces
{
	public interface IBlockManager
	{
		public bool IsOccupied(Vector3Int _nodePos);
		
		public void AddIBlockRef    (IBlock _newBlock, Vector3Int _nodePos);
		public void RemoveIBlockRef (Vector3Int _nodePos);

		public void DestroyIBlockObject(Vector3Int _nodePos);
		
		public IBlock       GetIBlock  (Vector3Int _nodePos);
		public List<IBlock> GetIBlocks (Vector3Int _startPos, Vector3Int _endPos);
		public List<IBlock> GetIBlocks3x3Cube (Vector3Int _nodePos);
	}
}
