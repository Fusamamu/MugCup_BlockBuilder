#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugCup_BlockBuilder
{
    [CustomEditor(typeof(ModuleData))]
    public class ModuleDataEditor : Editor
    {
        private ModuleData moduleData;

        private void OnEnable()
        {
            moduleData = (ModuleData)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Store Modules Possible Neighbors"))
            {
                moduleData.StoreModulesPossibleNeighbors();
                EditorUtility.SetDirty(moduleData);
            }
            
            if (GUILayout.Button("Try Initialize Module Data"))
            {
                moduleData.Initialized();
            }

            EditorGUILayout.BeginVertical("HelpBox");
            
            moduleData.DebugCornerMeshModuleData();
            
            EditorGUILayout.EndVertical();


        }
    }
}
#endif
