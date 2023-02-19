using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    //Naming more like Module??
    [CreateAssetMenu(fileName = "PrototypeData", menuName = "ScriptableObjects/PrototypeDataObject", order = 8)]
    public class Module : ScriptableObject
    {
        public string Name;

        public int BitMask;
        
        public Mesh MeshPrototype;

        public int RotationIndex;
        
        [Header("Socket")]
        public string PosXSocket = "PosX";
        public string NegXSocket = "NegX";
        public string PosZSocket = "PosZ";
        public string NegZSocket = "NegZ";
        public string PosYSocket = "PosY";
        public string NegYSocket = "NegY";

        [Header("Neighbors")]
        public List<string> PosXNeighbors = new List<string>();
        public List<string> NegXNeighbors = new List<string>();
        public List<string> PosZNeighbors = new List<string>();
        public List<string> NegZNeighbors = new List<string>();
        public List<string> PosYNeighbors = new List<string>();
        public List<string> NegYNeighbors = new List<string>();

        public void CopyData(ModulePrototype _modulePrototype)
        {
            Name = _modulePrototype.Name;

            BitMask = _modulePrototype.BitMask;

            MeshPrototype = _modulePrototype.MeshPrototype;
            RotationIndex = _modulePrototype.RotationIndex;

            PosXSocket = _modulePrototype.PosXSocket;
            NegXSocket = _modulePrototype.NegXSocket;
            PosZSocket = _modulePrototype.PosZSocket;
            NegZSocket = _modulePrototype.NegZSocket;
            PosYSocket = _modulePrototype.PosYSocket;
            NegYSocket = _modulePrototype.NegYSocket;
            
            PosXNeighbors = _modulePrototype.PosXNeighbors;
            NegXNeighbors = _modulePrototype.NegZNeighbors;
            PosZNeighbors = _modulePrototype.PosZNeighbors;
            NegZNeighbors = _modulePrototype.NegZNeighbors;
            PosYNeighbors = _modulePrototype.PosYNeighbors;
            NegYNeighbors = _modulePrototype.NegYNeighbors;
        }
    }
}
