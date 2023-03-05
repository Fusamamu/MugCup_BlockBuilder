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
        [field: ReadOnly, SerializeField] public ModuleSlotData ModuleSlotData { get; private set; }
        
        [field: SerializeField] public Module CollapsedModule { get; private set; }
        
        public bool IsCollapsed => CollapsedModule != null;

        public ModuleSet AvailableModuleSet;
        
        public short[][] ModuleHealth;

        private static readonly System.Random random = new System.Random();

        public ModuleSlot SetModuleSlotData(ModuleSlotData _moduleSlotData)
        {
            ModuleSlotData = _moduleSlotData;
            return this;
        }
        
        public void CollapseRandom() 
        {
            if (!AvailableModuleSet.Any()) 
            {
               return;
            }
            
            if (IsCollapsed) 
            {
                throw new Exception("Slot is already collapsed.");
            }
		
            float _max  = AvailableModuleSet.Select(_module => _module.Probability).Sum();
            float _roll = (float)(random.NextDouble() * _max);
            
            float _p = 0;
            foreach (var _candidate in AvailableModuleSet) 
            {
                _p += _candidate.Probability;
                if (_p >= _roll) 
                {
                    Collapse(_candidate);
                    return;
                }
            } 
            
            Collapse(AvailableModuleSet.First());
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
            
            ModuleSlotData.NotifySlotCollapsed(this);
        }

        public void RemoveModules(ModuleSet _toRemoveModules, bool _recursive = true)
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
                        if (_neighbor.ModuleHealth[_oppositeFaceIndex][_possibleModule.Index] == 1 && _neighbor.AvailableModuleSet.Contains(_possibleModule)) 
                        {
                            ModuleSlotData.RemovalQueue[_neighbor.NodeGridPosition].Add(_possibleModule);
                        }
                        
                        _neighbor.ModuleHealth[_oppositeFaceIndex][_possibleModule.Index]--;
                    }
                }
            }

            AvailableModuleSet.Remove(_toRemoveModules);

            if (_recursive)
                ModuleSlotData.FinishRemovalQueue();
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
