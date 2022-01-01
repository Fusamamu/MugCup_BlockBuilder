using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;
#if UNITY_EDITOR
using UnityEditor;
#endif
using BlockBuilder;

namespace MugCup_BlockBuilder
{
#if UNITY_EDITOR
    public static class Visualizer
    {
        private static GameObject pointer;

        public static GameObject GetPointerReference()
        {
            CreatePointer();
            return pointer;
        }

        private static void CreatePointer()
        {
            if (pointer != null) return;
            
            AssetManager.LoadAssets();
                
            GameObject _block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            pointer = Object.Instantiate(_block, Vector3.zero, Quaternion.identity);
            pointer.name = "Pointer";
            pointer.GetComponent<Renderer>().material = AssetManager.VisualizerPointerMaterial;
            pointer.GetComponent<Collider>().enabled = false;
            pointer.transform.localScale *= 1.2f;
            
            Object.DestroyImmediate(_block);
        }

        public static void DrawLine(Vector3 _pointA, Vector3 _pointB, Color _color, float _datSize)
        {
            Handles.color = _color;
            Handles.DrawDottedLine(_pointA, _pointB, _datSize);
        }
        
        public static void ClearPointer()
        {
            Object.DestroyImmediate(pointer);
            pointer = null;
        }
    }
#endif
}
