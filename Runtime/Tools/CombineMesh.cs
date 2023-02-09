using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public class CombineMesh : MonoBehaviour
    {
        [SerializeField] private MeshFilter MeshFilter;
        [SerializeField] private Material DefaultMaterial;

        private const string targetSavePath = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Meshes/Corner Meshes/DualCornerMesh/";

        public void CloneMesh()
        {
            int _bit = 0b_0000_1000;

            for (var _i = 0; _i < 4; _i++)
            {
                var _angle = (_i + 1) * 90;

                var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
                var _gameObject = new GameObject(_meshName);
                
                var _meshFilter = _gameObject.AddComponent<MeshFilter>();
                var _meshRender = _gameObject.AddComponent<MeshRenderer>();

                _meshRender.material = DefaultMaterial;

                var _meshToSave = CloneMesh(MeshFilter.sharedMesh);
                
                _meshFilter.sharedMesh = _meshToSave;

                var _vertices = new Vector3[_meshToSave.vertices.Length];

                for (int _j = 0; _j < _vertices.Length; _j++)
                {
                    var _point = MeshFilter.sharedMesh.vertices[_j];

                    var _newPoint = RotatePointAroundPivot(_point, MeshFilter.transform.position, new Vector3(0, _angle, 0));

                    _vertices[_j] = _newPoint;
                }

                _meshFilter.sharedMesh.vertices = _vertices;
                _meshFilter.sharedMesh.RecalculateNormals();
                _meshFilter.sharedMesh.RecalculateBounds();
                
                AssetDatabase.CreateAsset(_meshToSave, $"{targetSavePath}{_meshName}.asset");
                AssetDatabase.SaveAssets();

                _bit >>= 1;
            }
            
            _bit = 0b_1000_0000;
            
            for (var _i = 0; _i < 4; _i++)
            {
                var _angle = (_i + 1) * 90;

                var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
                var _gameObject = new GameObject(_meshName);
                
                var _meshFilter = _gameObject.AddComponent<MeshFilter>();
                var _meshRender = _gameObject.AddComponent<MeshRenderer>();

                _meshRender.material = DefaultMaterial;

                var _meshToSave = CloneMesh(MeshFilter.sharedMesh);
                
                _meshFilter.sharedMesh = _meshToSave;

                var _vertices = new Vector3[_meshToSave.vertices.Length];

                for (int _j = 0; _j < _vertices.Length; _j++)
                {
                    var _point = MeshFilter.sharedMesh.vertices[_j];

                    var _newPoint = RotatePointAroundPivot(_point, MeshFilter.transform.position, new Vector3(-90, _angle, 0));

                    _vertices[_j] = _newPoint;
                }

                _meshFilter.sharedMesh.vertices = _vertices;
                _meshFilter.sharedMesh.RecalculateNormals();
                _meshFilter.sharedMesh.RecalculateBounds();
                
                AssetDatabase.CreateAsset(_meshToSave, $"{targetSavePath}{_meshName}.asset");
                AssetDatabase.SaveAssets();

                _bit >>= 1;
            }
        }

        private Mesh CloneMesh(Mesh _mesh)
        {
            var _newMesh = new Mesh
            {
                vertices  = _mesh.vertices,
                triangles = _mesh.triangles,
                normals   = _mesh.normals,
                tangents  = _mesh.tangents,
                bounds    = _mesh.bounds,
                uv        = _mesh.uv
            };

            return _newMesh;
        }
        
        public Vector3 RotatePointAroundPivot(Vector3 _point, Vector3 _pivot, Vector3 _angles)
        {
            return Quaternion.Euler(_angles) * (_point - _pivot) + _pivot;
        }
    }
}
