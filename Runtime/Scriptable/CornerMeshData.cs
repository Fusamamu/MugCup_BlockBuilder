using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core;

namespace BlockBuilder.Core.Scriptable
{
    [CreateAssetMenu(fileName = "CornerMeshData", menuName = "ScriptableObjects/CornerMeshDataObject", order = 7)]
    public class CornerMeshData : ScriptableObject
    {
        public Mesh M_0000_0001;
        public Mesh M_0000_0010;
        public Mesh M_0000_0100;
        public Mesh M_0000_1000;

        public Mesh M_0001_0000;
        public Mesh M_0010_0000;
        public Mesh M_0100_0000;
        public Mesh M_1000_0000;
        
        public Mesh GetCornerMesh(int _bitMask)
        {
            if (_bitMask == 0b_0000_0001)
            {
                return M_0000_0001;
            }

            if (_bitMask == 0b_0000_0010)
            {
                return M_0000_0010;
            }

            if (_bitMask == 0b_0000_0100)
            {
                return M_0000_0100;
            }

            if (_bitMask == 0b_0000_1000)
            {
                return M_0000_1000;
            }

            if (_bitMask == 0b_0001_0000)
            {
                return M_0001_0000;
            }

            if (_bitMask == 0b_0010_0000)
            {
                return M_0010_0000;
            }
            
            if (_bitMask == 0b_0100_0000)
            {
                return M_0100_0000;
            }

            if (_bitMask == 0b_1000_0000)
            {
                return M_1000_0000;
            }

            return null;
        }
    }
}
