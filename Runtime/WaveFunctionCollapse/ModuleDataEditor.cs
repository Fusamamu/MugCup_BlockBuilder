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

            // if (GUILayout.Button("Clean Module Data"))
            // {
            //     moduleData.CleanData();
            //     EditorUtility.SetDirty(moduleData);
            // }
            //
            // if (GUILayout.Button("Reset Module Data Index"))
            // {
            //     moduleData.ResetDataIndex();
            //     EditorUtility.SetDirty(moduleData);
            // }

            // if (GUILayout.Button("Store Modules Possible Neighbors"))
            // {
            //     moduleData.StoreModulesPossibleNeighbors();
            //     EditorUtility.SetDirty(moduleData);
            // }
            
            if (GUILayout.Button("Try Initialize Module Data"))
            {
                moduleData.Initialized();
            }

            EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(600));

                moduleData.DebugCornerMeshModuleData();
                moduleData.DebugModuleHealth();

            
            EditorGUILayout.EndVertical();
        }
    }
}
#endif
