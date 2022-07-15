using System.Linq;
using UnityEngine;
using BlockBuilder.Scriptable;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder.Runtime
{
    [System.Serializable]
    public class Block: NodeBase
    {
        //Should Has Reference to the Grid and Map that this Block reside//
        [SerializeField] protected BlockManager blockManager;
   
        [SerializeField] protected NodeBase[] gridNodeBases;
        
        [SerializeField] protected GridDataSettingSO  gridData;
        
        [SerializeField] protected MeshFilter mesh;
        
        //public Vector3 WorldPosition;
        
        public int GridPosX => NodePosition.x;
        public int GridPosY => NodePosition.y;
        public int GridPosZ => NodePosition.z;

        public int BitMask              = 0b_000000000_000000000_000000000;
        public int BitMaskMiddleSection = 0b_000000000_000000000_000000000;

        public Block[] TopBlocks    = new Block[9];
        public Block[] MiddleBlocks = new Block[9];
        public Block[] BottomBlocks = new Block[9];
        
        public VolumePoint[] VolumePoints = new VolumePoint[8];
        
        public bool IsEnable;

        [SerializeField] private bool isInit;

        private void Awake()
        {
        }

        public virtual void Init()
        {
            mesh = transform.GetComponent<MeshFilter>();
            
            tag  = "Block";
            name = $"Block: ({NodePosition.x}, {NodePosition.y}, {NodePosition.z})";
        }

        public virtual void Init(Vector3 _worldPos, Vector3Int _gridPos)
        {
            mesh = transform.GetComponent<MeshFilter>();
            
            //This might already set by NodeBase
            //NodeWorldPosition = _worldPos;
            SetNodeWorldPosition(_worldPos);
            NodePosition      = _gridPos;
            
            tag  = "Block";
            name = $"Block: ({_gridPos.x}, {_gridPos.y}, {_gridPos.z})";
        }

        public virtual void InjectDependency(BlockManager _blockManager)
        {
            blockManager  = _blockManager;
            gridNodeBases = _blockManager.GetCurrentGridBlockDataManager().GetGridUnitNodeBases;
            gridData      = _blockManager.GetCurrentGridBlockDataManager().GetGridDataSetting();
        }

        public virtual void UpdateBlockData()
        {
            UpdateNodePosition();
            GetSurroundingBlocksReference();
            SetBitMask();
        }

        public virtual void UpdateNodePosition()
        {
            NodePosition = Utilities.CastVec3ToVec3Int(NodeWorldPosition);
        }

        public virtual void GetSurroundingBlocksReference()
        {
            if(!IsGridDataInit()) return;

            var _gridUnitBlocks = blockManager.GetCurrentGridBlockDataManager().GetGridUnitArray<Block>();
            
            TopBlocks    = GridUtility.GetTopSectionNodesFrom3x3Cube   (NodePosition, gridData.GridUnitSize, _gridUnitBlocks).ToArray();
            MiddleBlocks = GridUtility.GetMiddleSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _gridUnitBlocks).ToArray();
            BottomBlocks = GridUtility.GetBottomSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, _gridUnitBlocks).ToArray();
        }

        public virtual void SetBitMask()
        {
            BitMask = 0b_000000000_000000000_000000000;
            
            int _startBit = 0b_100000000_000000000_000000000;

            foreach (var _block in TopBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in MiddleBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in BottomBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
        }
        
        public int GetBitMaskMiddleSection()
        {
            BitMaskMiddleSection = (BitMask >> 9) & 0b_000000000_000000000_111111111;

            return BitMaskMiddleSection;
        }

#region For Future WFC Function
        /// <summary>
        /// For Marching Points [Not using right now]
        /// </summary>
        /// <param name="_volumePoints"></param>
        public void SetVolumePoints(VolumePoint[] _volumePoints)
        {
            VolumePoints = _volumePoints;
        }
        
        public void SetMesh(Mesh _mesh)
        {
            mesh.mesh = _mesh;
        }
        
        public void Enable()
        {
            IsEnable = true;

            foreach (var _point in VolumePoints)
            {
                _point.SetBitMask   ();
                _point.SetCornerMesh();
            }
        }
        public void Disable()
        {
            IsEnable = false;
            
            foreach (var _point in VolumePoints)
            {
                _point.SetBitMask   ();
                _point.SetCornerMesh();
            }
        }
#endregion
        
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
