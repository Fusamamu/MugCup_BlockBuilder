#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugCup_BlockBuilder.Editor
{
    public static class BlockAssetManager
    {
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> _assets = new List<T>();
            
            string[] _guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            
            for (var _i = 0; _i < _guids.Length; _i++ )
            {
                string _assetPath = AssetDatabase.GUIDToAssetPath(_guids[_i]);
                
                T _asset = AssetDatabase.LoadAssetAtPath<T>(_assetPath);
                
                if(_asset != null)
                    _assets.Add(_asset);
            }
            return _assets;
        }
    }
}
#endif
