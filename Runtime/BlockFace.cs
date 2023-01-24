using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBuilder.Core
{
	public enum Facing
	{
		SOUTH = 0, 
		WEST  = 1, 
		NORTH = 2, 
		EAST  = 3
	}
	
	public enum NormalFace
	{
		PosX, NegX, PosY, PosZ, NegZ, None
	}

	public static class BlockFaceUtil
	{
		public static NormalFace GetSelectedFace(RaycastHit _hit)
		{
			return GetSelectedFace(_hit.normal);
		}
		
		public static NormalFace GetSelectedFace(Vector3 _hitNormal)
		{
			if (_hitNormal.x > 0)
				return NormalFace.PosX;
			if (_hitNormal.x < 0)
				return NormalFace.NegX;
			if (_hitNormal.y > 0)
				return NormalFace.PosY;
			if (_hitNormal.z > 0)
				return NormalFace.PosZ;
			if (_hitNormal.z < 0)
				return NormalFace.NegZ;

			return NormalFace.None;
		}
	}
}
