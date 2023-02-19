using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CreateAssetMenu(fileName = "CornerMeshData", menuName = "ScriptableObjects/Wave Function Collapse/CornerMeshDataObject", order = 7)]
    public class CornerMeshModuleData : ScriptableObject
    {
        public Module[] Modules;
    }
}
