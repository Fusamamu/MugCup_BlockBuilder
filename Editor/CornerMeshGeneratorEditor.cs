using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MugCup_BlockBuilder.Runtime;

namespace MugCup_BlockBuilder.Editor
{
    [CustomEditor(typeof(CornerMeshGenerator))]
    public class CornerMeshGeneratorEditor : UnityEditor.Editor
    {
        private CornerMeshGenerator cornerMeshGenerator;

        [SerializeField] private bool ShowGizmos;
        [SerializeField] private bool ShowDebugText;

        private void OnEnable()
        {
            cornerMeshGenerator = (CornerMeshGenerator)target;
            
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (GUILayout.Button("Generate Corner Meshes"))
            {
                cornerMeshGenerator.GenerateCornerMeshes();
            }
            
            if (GUILayout.Button("Clear All Corner Meshes"))
            {
                cornerMeshGenerator.ClearGeneratedMeshes();
            }
            
            if (GUILayout.Button("Update Prototypes Data"))
            {
                cornerMeshGenerator.UpdatePrototypesData();
            }

            ShowGizmos    = EditorGUILayout.Toggle("Show Gizmos", ShowGizmos);
            ShowDebugText = EditorGUILayout.Toggle("Show Debug Text", ShowDebugText);
            
            cornerMeshGenerator.SetShowGizmos   (ShowGizmos);
            cornerMeshGenerator.SetShowDebugText(ShowDebugText);
        }
    }
}
