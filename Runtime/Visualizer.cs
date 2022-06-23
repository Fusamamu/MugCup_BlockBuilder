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

        private const string PointerName = "Pointer";
        
        public enum PointerType { Block, Path }

        private static PointerType selectedPointerType = PointerType.Block;

        public static GameObject GetPointerReference(PointerType _type)
        {
            return null;
        }

        public static GameObject GetPointerReference()
        {
            if (pointer == null)
                pointer = CreatePointer();
            
            return pointer;
        }

        public static GameObject CreatePointer()
        {
            AssetManager.LoadAssets();
                
            GameObject _block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            var _pointer  = Object.Instantiate(_block, Vector3.zero, Quaternion.identity);
            _pointer.name = PointerName;
            _pointer.GetComponent<Renderer>().material = AssetManager.MaterialData.VisualizerPointerMaterial;
            _pointer.GetComponent<Collider>().enabled = false;
            _pointer.transform.localScale *= 1.2f;
            
            Object.DestroyImmediate(_block);

            return _pointer;
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
