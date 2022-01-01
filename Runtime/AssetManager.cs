using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BlockBuilder.Core;
using BlockBuilder.Runtime.Core;
using BlockBuilder.Runtime.Scriptable;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime.Core;

namespace BlockBuilder
{
    public static class AssetManager
    {
        public static Material VisualizerPointerMaterial;

        public static Block DefaultHouse;

        public static AssetCollection AssetCollection;

        public static void LoadAssets()
        {
            VisualizerPointerMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/VisualizerPointer.mat");
            
            DefaultHouse    = AssetDatabase.LoadAssetAtPath<Block>("Assets/Resources/Prefabs/Buildings/House1x.prefab");

            AssetCollection = Resources.Load<AssetCollection>("BlockBuilder/Setting/AssetCollection");
        }
    }
}
