#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Core.Scriptable;
using BlockBuilder.Scriptable;
using UnityEditor;
using UnityEngine;

using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Editor;
using MugCup_PathFinder.Runtime;

namespace MugCup_BlockBuilder
{
    public static class SettingPanelGUI
    {
        public static void Display()
        {
            BBEditorManager.GridDataSettingSo = (GridDataSettingSO)EditorGUILayout.ObjectField("Grid Data Setting", BBEditorManager.GridDataSettingSo, typeof(GridDataSettingSO), true);
            
            var _meshData = (BlockMeshData)EditorGUILayout.ObjectField("Block Mesh Data Setting", null, typeof(BlockMeshData), true);
            var _material = (Material)     EditorGUILayout.ObjectField("Default Block Material" , null, typeof(Material)     , true);
        }
    }
}
#endif
