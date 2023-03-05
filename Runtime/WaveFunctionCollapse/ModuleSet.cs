using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class ModuleSet : ICollection<Module>
    {
        public float Entropy 
        {
            get
            {
                return CalculateEntropy();
            }
        }
        
        [SerializeField] private long[] Data;
        
        public int  Count      { get; }
        public bool IsReadOnly { get; }

        private float entropy;
        private const int bitsPerItem = 64;

        public ModuleSet(int _allModuleCount, bool _initializedFull = false) 
        {
            Data = new long[_allModuleCount / bitsPerItem + (_allModuleCount % bitsPerItem == 0 ? 0 : 1)];
            
            if (_initializedFull) 
            {
                for (var _i = 0; _i < Data.Length; _i++) {
                    Data[_i] = ~0;
                }
            }
        }
        
        public ModuleSet(ModuleSet _source, Module _toRemoveModule = null)
        {
            Data = _source.Data.ToArray();
            //entropy = source.Entropy;

            if(_toRemoveModule)
                Remove(_toRemoveModule);
        }
        
        public bool ContainModule(Module _module)
        {
            return Contains(_module.Index);
        }
        
        public bool Contains(int _index) 
        {
            int _i = _index / bitsPerItem;
            
            long _mask = (long)1 << (_index % bitsPerItem);
            
            return (Data[_i] & _mask) != 0;
        }

        public void Intersect(ModuleSet _moduleSet)
        {
            for (var _i = 0; _i < Data.Length; _i++)
            {
                long _current = Data[_i];
                long _mask    = _moduleSet.Data[_i];
                long _updated = _current & _mask;

                if (_current != _updated)
                    Data[_i] = _updated;
            }
        }
        
#region Add/Remove Modules
        public void Add(Module _module)
        {
            if (_module == null)
                return;
            
            int _i = _module.Index / bitsPerItem;
            
            long _mask  = (long)1 << (_module.Index % bitsPerItem);
            long _value = Data[_i];
	
            if ((_value & _mask) == 0) 
            {
                Data[_i] = _value | _mask;
            }
        }
        
        public bool Remove(Module _module)
        {
            if (_module == null)
                return false;
            
            int _i = _module.Index / bitsPerItem;
            
            long _mask  = (long)1 << (_module.Index % bitsPerItem);
            long _value = Data[_i];
	
            if ((_value & _mask) != 0) 
            {
                Data[_i] = _value & ~_mask;
                return true;
            } 
            return false;
        }
        
        public void Add(ModuleSet _moduleSet) 
        {
            for (var _i = 0; _i < Data.Length; _i++) 
            {
                long _current = Data[_i];
                long _updated = _current | _moduleSet.Data[_i];
            
                if (_current != _updated) 
                {
                    Data[_i] = _updated;
                }
            }
        }

        public void Remove(ModuleSet _moduleSet) 
        {
            for (var _i = 0; _i < Data.Length; _i++) 
            {
                long _current = Data[_i];
                long _updated = _current & ~_moduleSet.Data[_i];
            
                if (_current != _updated) 
                {
                    Data[_i] = _updated;
                }
            }
        }
#endregion
        
        public IEnumerator<Module> GetEnumerator()
        {
            var _index = 0;

            foreach (var _value in Data)
            {
                if (_value == 0) 
                {
                    _index += bitsPerItem;
                    continue;
                }
                
                for (var _j = 0; _j < bitsPerItem; _j++) 
                {
                    if ((_value & ((long)1 << _j)) != 0)
                    {
                        yield return ModuleData.Data.Modules[_index];
                    }
                    
                    _index++;
                    
                    if (_index >= ModuleData.Data.Modules.Length) 
                        yield break;
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public void Clear()
        {
        }
        
        public bool Contains(Module _item)
        {
            return false;
        }
        
        public void CopyTo(Module[] _array, int _arrayIndex)
        {
        }

        private float CalculateEntropy()
        {
            float _total      = 0;
            float _entropySum = 0;
                
            foreach (var _module in this) 
            {
                _total      += _module.Probability;
                _entropySum += _module.PLogP;
            }
                
            return -1f / _total * _entropySum + Mathf.Log(_total);
        }
    }
}
