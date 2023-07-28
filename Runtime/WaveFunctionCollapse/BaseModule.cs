using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class BaseModule
    {
        public string Name;
        
        public int BitMask ;
        public int MetaData;
        
        public GameObject Prefab;
        public MeshFilter MeshFilterPrototype;
    }
}
