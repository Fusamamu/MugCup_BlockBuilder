using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [DisallowMultipleComponent]
    public class ModuleData : MonoBehaviour
    {
        public static CornerMeshModuleData Data;

        [SerializeField] private CornerMeshModuleData CornerMeshModuleData;

        private void Awake()
        {
            Initialized();
        }

        public void Initialized()
        {
            Data = CornerMeshModuleData;
        }

        public void StoreCornerMeshModuleData(CornerMeshModuleData _data)
        {
            CornerMeshModuleData = _data;
        }
    }
}
