#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugCup_BlockBuilder.Editor
{
    [CustomEditor(typeof(BaseModuleSetSo))]
    public class BaseModuleSetSoEditor : UnityEditor.Editor
    {
        private BaseModuleSetSo baseModuleSetSo;

        private void OnEnable()
        {
            baseModuleSetSo = (BaseModuleSetSo)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            if (GUILayout.Button("Generate default base module groups"))
            {
                if (baseModuleSetSo.BaseModuleGroups.Count != 0)
                {
                    bool _result = EditorUtility.DisplayDialog("Warning", "This will clear current data and generate new default module groups!", "OK", "Cancel");

                    if(_result)
                        baseModuleSetSo.GenerateDefaultBaseModuleGroups();
                }
                else
                {
                    baseModuleSetSo.GenerateDefaultBaseModuleGroups();
                }
            }

            if (GUILayout.Button("Update bitmask based on MetaData"))
                baseModuleSetSo.UpdateData();
            
            if (GUILayout.Button("Clear Data"))
            {
                bool _result = EditorUtility.DisplayDialog("Warning", "Confirm delete all current data", "OK", "Cancel");

                if(_result)
                    baseModuleSetSo.ClearData();
            }
        }
    }
}
#endif
