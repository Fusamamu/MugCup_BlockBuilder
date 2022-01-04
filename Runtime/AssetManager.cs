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
        public static AssetCollection AssetCollection 
        {
            get
            {
                if(!assetLoaded)
                    LoadAssets();

                return assetCollection;
            }
        }

        public static MaterialData MaterialData 
        {
            get
            {
                if(!assetLoaded)
                    LoadAssets();

                return materialData;
            }
        }

        private static AssetCollection assetCollection;
        private static MaterialData    materialData;

        private static bool assetLoaded = false;
        
        public static void LoadAssets()
        {
            assetCollection = AssetDatabase.LoadAssetAtPath<AssetCollection>(DataPath.AssetCollectionPath);
            materialData    = AssetDatabase.LoadAssetAtPath<MaterialData>   (DataPath.MaterialDataPath   );

            assetLoaded = true;
        }
    }
}
