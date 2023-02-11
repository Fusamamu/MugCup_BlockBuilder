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

            if (GUILayout.Button("Test"))
            {
                combineMesh.Clone();
            }

            if (GUILayout.Button("Clone Mesh"))
            {
                combineMesh.CloneMesh();
            }
        }
    }
}
