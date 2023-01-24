using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime.Core;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core.Interfaces
{
	public interface IBlockRaycaster
	{
		public bool GetRaycastHit();
		
		public T GetHitObject<T>() where T : Block;

		public T GetHitObjects<T>() where T : IEnumerable<Block>;

		public Vector3 GetHitNormal();
	}
}
