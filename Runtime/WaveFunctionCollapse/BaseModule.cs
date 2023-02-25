using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  MugCup_BlockBuilder
{
    [Serializable]
    public class BaseModuleSet
    {
        public List<BaseModule> BaseModules = new List<BaseModule>();

        public void UpdateNames()
        {
            foreach (var _module in BaseModules)
            {
                _module.Name = _module.MeshFilter.name;
            }
        }
    }
    
    [Serializable]
    public class BaseModule
    {
        public string Name;
        public GameObject Prefab;
        public MeshFilter MeshFilter;
        public FaceDetails FaceDetails;
    }
}
