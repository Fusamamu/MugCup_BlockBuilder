using System.Collections;
using System.Collections.Generic;
using MugCup_PathFinder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class DualGridElement : MonoBehaviour, IGridCoord
    {
        [field: SerializeField] public ModuleSlot  ModuleSlot  { get; private set; }
        [field: SerializeField] public VolumePoint VolumePoint { get; private set; }

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
