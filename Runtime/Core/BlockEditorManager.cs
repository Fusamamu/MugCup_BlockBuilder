using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using MugCup_BlockBuilder.Runtime.Core.Managers;

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
        
        private readonly Dictionary<NormalFace, Action<Block, Vector3Int>> addTable = new Dictionary<NormalFace, Action<Block, Vector3Int>>();
        
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
            
            addTable.Add(NormalFace.PosY, AddBlockOnTop  );
            addTable.Add(NormalFace.PosX, AddBlockRight  );
            addTable.Add(NormalFace.NegX, AddBlockLeft   );
            addTable.Add(NormalFace.PosZ, AddBlockForward);
            addTable.Add(NormalFace.NegZ, AddBlockBack   );
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

            //Need to refactor this do 2 things// Remove n update blocks
            List<Block> _blocks = blockManager.GetIBlocks3x3Cube(_nodePos);

            Block[] _checkedBlocks = _blocks.Select(_block => _block as Block).Where(_block => _block != null).ToArray();
            
            foreach (var _block in _checkedBlocks)
            {
                var _checkedBlock = _block;
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
