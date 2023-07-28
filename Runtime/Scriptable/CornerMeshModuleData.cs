using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CreateAssetMenu(fileName = "CornerMeshData", menuName = "ScriptableObjects/MugCup BlockBuilder/CornerMeshDataObject", order = 7)]
    public class CornerMeshModuleData : ScriptableObject, ISerializationCallbackReceiver
    {
        public Module[] Modules;

        public void SetAllModuleProbability(float _value)
        {
            foreach (var _module in Modules)
            {
                _module.SetProbability(_value);
                SetDirtyModule(_module);
            }
        }

        public void CalculateAllPLogP()
        {
            foreach (var _module in Modules)
            {
                _module.CalculatePLogP();
                SetDirtyModule(_module);
            }
        }
        
        public void CleanData()
        {
            var _validModuleCount = Modules.Count(_module => _module != null);

            var _newData = new Module[_validModuleCount];

            var _index = 0;
            
            foreach (var _module in Modules)
            {
                if (_module == null)
                    continue;

                _newData[_index] = _module;
                
                _index++;
            }

            Modules = _newData;
            
            ResetDataIndex();
        }
        
        private void ResetDataIndex()
        {
            for (var _i = 0; _i < Modules.Length; _i++)
            {
                Modules[_i].Index = _i;
                SetDirtyModule(Modules[_i]);
            }
        }
        
        public void StoreModulesPossibleNeighbors()
        {
            var _modulesInScene = Modules.Where(_module => _module != null).ToList();
            
            foreach (var _module in _modulesInScene)
            {
                _module.StorePossibleNeighbors(_modulesInScene);
                SetDirtyModule(_module);
            }
        }
        
        public void OnBeforeSerialize()
        {
        }
        
        public void OnAfterDeserialize()
        {
        }

        private void SetDirtyModule(Module _module)
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(_module);
            #endif
        }
    }
}
