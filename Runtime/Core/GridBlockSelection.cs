using System;
using System.Collections.Generic;
using UnityEngine;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using Unity.Collections;

namespace BlockBuilder.Runtime.Core
{
	public class GridBlockSelection : MonoBehaviour, IBlockRaycaster
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
		
#region Callbacks
		public event Action<Vector3> OnGridHit          = delegate { };
		public event Action          OnOutOfGridBounds  = delegate { };
#endregion
		
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
				OnOutOfGridBounds?.Invoke();
				return;
			}
			
			hitNormal = hit.normal;
			hitBlock  = hit.collider.GetComponent<Block>();

			if (hitBlock != null)
			{
				var _hitPos = hitBlock.NodePosition;
				OnGridHit?.Invoke(_hitPos);
			}
		}
		
		public bool GetRaycastHit()
		{
			ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			
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
	}
}
