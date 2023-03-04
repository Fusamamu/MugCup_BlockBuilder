using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MugCup_PathFinder.Runtime;
using Unity.Collections;

namespace MugCup_BlockBuilder
{
    public class ModuleSlot : MonoBehaviour, IGridCoord
    {
        [field: SerializeField] public Module CollapsedModule { get; private set; }
        
        [field: ReadOnly, SerializeField] 
        public ModuleSlotData ModuleSlotData { get; private set; }
        
        public bool IsCollapsed => CollapsedModule != null;

        public ModuleSet AvailableModuleSet;
        
        public short[][] ModuleHealth;

        public ModuleSlot SetModuleSlotData(ModuleSlotData _moduleSlotData)
        {
            ModuleSlotData = _moduleSlotData;
            return this;
        }
        
        public void Collapse(Module _module) 
        {
            if (IsCollapsed) 
            {
                Debug.LogWarning("Trying to collapse already collapsed slot.");
                return;
            }

            CollapsedModule = _module;
            
            RemoveModules(new ModuleSet(AvailableModuleSet, _toRemoveModule: _module));
        }
        
        public void CollapseRandom() 
        {
            if (!AvailableModuleSet.Any()) 
            {
               // throw new CollapseFailedException(this);
            }
            
            if (IsCollapsed) 
            {
                throw new Exception("Slot is already collapsed.");
            }
		
            // float max = this.Modules.Select(module => module.Prototype.Probability).Sum();
            // float roll = (float)(InfiniteMap.Random.NextDouble() * max);
            // float p = 0;
            // foreach (var candidate in this.Modules) {
            //     p += candidate.Prototype.Probability;
            //     if (p >= roll) {
            //         this.Collapse(candidate);
            //         return;
            //     }
            // } 
            
            Collapse(AvailableModuleSet.First());
        }

        private void RemoveModules(ModuleSet _toRemoveModules)
        {
            foreach(ModuleFace _face in Enum.GetValues(typeof(ModuleFace)))
            {
                var _neighbor = ModuleSlotData.GetNeighbor(NodeGridPosition, _face);
                
                if(_neighbor == null)
                    continue;
                
                var _faceIndex         = (int)_face;
                var _oppositeFaceIndex = (int)Orientations.GetOppositeFace(_face);
                
                foreach (var _module in _toRemoveModules)
                {
                    var _possibleNeighbors = _module.PossibleNeighborsArray[_faceIndex];

                    foreach (var _possibleModule in _possibleNeighbors)
                    {
                        _neighbor.ModuleHealth[_oppositeFaceIndex][_possibleModule.Index]--;
                    }
                }
            }

            AvailableModuleSet.Remove(_toRemoveModules);
        }
        
        
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
