#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugCup_BlockBuilder
{
    [CustomEditor(typeof(ModuleSlot))]
    public class ModuleSlotEditor : Editor
    {
        private ModuleSlot moduleSlot;

        private void OnEnable()
        {
            moduleSlot = (ModuleSlot)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.LabelField($"[Is Collapse] : {moduleSlot.IsCollapsed}");

            if (GUILayout.Button("Collapse Random"))
            {
                moduleSlot.CollapseRandom();
            }
        }
    }
}
#endif
