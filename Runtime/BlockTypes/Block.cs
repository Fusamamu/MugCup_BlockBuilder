using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BlockBuilder.Scriptable;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder.Runtime
{
    [System.Serializable]
    public class Block: GridNode
    {
        //Should Has Reference to the Grid and Map that this Block reside//
        [SerializeField] protected BlockManager blockManager;
        [SerializeField] protected GridDataSettingSO  gridData;
        [SerializeField] protected GridNode[] gridNodeBases;
        
#region Will Remove to other component
                [Header("Block Meshes Setting")]
                [SerializeField] protected int currentMeshIndex;
                [SerializeField] protected MeshFilter mesh;
                [SerializeField] protected List<Mesh> meshVariants = new List<Mesh>();
                
                public void ChangeMeshVariant()
                {
                    if(meshVariants.Count == 0) return;
                    
                    currentMeshIndex++;
                    
                    if (currentMeshIndex > meshVariants.Count - 1)
                        currentMeshIndex = 0;
                    
                    mesh.sharedMesh = meshVariants[currentMeshIndex];
                }
#endregion

        [Header("Bit Mask")]
        public int BitMask                       = 0b_000000000_000000000_000000000;
        public int BitMaskMiddleSection          = 0b_000000000_000000000_000000000;
        
        [Header("Bit Mask For Composite")]
        public int BitMaskComposite              = 0b_000000000_000000000_000000000;
        public int BitMaskCompositeMiddleSection = 0b_000000000_000000000_000000000;
        
        [Header("Neighbor Blocks References")]
        public Block[] TopBlocks    = new Block[9];
        public Block[] MiddleBlocks = new Block[9];
        public Block[] BottomBlocks = new Block[9];
      
#if UNITY_EDITOR
        /// In case of losing node position data. Use this method to reinit
        public void InitNodePosition()
        {
            SetNodeWorldPosition(transform.position);
            SetNodePosition(Utilities.CastVec3ToVec3Int(NodeWorldPosition));
        }
#endif

        public virtual Block SetPosition(Vector3 _worldPos, Vector3Int _gridPos)
        {
            SetNodeWorldPosition(_worldPos);
            SetNodePosition     (_gridPos);
            return this;
        }
        
        public virtual Block InjectDependency(BlockManager _blockManager)
        {
            blockManager  = _blockManager;
            gridNodeBases = _blockManager.GridBlockDataManager.GridNodeData.GridNodes;
            gridData      = _blockManager.GridBlockDataManager.GridDataSetting;
            
            return this;
        }
        
        public virtual Block Init()
        {
            mesh = transform.GetComponent<MeshFilter>();
            
            tag  = "Block";
            name = $"Block: ({NodeGridPosition.x}, {NodeGridPosition.y}, {NodeGridPosition.z})";

            return this;
        }

        public virtual void UpdateBlockData()
        {
            UpdateNodePosition();
            GetSurroundingBlocksReference();
            SetBitMask();
        }

        public virtual void UpdateNodePosition()
        {
            var _nodePosition = Utilities.CastVec3ToVec3Int(NodeWorldPosition);
            SetNodePosition(_nodePosition);
        }

        public virtual void GetSurroundingBlocksReference()
        {
            if(!IsGridDataInit()) return;

            var _gridUnitBlocks = blockManager.GridBlockDataManager.GridNodeData.GetGridUnitArray<Block>();
            
            TopBlocks    = GridUtility.GetTopSectionNodesFrom3x3Cube   (NodeGridPosition, gridData.GridUnitSize, _gridUnitBlocks).ToArray();
            MiddleBlocks = GridUtility.GetMiddleSectionNodesFrom3x3Cube(NodeGridPosition, gridData.GridUnitSize, _gridUnitBlocks).ToArray();
            BottomBlocks = GridUtility.GetBottomSectionNodesFrom3x3Cube(NodeGridPosition, gridData.GridUnitSize, _gridUnitBlocks).ToArray();
        }

        public virtual void SetBitMask()
        {
            BitMask = GetBitMask();
        }

        public virtual int GetBitMask()
        {
            var _bitMask  = 0b_000000000_000000000_000000000;
            int _startBit = 0b_100000000_000000000_000000000;

            foreach (var _block in TopBlocks)
            {
                if (_block != null)
                    _bitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in MiddleBlocks)
            {
                if (_block != null)
                    _bitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in BottomBlocks)
            {
                if (_block != null)
                    _bitMask |= _startBit;
                
                _startBit >>= 1;
            }

            return _bitMask;
        }
        
        public int GetBitMaskMiddleSection()
        {
            BitMaskMiddleSection = (BitMask >> 9) & 0b_000000000_000000000_111111111;
            return BitMaskMiddleSection;
        }

        public int GetBitMaskCompositeMiddleSection()
        {
            BitMaskCompositeMiddleSection = (BitMaskComposite >> 9) & 0b_000000000_000000000_111111111;
            return BitMaskCompositeMiddleSection;
        }
        
        protected bool IsGridDataInit()
        {
            if (gridData == null)
            {
                Debug.Log($"<color=red>[Warning] : </color>GridDataSetting is missing");
                return false;
            }

            if (gridNodeBases == null)
            {
                Debug.Log($"<color=red>[Warning] : </color>GridUnitBlocksRef is missing");
                return false;
            }

            return true;
        }
    }
}
