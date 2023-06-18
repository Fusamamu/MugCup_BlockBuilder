#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
    public static class BBAssets
    {
        public static List<GameObject> QueriedObjects;

        public static void LoadPrefab()
        {
            var _allGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources/Prefabs/Towers" });
            
            QueriedObjects = new List<GameObject>();
            
            foreach (var _guid in _allGuids)
            {
                QueriedObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_guid)));
            }
        }
    }
}
#endif
