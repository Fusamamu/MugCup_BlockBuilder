#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CustomEditor(typeof(ModulePrototype))]
    public class ModulePrototypeEditor : Editor
    {
        private ModulePrototype modulePrototype;

        private void OnEnable()
        {
            modulePrototype = (ModulePrototype)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Try Update Data"))
            {
                modulePrototype.TryUpdateData();
            }
            
            if (GUILayout.Button("Create Prototype Data Object"))
            {
                modulePrototype.CreateModule();
            }
        }
    }
}
#endif
