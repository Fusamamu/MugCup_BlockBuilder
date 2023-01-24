using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder
{
	public class GridElement : MonoBehaviour, IGridCoord
	{
		[field: SerializeField] public Vector3Int NodeGridPosition  { get; private set; }
		[field: SerializeField] public Vector3    NodeWorldPosition { get; private set; }


		public IGridCoord SetNodePosition(Vector3Int _nodePosition)
		{
			return this;
		}
		public IGridCoord SetNodeWorldPosition(Vector3 _worldPosition)
		{
			return this;
		}
		
		[field: SerializeField] public bool IsInit   { get; private set; }
		[field: SerializeField] public bool IsEnable { get; private set; }

		public VolumePoint[] VolumePoints = new VolumePoint[8];
        
		public void SetVolumePoints(VolumePoint[] _volumePoints)
		{
		    VolumePoints = _volumePoints;
		}
		
		public void Enable()
		{
		    IsEnable = true;
		
		    foreach (var _point in VolumePoints)
		    {
		        _point.SetBitMask   ();
		        _point.SetCornerMesh();
		    }
		}
		
		public void Disable()
		{
		    IsEnable = false;
		    
		    foreach (var _point in VolumePoints)
		    {
		        _point.SetBitMask   ();
		        _point.SetCornerMesh();
		    }
		}
	}
}
