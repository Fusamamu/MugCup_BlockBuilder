using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_Utilities;
using MugCup_Utilities.Runtime.DesignPattern.Observer;
using UnityEngine.InputSystem;

namespace BlockBuilder.Runtime.Core
{
	public enum GridEvent
	{
		ON_GRID_HIT,
		ON_GIRD_OUT_OF_BOUND
	}
	
	public class GridBlockSelection : MC_Observable<GridEvent>, IBlockRaycaster
	{
		private Vector3Int startPos;
		private Vector3Int currentPos;
		private Vector3Int endNodePos;
		
		private Block    hitBlock;
		private Vector3  hitNormal;
		
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
				PostEvent(GridEvent.ON_GRID_HIT, this);
				return;
			}
			
			hitNormal = hit.normal;
			hitBlock  = hit.collider.GetComponent<Block>();

			if (hitBlock != null)
			{
				var _hitPos = hitBlock.NodePosition;
				PostEvent(GridEvent.ON_GIRD_OUT_OF_BOUND, this);
			}
		}
		
		public bool GetRaycastHit()
		{
			var _mousePosition = GetMousePosition();
			
			ray = mainCamera.ScreenPointToRay(_mousePosition);
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
				return true;
			
			return false;
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
			
			#if ENABLE_INPUT_SYSTEM
			_mousePosition = Mouse.current.position.ReadValue();
			#elif ENABLE_LEGACY_INPUT_MANAGER
			_mousePosition = Input.mousePosition;
			#endif
			
			return _mousePosition;
		}
	}
}
