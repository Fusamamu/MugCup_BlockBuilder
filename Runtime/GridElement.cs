using System;
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

		[SerializeField] private BoxCollider BoxCollider;
		[SerializeField] private BoxCollider InsideBoxCollider;
		[SerializeField] private BoxCollider OutsideBoxCollider;

		[SerializeField] private bool ShowGizmos;

		public void SetShowGizmos(bool _value)
		{
			ShowGizmos = _value;
		}

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				if (IsEnable)
					Enable();
				else
					Disable();
			}
		}

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
			    if(_point == null) continue;
			    
		        _point.SetBitMask   ();
		        _point.SetCornerMesh();
		    }

		    BoxCollider.enabled = true;
		}
		
		public void Disable()
		{
		    IsEnable = false;
		    
		    foreach (var _point in VolumePoints)
		    {
			    if(_point == null) continue;
			    
		        _point.SetBitMask   ();
		        _point.SetCornerMesh();
		    }

		    BoxCollider.enabled = false;
		}

		private void OnDrawGizmos()
		{
			if(!ShowGizmos) return;
			
			Gizmos.color = IsEnable ? Color.green : Color.red;
			Gizmos.DrawCube(transform.position, Vector3.one / 10);
		}

		private void OnDrawGizmosSelected()
		{
			if(!ShowGizmos) return;
			
			Gizmos.color = Color.red;

			foreach (var _point in VolumePoints)
			{
				if(_point == null) continue;

				Gizmos.DrawLine(transform.position, _point.transform.position);
			}
		}
	}
}
