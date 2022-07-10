using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime;
using UnityEditor;
using UnityEngine;
using MugCup_BlockBuilder.Runtime.Core;

namespace MugCup_BlockBuilder.Editor
{
    [CustomEditor(typeof(Block))]
    public class BlockEditor : UnityEditor.Editor
    {
        private Block block;

        private void OnEnable()
        {
            block = (Block)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update BitMask"))
            {
                block.UpdateBlockData();
            }

            if (GUILayout.Button("Debug Node Position"))
            {
                Debug.Log($"Node Position : {block.NodePosition}");
            }
        }
    }
}
