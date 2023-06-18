#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
    public static class BlockPreviewEditor 
    {
        private static PreviewRenderUtility previewRenderUtility;

        private static bool isInit;

        public static void Init()
        {
            Clean();
            
            if(previewRenderUtility == null)
                previewRenderUtility = new PreviewRenderUtility();
            
            var _camera = previewRenderUtility.camera;
            
            //_camera.cameraType = CameraType.Preview;
            
            _camera.fieldOfView   = 30f;
            _camera.nearClipPlane = 0.01f;
            _camera.farClipPlane  = 1000;
            
            _camera.transform.position = new Vector3(-3, 3, 0);
            _camera.transform.LookAt(Vector3.zero);
            
            //Not working
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = Color.cyan;
        }

        public static void ChangeCameraBackgroundColor(Color _color)
        {
            //Not working
            previewRenderUtility.camera.backgroundColor = _color;
            previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
        }
        
        public static Texture CreatePreviewTexture(Rect _rect, GameObject _gameObject)
        {
            MeshFilter   _meshFilter   = _gameObject.GetComponent<MeshFilter>();
            MeshRenderer _meshRenderer = _gameObject.GetComponent<MeshRenderer>();
				    
            previewRenderUtility.BeginPreview(_rect, GUIStyle.none);
            
            previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
            previewRenderUtility.lights[0].intensity = 2;
            
            Quaternion _rotation = Quaternion.Euler(270, 45, 0);
            
            Matrix4x4 _newRot = new Matrix4x4();
            
            _newRot.SetTRS(Vector3.zero, _rotation, Vector3.one);

            previewRenderUtility.DrawMesh(_meshFilter.sharedMesh, _newRot, _meshRenderer.sharedMaterial, 0);
            
            previewRenderUtility.camera.Render();
		    
            return previewRenderUtility.EndPreview();
        }

        public static void Clean()
        {
            if (previewRenderUtility != null)
            {
                previewRenderUtility.Cleanup();
                previewRenderUtility = null;
            }
        }
    }
}
#endif
