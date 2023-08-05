#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CustomEditor(typeof(VolumePoint))]
    public class VolumePointEditor : Editor
    {
        private VolumePoint volumePoint;

        private void OnEnable()
        {
            volumePoint = (VolumePoint)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update BitMask"))
            {
                volumePoint.UpdateBitMask();
            }
        }
    }
}
#endif
