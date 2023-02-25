using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class HorizontalFaceDetails : IFaceDetails, IEquatable<HorizontalFaceDetails>
    {
        [field: SerializeField] public short FaceBit { get; private set; }
        
        [field: SerializeField] public bool Walkable { get; private set; }
        [field: SerializeField] public int Connector { get; private set; }
        
        [field: SerializeField] public bool Symmetric;
        [field: SerializeField] public bool Flipped;

        public HorizontalFaceDetails()
        {
            
        }

        public HorizontalFaceDetails(HorizontalFaceDetails _face)
        {
            Walkable  = _face.Walkable;
            Connector = _face.Connector;
            Symmetric = _face.Symmetric;
            Flipped   = _face.Flipped;
        }

        public void SetBit(short _bit)
        {
            FaceBit = _bit;
        }

        public void ResetConnector()
        {
            Connector = 0;
            Symmetric = false;
            Flipped   = false;
        }

        public bool Equals(HorizontalFaceDetails _other)
        {
            if (_other == null)
                return false;
           
            return Connector == _other.Connector && (Symmetric || Flipped != _other.Flipped);
        }
        
        public override string ToString() 
        {
            return Connector + (Symmetric ? "s" : Flipped ? "F" : "");
        }
    }
}
