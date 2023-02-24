using System.Collections;
using System.Collections.Generic;
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
        
       

        public void CopyData(ModulePrototype _modulePrototype)
        {
            Name = _modulePrototype.Name;

            BitMask = _modulePrototype.BitMask;

            MeshPrototype = _modulePrototype.MeshPrototype;
            RotationIndex = _modulePrototype.RotationIndex;
          
        }
    }
}
