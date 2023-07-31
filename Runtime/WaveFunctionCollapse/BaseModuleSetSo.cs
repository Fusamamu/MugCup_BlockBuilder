using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CreateAssetMenu(fileName = "BaseModuleSetData", menuName = "ScriptableObjects/MugCup BlockBuilder/Base Module Set Data", order = 7)]
    public class BaseModuleSetSo : ScriptableObject
    {
        public List<BaseModuleGroup> BaseModuleGroups = new();

        public Dictionary<string, List<BaseModule>> GetBaseModuleGroupTable()
        {
            Dictionary<string, List<BaseModule>> _table = new();

            foreach (var _group in BaseModuleGroups)
            {
                if (!_table.ContainsKey(_group.BitMaskGroup))
                    _table.Add(_group.BitMaskGroup, _group.BaseModules);
            }

            return _table;
        }

        public void UpdateData()
        {
            // foreach (var _module in BaseModules)
            //     _module.UpdateData();
        }

        public void GenerateDefaultBaseModuleGroups()
        {
            BaseModuleGroups = new List<BaseModuleGroup>
            {
                new BaseModuleGroup()
                {
                    BitMaskGroup = "11110000",
                    HasRotateVariant = false,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00000001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00010000",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00010001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00100001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = true,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01010000",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00000011",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00110000",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00000111",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01110000",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00010011",
                    HasRotateVariant = true,
                    HasFlipXVariant  = true,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00110001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = true,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00011011",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "10110001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00011111",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "11110001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00111111",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "11110011",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01111111",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "11110111",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "00110011",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01110111",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01110001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = true,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01110011",
                    HasRotateVariant = true,
                    HasFlipXVariant  = true,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "11110101",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01010001",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01010101",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
                new BaseModuleGroup()
                {
                    BitMaskGroup = "01110101",
                    HasRotateVariant = true,
                    HasFlipXVariant  = false,
                },
            };

            foreach (var _group in BaseModuleGroups)
            {
                BaseModule _baseModule = new()
                {
                    MetaData = _group.BitMaskGroup,
                    HasRotateVariant = _group.HasRotateVariant,
                    HasFlipXVariant  =  _group.HasFlipXVariant
                };
                
                _baseModule.UpdateData();
                
                _group.BaseModules.Add(_baseModule);
            }
        }

        public void ClearData()
        {
            BaseModuleGroups = new List<BaseModuleGroup>();
        }
    }

    [Serializable]
    public class BaseModuleGroup
    {
        public string BitMaskGroup;
        
        public bool HasRotateVariant;
        public bool HasFlipXVariant;
        
        public List<BaseModule> BaseModules = new List<BaseModule>();
    }
}
