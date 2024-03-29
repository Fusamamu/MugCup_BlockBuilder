using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public interface IFaceDetails
    {
	    public short FaceBit { get; }
	    
	    public bool Walkable { get; }
        
        public int Connector { get; }

        public void ResetConnector();
     
       // public ModulePrototype[] ExcludedNeighbours;

       // public bool EnforceWalkableNeighbor { get; }
       // public bool IsOcclusionPortal       { get; }

       public void SetBit(short _bit);
    }
}
