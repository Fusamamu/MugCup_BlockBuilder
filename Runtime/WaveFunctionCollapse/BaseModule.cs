using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    //Pipeline BaseModule => ModulePrototype => Module
    [Serializable]
    public class BaseModule
    {
        public string Name;
        
        public int BitMask ;
        public int ModuleID;
        
        public string MetaData;
        
        public GameObject Prefab;
        public MeshFilter MeshFilterPrototype;

        public bool HasRotateVariant;
        public bool HasFlipXVariant;

        public void UpdateData()
        {
            UpdateName();
            UpdateBitMask();
            UpdateModuleID();
        }

        public void UpdateName()
        {
            Name = MetaDataUtil.GetMetaDataName(MetaData);
        }

        public void UpdateBitMask()
        {
            var _stringBinary = MetaDataUtil.TurnMetaDataIntoBinaryStringFormat(MetaData);
            BitMask = MetaDataUtil.BinaryStringToInt(_stringBinary);
        }

        public void UpdateModuleID()
        {
        }
    }
}
