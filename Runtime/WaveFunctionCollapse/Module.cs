using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    //Naming more like Module??
    [CreateAssetMenu(fileName = "PrototypeData", menuName = "ScriptableObjects/PrototypeDataObject", order = 8)]
    public class Module : ScriptableObject
    {
        public int Index;
        
        public string Name;

        public int BitMask;
        
        public Mesh MeshPrototype;

        public int RotationIndex;
        
        public HorizontalFaceDetails Forward;
        public HorizontalFaceDetails Back;
        public HorizontalFaceDetails Left;
        public HorizontalFaceDetails Right;
        public VerticalFaceDetails Up;
        public VerticalFaceDetails Down;
        
        public ModuleSet[] PossibleNeighbors;

        public void CopyData(ModulePrototype _modulePrototype)
        {
            Index         = _modulePrototype.Index;
            Name          = _modulePrototype.Name;
            BitMask       = _modulePrototype.BitMask;
            MeshPrototype = _modulePrototype.MeshPrototype;
            RotationIndex = _modulePrototype.RotationIndex;

            Forward = _modulePrototype.Forward;
            Back    = _modulePrototype.Back;
            Left    = _modulePrototype.Left;
            Right   = _modulePrototype.Right;
            Up      = _modulePrototype.Up;
            Down    = _modulePrototype.Down;
        }

        public void StorePossibleNeighbors(List<Module> _allModuleInScene)
        {
            var _allModuleCount = _allModuleInScene.Count;
            
            PossibleNeighbors = new ModuleSet[6];
            
            for (var _dir = 0; _dir < 6; _dir++)
            {
                var _newModuleSet = new ModuleSet(_allModuleCount);

                foreach (var _module in _allModuleInScene)
                {
                    if (_module.IsFit(this))
                        _newModuleSet.Add(_module);
                }

                PossibleNeighbors[_dir] = _newModuleSet;
            }
        }

        public bool IsFit(Module _otherModule)
        {
            return false;
        }
    }
}
