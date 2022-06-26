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

        private static List<GameObject> pathPoints = new List<GameObject>();

        private const string PointerName = "Pointer";
        
        public enum PointerType { Block, Path }

        private static PointerType selectedPointerType = PointerType.Block;

        public static GameObject GetPointerReference(PointerType _type)
        {
            switch (_type)
            {
                case PointerType.Block:
                    
                    if (pointer == null)
                        pointer = CreateBlockTypePointer();
                    
                    break;
                
                case PointerType.Path:

                    if (pointer == null)
                        pointer = CreatePathTypePointer();
                    
                    break;
            }
            
            return pointer;
        }

        //may remove
        // public static GameObject GetPointerReference()
        // {
        //     if (pointer == null)
        //         pointer = CreateBlockTypePointer();
        //     
        //     return pointer;
        // }

        public static GameObject CreateBlockTypePointer()
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
        

        public static GameObject CreatePathTypePointer()
        {
            AssetManager.LoadAssets();

            GameObject _pathPointerPrefab = AssetManager.AssetCollection.PathPointerVisualizer;
            
            var _pointer  = Object.Instantiate(_pathPointerPrefab, Vector3.zero, _pathPointerPrefab.transform.localRotation);
            
            _pointer.name = PointerName;

            return _pointer;
        }

        public static void ClearPathVisualizer()
        {
            foreach (var _point in pathPoints)
            {
                Object.DestroyImmediate(_point);
            }
            
            pathPoints.Clear();
        }

        public static void CreatePathPointsVisualizer(List<Vector3Int> _path)
        {
            AssetManager.LoadAssets();
            
            foreach (var _point in _path)
            {
                GameObject _pathPointerPrefab = AssetManager.AssetCollection.PathPointerVisualizer;
            
                var _pathPointVisual  = Object.Instantiate(_pathPointerPrefab, _point, _pathPointerPrefab.transform.localRotation);
            
                _pathPointVisual.name = PointerName;
                
                pathPoints.Add(_pathPointVisual);
            }
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
