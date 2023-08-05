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

        public Dictionary<int, Module> GetBitTable()
        {
            Dictionary<int, Module> _table = new();

            foreach (var _module in Modules)
            {
                var _bitMask = _module.BitMask;
                
                if(!_table.ContainsKey(_bitMask))
                    _table.Add(_bitMask, _module);
            }

            return _table;
        }

        public Dictionary<int, Dictionary<string, Module>> GetMetaTable()
        {
            Dictionary<int, Dictionary<string, Module>> _metaTable = new();

            foreach (var _module in Modules)
            {
                var _bitMask  = _module.BitMask;
                var _metaData = _module.MetaData;

                if (!_metaTable.ContainsKey(_bitMask))
                {
                    Dictionary<string, Module> _metaDict = new() {
                        { _metaData, _module }
                    };
                    
                    _metaTable.Add(_bitMask, _metaDict);
                }
                else
                {
                    if (!_metaTable[_bitMask].ContainsKey(_metaData))
                        _metaTable[_bitMask].Add(_metaData, _module);
                    else
                        Debug.LogWarning($"There are duplicate Meta data! : {_metaData}");
                }
            }

            return _metaTable;
        }

        public Dictionary<string, Module> GetModuleTable()
        {
            Dictionary<string, Module> _table = new();

            return _table;
        }
        
        

        //This is duplicate from CornerMeshData
        public Mesh GetCornerMesh(int _bitMask)
        {
            // var _prototypeData = GetPrototypeData(_bit);
            //
            // if (_prototypeData != null)
            // {
            //     return _prototypeData.MeshPrototype;
            // }
            
            return null;
        }

        public Mesh GetCornerMesh(string _metaData)
        {
            return null;
        }

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
