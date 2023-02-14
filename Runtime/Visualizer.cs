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

        private const string POINTER_NAME = "Pointer";
        
        public enum PointerType { BLOCK_V1, BLOCK_V2, PATH }

        private static PointerType selectedPointerType = PointerType.BLOCK_V1;

        public static GameObject GetPointerReference(PointerType _type)
        {
            switch (_type)
            {
                case PointerType.BLOCK_V1:
                    
                    if (pointer == null)
                        pointer = CreateBlockTypeV1Pointer();
                    
                    break;
                
                case PointerType.BLOCK_V2:

                    if (pointer == null)
                        pointer = CreateBlockTypeV2Pointer();
                    
                    break;
                
                case PointerType.PATH:

                    if (pointer == null)
                        pointer = CreatePathTypePointer();
                    
                    break;
            }
            
            return pointer;
        }

        public static GameObject CreateBlockTypeV1Pointer()
        {
            AssetManager.LoadAssets();
                
            GameObject _block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            var _pointer  = Object.Instantiate(_block, Vector3.zero, Quaternion.identity);
            
            _pointer.name = POINTER_NAME;
            _pointer.GetComponent<Renderer>().material = AssetManager.MaterialData.VisualizerPointerV1Material;
            _pointer.GetComponent<Collider>().enabled = false;
            _pointer.transform.localScale *= 1.2f;
            
            Object.DestroyImmediate(_block);

            return _pointer;
        }
        
        public static GameObject CreateBlockTypeV2Pointer()
        {
            AssetManager.LoadAssets();
                
            GameObject _block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            var _pointer  = Object.Instantiate(_block, Vector3.zero, Quaternion.identity);
            
            _pointer.name = POINTER_NAME;
            _pointer.GetComponent<Renderer>().material = AssetManager.MaterialData.VisualizerPointerV2Material;
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
            
            _pointer.name = POINTER_NAME;

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
            
                _pathPointVisual.name = POINTER_NAME;
                
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
