using System;
using UnityEngine;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace BlockBuilder.Runtime.Core
{
	public class GridBlockSelection : MonoBehaviour, IBlockRaycaster
	{
		private Camera mainCamera;
		
		public LayerMask groundMask;
		
		[SerializeField] private Block startBlock;
		[SerializeField] private Block endBlock;

		private Vector3Int startPos;
		private Vector3Int currentPos;
		private Vector3Int endNodePos;

		private BoundsInt bounds;

#region Callbacks
		public static event Action<Vector3> OnGridHit = delegate { };
		public static event Action OnOutOfGridBounds  = delegate { };
#endregion

		private static bool enable = true;

		public static void Enable () => enable = true;
		public static void Disable() => enable = false;
		
#region Public API
		public static void ClearAllEvents()
		{
			OnGridHit         = null;
			OnOutOfGridBounds = null;
		}
		
		public Block GetHitBlock()
		{
			if (hitBlock != null)
				return hitBlock;
			return null;
		}

		public T GetHitObject<T>() where T : Block
		{
			if (hitBlock != null)
				return hitBlock as T;
			return default;
		}
		
		public Vector3 GetHitNormal()
		{
			return hitNormal;
		}
#endregion

		private Block hitBlock;
		private Vector3 hitNormal;
		private Ray ray;
		private RaycastHit hit;
		
		private void Awake()
		{
			mainCamera = Camera.main;

			groundMask = LayerMask.GetMask("Block");
		}
		
		private void Update()
		{
			if(!enable) return;
			
			if (!GetRaycastHit())
			{
				hitBlock = null;
				OnOutOfGridBounds?.Invoke();
				return;
			}
			
			hitNormal = hit.normal;
			hitBlock  = GetBlockFromRaycast();

			if (hitBlock != null)
			{
				Vector3Int _hitPos = hitBlock.NodePosition;
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

		private Block GetBlockFromRaycast()
		{
			return hit.collider.GetComponent<Block>();
		}

		private Vector3Int GetBlockPosition()
		{
			return GetBlockFromRaycast().NodePosition;
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
