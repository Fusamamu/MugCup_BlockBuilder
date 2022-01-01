using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBuilder.Scriptable
{
    [CreateAssetMenu(fileName = "MaterialData", menuName = "ScriptableObjects/MaterialDataObject", order = 3)]
    public class MaterialData : ScriptableObject
    {
        public Material DefaultBlockMaterial;
        public Material VisualizerPointerMaterial;
    }
    
}
