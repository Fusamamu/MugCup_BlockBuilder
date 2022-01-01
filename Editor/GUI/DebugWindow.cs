using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace BlockBuilder.Editor.GUI
{
    public class DebugWindow : EditorWindow
    {
        private static GameObject selectedBlock;
        private static Vector3Int blockPosition;
        
        [MenuItem("Tools/Debug Window/Enable")]
        public static void Enable()
        {
            SceneView.duringSceneGui += OnScene;
        }
  
        [MenuItem("Tools/Debug Window/Disable")]
        public static void Disable()
        {
            SceneView.duringSceneGui -= OnScene;
        }
  
        private static void OnScene(SceneView sceneview)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(20, 20, 300, 100));
            
            if (GUILayout.Button("Generate Grid Unit")) 
            {
                if (Application.isPlaying)
                {
                    
                }
            }

            selectedBlock = EditorGUILayout.ObjectField("Selected Block", selectedBlock, typeof(GameObject), true) as GameObject;
            blockPosition = EditorGUILayout.Vector3IntField("Block Position", blockPosition);
        
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}
#endif
