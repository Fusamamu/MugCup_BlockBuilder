#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_BlockBuilder.Runtime.Core.Managers;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder.Runtime.Core
{
    public sealed class BlockEditorManager : BaseBuilderManager
    {
#region Dependencies
        private BlockBuilderManager blockBuilderManager;
        private PointerVisualizer   pointerVisualizer;
        
        private IBlockRaycaster     gridBlockSelection;
        private IInputManager       inputManager;
#endregion
        
        private static Dictionary<NormalFace, Action<Block, Vector3Int>> addTable = new Dictionary<NormalFace, Action<Block, Vector3Int>>();
        
        public override void Init()
        {
            base.Init();
            
            blockBuilderManager = BlockBuilderManager.Instance;
            
            pointerVisualizer  = FindObjectOfType<PointerVisualizer> ();
            gridBlockSelection = FindObjectOfType<GridBlockSelection>();
            inputManager       = FindObjectOfType<InputManager>      ();
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            AssetManager.LoadAssets();
            
            InitializeAddTable();
        }

        public void InitializeAddTable()
        {
            if (addTable == null || addTable.Count == 0)
            {
                addTable = new Dictionary<NormalFace, Action<Block, Vector3Int>>();
                
                addTable.Add(NormalFace.PosY, AddBlockOnTop  );
                addTable.Add(NormalFace.PosX, AddBlockRight  );
                addTable.Add(NormalFace.NegX, AddBlockLeft   );
                addTable.Add(NormalFace.PosZ, AddBlockForward);
                addTable.Add(NormalFace.NegZ, AddBlockBack   );
                addTable.Add(NormalFace.None, AddBlockCenter );
            }
        }

        private void Update()
        {
            // if (inputManager.CheckLeftMouseClicked())
            // {
            //     Vector3Int _hitNodePos = GetHitNodePosition();
            //     NormalFace _normalFace = GetHitNormalFace();
            //     
            //     Block      _newBlock   = AssetManager.AssetCollection.DefualtBlock;
            //     AddBlock(_newBlock, _hitNodePos, _normalFace);
            // }
            //
            // if (inputManager.CheckRightMouseClicked())
            // {
            //     Vector3Int _hitNodePos = GetHitNodePosition();
            //     RemoveBlock(_hitNodePos);
            // }
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
             
            return _hitBlock.NodeGridPosition;
        }
            
        public void AddBlock(Block _blockPrefab, Vector3Int _nodePos, NormalFace _normalFace)
        {
            if(addTable.ContainsKey(_normalFace))
                addTable[_normalFace].Invoke(_blockPrefab, _nodePos);
        }

        /// <summary>
        /// Add Block at node center. Called when Normal Face is none.
        /// </summary>
        private void AddBlockCenter(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos;
            GetBlockManager().AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockOnTop(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.up;
            GetBlockManager().AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockLeft(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.left;
            GetBlockManager().AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockRight(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.right;
            GetBlockManager().AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockForward(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.forward;
            GetBlockManager().AddBlock(_blockPrefab, _targetNodePos);
        }
        
        private void AddBlockBack(Block _blockPrefab, Vector3Int _nodePos)
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.back;
            GetBlockManager().AddBlock(_blockPrefab, _targetNodePos);
        }
        
        public void RemoveBlock(Vector3Int _nodePos)
        {
            GetBlockManager().RemoveBlock(_nodePos);

            //Need to refactor this do 2 things// Remove n update blocks
            
            //GetBlockManager().UpdateSurroundBlocksBitMask(_nodePos);
            
            // List<Block> _blocks = blockManager.GetBlocks3x3Cube(_nodePos);
            //
            // Block[] _checkedBlocks = _blocks.Where(_block => _block != null).ToArray();
            //
            // foreach (var _block in _checkedBlocks)
            // {
            //     if (_block != null)
            //     {
            //         _block.GetSurroundingBlocksReference();
            //         _block.SetBitMask();
            //     }
            // }
            
            //blockManager.UpdateMeshBlocks(_checkedBlocks);
        }

        
#region Add/Remove NodeBase Generic
        public T AddNodeCenter<T>(T _node, Vector3Int _nodePos) where T : GridNode
        {
            Vector3Int _targetNodePos = _nodePos;
            return GetBlockManager().AddNodeAt(_node, _targetNodePos);
        }
        
        public T AddNodeOnTop<T>(T _node, Vector3Int _nodePos) where T : GridNode
        {
            Vector3Int _targetNodePos = _nodePos + Vector3Int.up;
            return GetBlockManager().AddNodeAt(_node, _targetNodePos);
        }
#endregion
    }
}
#endif
