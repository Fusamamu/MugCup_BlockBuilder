using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder;
using UnityEngine;

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

        public Mesh M_0000_0011;
        public Mesh M_0000_1100;
        public Mesh M_0011_0000;
        public Mesh M_1100_0000;

        public PrototypeData P_0000_0001;
        public PrototypeData P_0000_0010;

        public PrototypeData GetPrototypeData(int _bit)
        {
            if (_bit == BitTable.B_0000_00001) return P_0000_0001;
            
            return null;
        }

        public Mesh GetPrototypeMesh(int _bit)
        {
            return null;
        }
        
        public Mesh GetCornerMesh(int _bitMask)
        {
            if (_bitMask == 0b_0000_0001)
                return M_0000_0001;

            if (_bitMask == 0b_0000_0010)
                return M_0000_0010;

            if (_bitMask == 0b_0000_0100)
                return M_0000_0100;

            if (_bitMask == 0b_0000_1000)
                return M_0000_1000;

            if (_bitMask == 0b_0001_0000)
                return M_0001_0000;

            if (_bitMask == 0b_0010_0000)
                return M_0010_0000;
            
            if (_bitMask == 0b_0100_0000)
                return M_0100_0000;

            if (_bitMask == 0b_1000_0000)
                return M_1000_0000;

            if (_bitMask == 0b_0000_0011)
                return M_0000_0011;

            if (_bitMask == 0b_0000_1100)
                return M_0000_1100;

            if (_bitMask == 0b_0011_0000)
                return M_0011_0000;

            if (_bitMask == 0b_1100_0000)
                return M_1100_0000;

            return null;
        }
    }
}
