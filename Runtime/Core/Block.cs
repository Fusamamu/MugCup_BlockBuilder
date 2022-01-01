using System.Linq;
using UnityEngine;
using BlockBuilder.Scriptable;
using BlockBuilder.Runtime.Core;
using MugCup_PathFinder.Runtime;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace MugCup_BlockBuilder.Runtime.Core
{
    [System.Serializable]
    public class Block: MonoBehaviour, IBlock, INode
    {
        //Should Has Reference to the Grid and Map that this Block reside//
        private static IBlock[] gridBlocks;
        private static GridDataSettingSO  gridData;
        
        private MeshFilter mesh;
        
#region INode Implementation
        public INode      NodeParent   { get; set; }
        public Vector3Int NodePosition { get; set; }
        
        public int G_Cost { get; set; }
        public int H_Cost { get; set; }
        public int F_Cost => G_Cost + H_Cost; 
#endregion
        
        public Vector3 WorldPosition;
        
        public int GridPosX => NodePosition.x;
        public int GridPosY => NodePosition.y;
        public int GridPosZ => NodePosition.z;

        public int BitMask = 0b_000000000_000000000_000000000;

        public IBlock[] TopIBlocks    = new IBlock[9];
        public IBlock[] MiddleIBlocks = new IBlock[9];
        public IBlock[] BottomIBlocks = new IBlock[9];

        private void Awake()
        {
            gridBlocks ??= GridBlockData.GetBlocks();
            gridData   ??= GridBlockData.GetGridDataSetting();
        }

        public void Init(Vector3 _worldPos, Vector3Int _gridPos)
        {
            mesh = transform.GetComponent<MeshFilter>();
            
            WorldPosition = _worldPos;
            NodePosition  = _gridPos;
            
            tag  = "Block";
            name = $"Block: ({_gridPos.x}, {_gridPos.y}, {_gridPos.z})";
            
            gameObject.layer = LayerMask.NameToLayer("Block");
        }

        public void UpdateBlockData()
        {
            GetSurroundingIBlocksReference();
            SetBitMask();
        }

        public void GetSurroundingIBlocksReference()
        {
            if(!IsGridDataInit()) return;
            
            TopIBlocks    = GridUtility.GetTopSectionNodesFrom3x3Cube   (NodePosition, gridData.GridUnitSize, gridBlocks).ToArray();
            MiddleIBlocks = GridUtility.GetMiddleSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, gridBlocks).ToArray();
            BottomIBlocks = GridUtility.GetBottomSectionNodesFrom3x3Cube(NodePosition, gridData.GridUnitSize, gridBlocks).ToArray();
        }

        public void SetMesh(Mesh _mesh)
        {
            mesh.mesh = _mesh;
        }

        public void SetBitMask()
        {
            BitMask = 0b_000000000_000000000_000000000;
            
            int _startBit = 0b_100000000_000000000_000000000;

            foreach (var _block in TopIBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in MiddleIBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
            
            foreach (var _block in BottomIBlocks)
            {
                if (_block != null)
                    BitMask |= _startBit;
                
                _startBit >>= 1;
            }
        }
        
        private bool IsGridDataInit()
        {
            if (gridData == null)
            {
                Debug.Log($"<color=red>[Warning] : </color>GridDataSetting is missing");
                return false;
            }

            if (gridBlocks == null)
            {
                Debug.Log($"<color=red>[Warning] : </color>GridUnitBlocksRef is missing");
                return false;
            }

            return true;
        }
    }
}
