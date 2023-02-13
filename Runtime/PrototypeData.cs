using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CreateAssetMenu(fileName = "PrototypeData", menuName = "ScriptableObjects/PrototypeDataObject", order = 8)]
    public class PrototypeData : ScriptableObject
    {
        public string Name;
        
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

        public void CopyData(Prototype _prototype)
        {
            Name = _prototype.Name;

            MeshPrototype = _prototype.MeshPrototype;
            RotationIndex = _prototype.RotationIndex;

            PosXSocket = _prototype.PosXSocket;
            NegXSocket = _prototype.NegXSocket;
            PosZSocket = _prototype.PosZSocket;
            NegZSocket = _prototype.NegZSocket;
            PosYSocket = _prototype.PosYSocket;
            NegYSocket = _prototype.NegYSocket;
            
            PosXNeighbors = _prototype.PosXNeighbors;
            NegXNeighbors = _prototype.NegZNeighbors;
            PosZNeighbors = _prototype.PosZNeighbors;
            NegZNeighbors = _prototype.NegZNeighbors;
            PosYNeighbors = _prototype.PosYNeighbors;
            NegYNeighbors = _prototype.NegYNeighbors;
        }
    }
}
