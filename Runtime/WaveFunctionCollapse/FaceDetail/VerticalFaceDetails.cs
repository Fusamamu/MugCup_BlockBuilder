using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class VerticalFaceDetails : IFaceDetails
    {
        [field: SerializeField] public bool Walkable { get; private set; }
        [field: SerializeField] public int Connector { get; private set; }
        
        [field: SerializeField] public bool Invariant {  get; private set; }
        [field: SerializeField] public int  Rotation  { get; private set; }

        public void ResetConnector()
        {
            Connector = 0;
            Rotation  = 0;
            Invariant = false;
        }
        
        public override string ToString() 
        {
            return Connector + (Invariant ? "i" : Rotation != 0 ? "_bcd".ElementAt(Rotation).ToString() : "");
        }
    }
}

