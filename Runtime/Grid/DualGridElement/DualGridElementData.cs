using System.Collections;
using System.Collections.Generic;
using MugCup_PathFinder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class DualGridElementData : GridData<DualGridElement>
    {
        public override void Initialized()
        {
            var _index = 0;
            
            foreach (var _slot in GetModuleSlots())
            {
                var _nodePos = GridNodes[_index].NodeGridPosition;
                _slot.SetNodePosition(_nodePos);
                _index++;
            }

            _index = 0;
            
            foreach (var _point in GetVolumePoints())
            {
                var _nodePos = GridNodes[_index].NodeGridPosition;
                _point.SetNodePosition(_nodePos);
                _index++;
            }
        }
        
        public IEnumerable<ModuleSlot> GetModuleSlots()
        {
            return GetNodes<ModuleSlot>();
        }

        public IEnumerable<VolumePoint> GetVolumePoints()
        {
            return GetNodes<VolumePoint>();
        }

        public IEnumerable<T> GetNodes<T>() where T : Component, IGridCoord
        {
            foreach (var _node in GridNodes)
            {
                if (_node.TryGetComponent<T>(out var _castNode))
                {
                    yield return _castNode;
                }
            }
        }
    }
}
