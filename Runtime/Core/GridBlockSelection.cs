using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Collections;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_Utilities.Runtime.DesignPattern.Observer;

namespace MugCup_BlockBuilder.Runtime.Core
{
	public enum GridEvent
	{
		ON_GRID_HIT,
		ON_GIRD_OUT_OF_BOUND
	}
	
	public class GridBlockSelection : MC_Observable<GridEvent>, IBlockRaycaster
	{
		public Vector3Int HitPosition => hitPos;
		
		private Vector3Int startPos;
		private Vector3Int currentPos;
		private Vector3Int endNodePos;
		
		private Block    hitBlock;
		private Vector3  hitNormal;
		private Vector3Int  hitPos;
		
		private BoundsInt bounds;
		
		private Ray ray;
		private RaycastHit hit;

		[ReadOnly] [SerializeField] private Camera    mainCamera;
		[ReadOnly] [SerializeField] private LayerMask groundMask;　//-> This can't define layer by script beforehand. Need user to set in Unity
		
#region Public API
		public T GetHitObject<T>()  where T : Block
		{
			if (hitBlock != null)
				return hitBlock as T;
			
			return default;
		}

		//WIP//
		public T GetHitObjects<T>() where T : IEnumerable<Block>
		{
			return default;
		}
		
		public Vector3 GetHitNormal()
		{
			return hitNormal;
		}
#endregion
		
		private void Awake()
		{
			mainCamera = Camera.main;
			groundMask = LayerMask.GetMask("Block");
		}
		
		private void Update()
		{
			if (!GetRaycastHit())
			{
				hitBlock = null;
				PostEvent(GridEvent.ON_GIRD_OUT_OF_BOUND, this);
				return;
			}
			
			hitNormal = hit.normal;
			hitBlock  = hit.collider.GetComponent<Block>();

			if (hitBlock != null)
			{
				hitPos = hitBlock.NodePosition;
				PostEvent(GridEvent.ON_GRID_HIT, this);
			}
		}
		
		public bool GetRaycastHit()
		{
			var _mousePosition = GetMousePosition();
			
			ray = mainCamera.ScreenPointToRay(_mousePosition);
			
			return Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask);
		}
		
		private void OnDrawGizmos()
		{
			if(!Application.isPlaying) return;
			
			Debug.Log("Draw Gizmos");
			Gizmos.color = Color.red;
			
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}

		private Vector3 GetMousePosition()
		{
			var _mousePosition = Vector3.zero;
			
			_mousePosition = Mouse.current.position.ReadValue();
			
			return _mousePosition;
		}
	}
}
