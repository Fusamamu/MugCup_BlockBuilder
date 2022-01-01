using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEditor.UIElements;
#endif

using BlockBuilder.Scriptable;

#if UNITY_EDITOR
namespace BlockBuilder.Editor.GUI
{
    // [CustomPropertyDrawer(typeof(GridDataSetting))]
    // public class GridDataDrawer : PropertyDrawer
    // {
        // public override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
        // {
        //     return 32 * 3;
        // }
        
        // public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        // {
        //     EditorGUI.BeginProperty( _position, _label, _property );
        //     
        //     Rect _texRect = new Rect(_position.x, _position.y, _position.width, EditorGUIUtility.singleLineHeight);
        //     
        //     UnityEngine.GUI.DrawTexture(_texRect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
        //     
        //         EditorGUI.PrefixLabel( _position, GUIUtility.GetControlID( FocusType.Passive ), _label );
        //     
        //         
        //         SerializedObject _gridDataSo = new SerializedObject(_property.objectReferenceValue);
        //         
        //         SerializedProperty _row    = _gridDataSo.FindProperty("Row");
        //         SerializedProperty _column = _gridDataSo.FindProperty("Column");
        //         SerializedProperty _height = _gridDataSo.FindProperty("Height");
        //     
        //         SerializedProperty _unitRow    = _gridDataSo.FindProperty("UnitRow");
        //         SerializedProperty _unitColumn = _gridDataSo.FindProperty("UnitColumn");
        //         SerializedProperty _unitHeight = _gridDataSo.FindProperty("UnitHeight");
        //     
        //         SerializedProperty _blockWidth = _gridDataSo.FindProperty("BlockWidth");
        //         SerializedProperty _blockHeight = _gridDataSo.FindProperty("BlockHeight");
        //         SerializedProperty _blockDepth = _gridDataSo.FindProperty("BlockDepth");
        //
        //         float _verticalPadding = 5f;
        //         
        //         Rect _rowRect    = new Rect(_position.x, _position.y + EditorGUIUtility.singleLineHeight + _verticalPadding, _position.width,      EditorGUIUtility.singleLineHeight);
        //         Rect _columnRect = new Rect(_position.x, _position.y + (EditorGUIUtility.singleLineHeight + _verticalPadding) * 2 ,  _position.width, EditorGUIUtility.singleLineHeight);
        //         Rect _heightRect = new Rect(_position.x, _position.y + (EditorGUIUtility.singleLineHeight + _verticalPadding) * 3, _position.width, EditorGUIUtility.singleLineHeight);
        //         
        //         EditorGUIUtility.labelWidth = _position.width / 3;
        //         EditorGUIUtility.fieldWidth = 20;
        //         _row.intValue    = EditorGUI.IntField(_rowRect,    "Row    : ", _row.intValue);
        //         _column.intValue = EditorGUI.IntField(_columnRect, "Column : ", _column.intValue);
        //         _height.intValue = EditorGUI.IntField(_heightRect, "Height : ", _column.intValue);
        //     
        //     GUILayout.BeginVertical("HelpBox");
        //     
        //         GUILayout.Label("Grid Data");
        //         
        //         GUILayout.BeginVertical("GroupBox");
        //         GUILayout.Label("Map Size Setting");
        //         _row.intValue    = EditorGUILayout.IntField("Row :", _row.intValue);
        //         _column.intValue = EditorGUILayout.IntField("Column : ", _column.intValue);
        //         _height.intValue = EditorGUILayout.IntField("Height : ", _height.intValue);
        //         GUILayout.EndVertical();
        //         
        //         GUILayout.BeginVertical("GroupBox");
        //         GUILayout.Label("Grid Unit Size Setting");
        //         _unitRow.intValue    = EditorGUILayout.IntField("Unit Row : ", _unitRow.intValue);
        //         _unitColumn.intValue = EditorGUILayout.IntField("Unit Column : ", _unitColumn.intValue);
        //         _unitHeight.intValue = EditorGUILayout.IntField("Unit Height : ", _unitColumn.intValue);
        //         GUILayout.EndVertical();
        //         
        //         GUILayout.BeginVertical("GroupBox");
        //         GUILayout.Label("Block Size");
        //         GUILayout.BeginHorizontal();
        //         _blockWidth.floatValue = EditorGUILayout.FloatField("Width : ", _blockWidth.floatValue);
        //         _blockDepth.floatValue = EditorGUILayout.FloatField("Depth : ", _blockDepth.floatValue );
        //         _blockHeight.floatValue= EditorGUILayout.FloatField("Height : ", _blockHeight.floatValue);
        //         GUILayout.EndHorizontal();
        //         GUILayout.EndVertical();
        //         
        //     GUILayout.EndVertical();
        //     
        //     EditorGUI.EndProperty();
        // }
    //}
}
#endif
