using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class VerticalFaceDetails : IFaceDetails, IEquatable<VerticalFaceDetails>
    {
        [field: SerializeField] public short FaceBit { get; private set; }
        
        [field: SerializeField] public bool Walkable { get; private set; }
        [field: SerializeField] public int Connector { get; private set; }
        
        [field: SerializeField] public bool Invariant { get; private set; }
        
        [field: Range(0, 3),SerializeField] public int Rotation  { get; private set; }
        
        public void SetBit(short _bit)
        {
            FaceBit = _bit;
        }

        public void ResetConnector()
        {
            Connector = 0;
            Rotation  = 0;
            Invariant = false;
        }

        public void RotateDetailsCounterClockwise()
        {
            Rotation++;
            if (Rotation == 4)
                Rotation = 0;
        }

        public bool Equals(VerticalFaceDetails _other)
        {
            if (_other == null) 
                return false;

            return Connector == _other.Connector && Invariant;
        }
        
        public override string ToString() 
        {
            //return Connector + (Invariant ? "i" : Rotation != 0 ? "ABCD".ElementAt(Rotation).ToString() : "");
            
            return Connector + (Invariant ? "i" : "ABCD".ElementAt(Rotation).ToString());
        }
    }
}

