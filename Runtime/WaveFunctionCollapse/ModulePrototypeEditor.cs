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

            if (GUILayout.Button("Create Prototype Data Object"))
            {
                modulePrototype.CreateModule();
            }

            if (GUILayout.Button("Rotate Face Detail CCW"))
            {
                modulePrototype.FaceDetails.RotateCounterClockWise();
            }

#region Debug Section
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Debug Section");

            if (GUILayout.Button("Check Forward Face Bit"))
            {
                var _forwardBit = BitUtil.GetForwardFaceBit(modulePrototype.BitMask);
                BitUtil.Log4Bit(_forwardBit);
            }

            if (GUILayout.Button("Check Right Face Bit"))
            {
                var _rightBit = BitUtil.GetRightFaceBit(modulePrototype.BitMask);
                BitUtil.Log4Bit(_rightBit);
            }

            if (GUILayout.Button("Check Back Face Bit"))
            {
                var _backBit = BitUtil.GetBackFaceBit(modulePrototype.BitMask);
                BitUtil.Log4Bit(_backBit);
            }

            if (GUILayout.Button("Check Left Face Bit"))
            {
                var _leftBit = BitUtil.GetLeftFaceBit(modulePrototype.BitMask);
                BitUtil.Log4Bit(_leftBit);
            }

            if (GUILayout.Button("Check Up Face Bit"))
            {
                var _upBit = BitUtil.GetUpFaceBit(modulePrototype.BitMask);
                BitUtil.Log4Bit(_upBit);
            }

            if (GUILayout.Button("Check Down Face Bit"))
            {
                var _downBit = BitUtil.GetDownFaceBit(modulePrototype.BitMask);
                BitUtil.Log4Bit(_downBit);
            }

            if (GUILayout.Button("Check Flip Forward Face Bit"))
            {
                var _forwardBit = BitUtil.GetForwardFaceBit(modulePrototype.BitMask);
                var _flipBit    = BitUtil.FlipHorizontalFaceBIt(_forwardBit);
                BitUtil.Log4Bit(_flipBit);
            }
#endregion
        }
    }
}
#endif
