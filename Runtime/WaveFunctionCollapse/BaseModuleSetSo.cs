using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CreateAssetMenu(fileName = "BaseModuleSetData", menuName = "ScriptableObjects/MugCup BlockBuilder/Base Module Set Data", order = 7)]
    public class BaseModuleSetSo : ScriptableObject
    {
        public List<BaseModule> BaseModules_1111_0000 = new List<BaseModule>();
        
        public List<BaseModule> BaseModules = new List<BaseModule>();

        public void UpdateNames()
        {
            foreach (var _module in BaseModules)
            {
                _module.Name = _module.MeshFilterPrototype.name;
            }
        }
    }
}
