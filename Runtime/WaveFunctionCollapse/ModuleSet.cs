using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class ModuleSet : ICollection<Module>
    {
        [SerializeField] private long[] Data;
        
        public int  Count      { get; }
        public bool IsReadOnly { get; }
        
        private const int bitsPerItem = 64;
        
        public void Add(Module _module)
        {
            if (_module == null)
            {
                return;
            }
            
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
                        //yield return ModuleData.Current[_index];
                        yield break;
                    }
                    
                    _index++;
                    
                    // if (_index >= ModuleData.Current.Length) 
                    // {
                    //     yield break;
                    // }
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
        
    }
}
