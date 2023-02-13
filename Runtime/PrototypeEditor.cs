#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CustomEditor(typeof(Prototype))]
    public class PrototypeEditor : Editor
    {
        private Prototype prototype;

        private void OnEnable()
        {
            prototype = (Prototype)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Try Update Data"))
            {
                prototype.TryUpdateData();
            }
            
            if (GUILayout.Button("Create Prototype Data Object"))
            {
                prototype.CreatePrototype();
            }
        }
    }
}
#endif
