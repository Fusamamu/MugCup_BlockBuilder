using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public interface IModule
    {
        public string Name { get; }
        
        public int Index    { get; set; }
        public int BitMask  { get; }
        public int MetaData { get; }
        
        public Mesh MeshPrototype { get; }

        public float Probability { get; }
        public float PLogP       { get; }

        public int RotationIndex { get; }

        public FaceDetails FaceDetails { get; }
    }
}
