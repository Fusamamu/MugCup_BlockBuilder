using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MugCup_BlockBuilder.Runtime;

namespace MugCup_BlockBuilder.Editor
{
    [CustomEditor(typeof(CombineMesh))]
    public class CombineMeshEditor : UnityEditor.Editor
    {
        private CombineMesh combineMesh;

        private void OnEnable()
        {
            combineMesh = (CombineMesh)target;
            
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // if (GUILayout.Button("Test Shift Bit"))
            // {
            //     combineMesh.TestShiftBit();
            // }
            
            if (GUILayout.Button("Generate Corner Meshes"))
            {
                combineMesh.GenerateCornerMeshes();
            }
            
            if (GUILayout.Button("Clear All Corner Meshes"))
            {
                combineMesh.ClearGeneratedMeshes();
            }
            
            if (GUILayout.Button("Toggle Show Gizmos"))
            {
                combineMesh.ToggleShowGizmos();
            }
            
            if (GUILayout.Button("Toggle Show Debug Text"))
            {
                combineMesh.ToggleShowDebugText();
            }

            if (GUILayout.Button("Generate CM 0000_0001"))
            {
                combineMesh.Generate_CM_0000_0001();
            }
            
            if (GUILayout.Button("Generate CM 0000_0011"))
            {
                combineMesh.Generate_CM_0000_0011();
            }
            
            if (GUILayout.Button("Generate CM 0000_0111"))
            {
                combineMesh.Generate_CM_0000_0111();
            }
            
            if (GUILayout.Button("Generate CM 0001_0011"))
            {
                combineMesh.Generate_CM_0001_0011();
            }
            
            if (GUILayout.Button("Generate CM 0001_1011"))
            {
                combineMesh.Generate_CM_0001_1011();
            }
            
            if (GUILayout.Button("Generate CM 0001_1111"))
            {
                combineMesh.Generate_CM_0001_1111();
            }
        }
    }
}
