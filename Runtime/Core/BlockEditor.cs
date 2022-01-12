using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BlockBuilder;
using BlockBuilder.Core;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public class BlockEditor : MonoBehaviour
    {
#region Dependencies
        private BlockManager    blockManager;
        private IBlockRaycaster gridBlockSelection;
#endregion
        
        // public static void Enable () => enable = true;
        // public static void Disable() => enable = false;
        
        // private static bool enable = false;

        private readonly Dictionary<NormalFace, Action<Block, Vector3Int>> addTable = new Dictionary<NormalFace, Action<Block, Vector3Int>>();

        private void Awake()
        {
            blockManager = BlockManager.Instance;
            gridBlockSelection = FindObjectOfType<GridBlockSelection>();
        }

        private void Start()
        {
            AssetManager.LoadAssets();
            
            addTable.Add(NormalFace.PosY, AddBlockOnTop  );
            addTable.Add(NormalFace.PosX, AddBlockRight  );
            addTable.Add(NormalFace.NegX, AddBlockLeft   );
            addTable.Add(NormalFace.PosZ, AddBlockForward);
            addTable.Add(NormalFace.NegZ, AddBlockBack   );
        }

        private void Update()
        {
            //if(!enable) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                Vector3Int _hitNodePos = GetHitNodePosition();
                NormalFace _normalFace = GetHitNormalFace();
                
                Block      _newBlock   = AssetManager.AssetCollection.DefualtBlock;
                AddBlock(_newBlock, _hitNodePos, _normalFace);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3Int _hitNodePos = GetHitNodePosition();
                RemoveBlock(_hitNodePos);
            }
        }

        private NormalFace GetHitNormalFace()
        {
            Vector3    _hitNormal  = gridBlockSelection.GetHitNormal();
            NormalFace _normalFace = BlockFaceUtil.GetSelectedFace(_hitNormal);

            return _normalFace;
        }

        private Vector3Int GetHitNodePosition()
        {
            Block _hitBlock = gridBlockSelection.GetHitObject<Block>();
            if (_hitBlock == null)
                return Vector3Int.zero;
             
            return _hitBlock.NodePosition;
        }
            
        private void AddBlock(Block _blockPrefab, Vector3Int _nodePos, NormalFace _normalFace)
        {
            if(addTable.ContainsKey(_normalFace))
                addTable[_normalFace].Invoke(_blockPrefab, _nodePos);
        }
        
        private void AddBlockOnTop(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.up;
            blockManager.AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockLeft(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.left;
            blockManager.AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockRight(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.right;
            blockManager.AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockForward(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.forward;
            blockManager.AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockBack(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.back;
            blockManager.AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void RemoveBlock(Vector3Int _nodePos)
        {
            blockManager.RemoveBlock(_nodePos);

            List<IBlock> _blocks = blockManager.GetIBlocks3x3Cube(_nodePos);

            Block[] _checkedBlocks = _blocks.Select(_block => _block as Block).Where(_block => _block != null).ToArray();
            
            foreach (var _block in _checkedBlocks)
            {
                var _checkedBlock = _block as Block;
                if (_checkedBlock != null)
                {
                    _checkedBlock.GetSurroundingIBlocksReference();
                    _checkedBlock.SetBitMask();
                }
            }
            
            blockManager.UpdateMeshBlocks(_checkedBlocks);
        }
    }
}
