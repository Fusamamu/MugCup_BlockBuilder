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

		[SerializeField] private BoxCollider InsideBoxCollider;
		[SerializeField] private BoxCollider OutsideBoxCollider;

		[SerializeField] private Renderer Renderer;
		[SerializeField] private Material ActiveMaterial;
		[SerializeField] private Material InactiveMaterial;

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				if (Renderer == null)
				{
					Renderer = GetComponent<Renderer>();
				}
				
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
		        
		        Debug.Log(_point.NodeGridPosition);
		    }

		    InsideBoxCollider .enabled = false;
		    OutsideBoxCollider.enabled = true;

		    Renderer.material = ActiveMaterial;
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
		    
		    InsideBoxCollider .enabled = true;
		    OutsideBoxCollider.enabled = false;

		    Renderer.material = InactiveMaterial;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;

			foreach (var _point in VolumePoints)
			{
				if(_point == null) continue;

				Gizmos.DrawLine(transform.position, _point.transform.position);
			}
		}
	}
}
