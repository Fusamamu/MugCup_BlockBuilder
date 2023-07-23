using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CreateAssetMenu(fileName = "MapTextureDataSetting", menuName = "ScriptableObjects/MugCup Block Builder/Map Texture Data Setting", order = 2)]
    public class MapTextureDataSettingSO : ScriptableObject
    {
        public Texture2D GeneratedTexture;
        
        public int TextureWidth  = 512;
        public int TextureHeight = 512;

        public int TerrainLevel = 2;
        public int NoiseSeed;
        public float NoiseScale = 1;
        public float NoiseFrequency;
        public float NoiseRedistribution;
    }
}
