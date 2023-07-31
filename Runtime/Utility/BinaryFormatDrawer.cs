#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugCup_BlockBuilder
{
    // [CustomPropertyDrawer(typeof(int))]
    // public class BinaryFormatDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    //     {
    //         EditorGUI.BeginProperty(_position, _label, _property);
    //         EditorGUI.LabelField(_position, _label.text, GetBinaryString(_property.intValue));
    //         EditorGUI.EndProperty();
    //     }
    //
    //     private string GetBinaryString(int _value)
    //     {
    //         return System.Convert.ToString(_value, 2);
    //     }
    // }
}
#endif