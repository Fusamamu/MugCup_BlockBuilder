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
        }
    }
}
#endif
