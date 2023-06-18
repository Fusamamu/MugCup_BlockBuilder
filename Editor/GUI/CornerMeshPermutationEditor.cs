#if UNITY_EDITOR
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime.Tools;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
    [CustomEditor(typeof(CornerMeshPermutation))]
    [CanEditMultipleObjects]
    public class CornerMeshPermutationEditor : UnityEditor.Editor
    {
        private CornerMeshPermutation cornerMeshPermutation;

        private List<GameObject> dualBlocks = new List<GameObject>();

        private void OnEnable()
        {
            cornerMeshPermutation = (CornerMeshPermutation)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate Corner Meshes"))
            {
                Undo.RecordObject(cornerMeshPermutation, "Corner Meshed Generated");
                EditorUtility.SetDirty(cornerMeshPermutation);
                
                cornerMeshPermutation.ClearDualBlocks();
                cornerMeshPermutation.GenerateCornerMeshPermutation();

                dualBlocks = cornerMeshPermutation.GetDualBlocks();
            }

            if (GUILayout.Button("Clear"))
            {
                cornerMeshPermutation.ClearDualBlocks();
            }
        }

        private void OnSceneGUI()
        {
            // if(dualBlocks == null || dualBlocks.Count == 0) return;
            //
            // foreach (var _block in dualBlocks)
            // {
            //     var _children = _block.GetComponentsInChildren<Transform>();
            //
            //     foreach (var _child in _children)
            //     {
            //         Gizmos.color = Color.yellow;
            //         Gizmos.DrawSphere(_child.position, 0.1f);
            //     }
            // }
        }
    }
}
#endif
