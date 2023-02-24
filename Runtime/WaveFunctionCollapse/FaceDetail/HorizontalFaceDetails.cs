using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class HorizontalFaceDetails : IFaceDetails
    {
        [field: SerializeField] public bool Walkable { get; private set; }
        [field: SerializeField] public int Connector { get; private set; }
        
        [field: SerializeField] public bool Symmetric;
        [field: SerializeField] public bool Flipped;

        public void ResetConnector()
        {
            Connector = 0;
            Symmetric = false;
            Flipped   = false;
        }
        
        public override string ToString() 
        {
            return Connector + (Symmetric ? "s" : Flipped ? "F" : "");
        }
    }
}
