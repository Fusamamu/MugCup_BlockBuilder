using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder
{
    public class ModuleSlot : MonoBehaviour, IGridCoord
    {
        [field: SerializeField] public Vector3Int NodeGridPosition  { get; private set; }
        [field: SerializeField] public Vector3    NodeWorldPosition { get; private set; }
        
        public IGridCoord SetNodePosition(Vector3Int _nodePosition)
        {
            NodeGridPosition = _nodePosition;
            return this;
        }
        
        public IGridCoord SetNodeWorldPosition(Vector3 _worldPosition)
        {
            NodeWorldPosition = _worldPosition;
            return this;
        }
    }
}
