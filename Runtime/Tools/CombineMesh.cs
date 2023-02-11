using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public class CombineMesh : MonoBehaviour
    {
        [SerializeField] private MeshFilter SourceMeshFilter;
        [SerializeField] private Material DefaultMaterial;

        [SerializeField] private bool CreateMeshAsset;
        [SerializeField] private bool CreatePrefabAsset;

        private const string targetSavePath          = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Meshes/Corner Meshes/DualCornerMesh/";
        private const string volumePointPrefabFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Prefabs/VolumePoints";
        
        public void Clone()
        {
            int _bit = 0b_0000_0001;

            var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
            var _cloneMesh = CloneMesh(SourceMeshFilter.sharedMesh);

            RotateMesh(_cloneMesh, SourceMeshFilter.transform.position, new Vector3(90, 0, 0));

            if (CreateMeshAsset)
                SaveMesh(_cloneMesh, _meshName);

            var _newVolumePoint = CreateNewMesh(_cloneMesh, _meshName, out _, out _);

            if(CreatePrefabAsset)
                SaveAsPrefab(_newVolumePoint, _meshName);
        }

        public void CloneMesh()
        {
            int _bit = 0b_0000_0001;

            for (var _i = 0; _i < 8; _i++)
            {
                var _xRot = 0;
                var _yRot = _i * -90;
                var _zRot = 0;

                if (_i > 3)
                    _xRot = 90;

                var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
                var _cloneMesh = CloneMesh(SourceMeshFilter.sharedMesh);

                RotateMesh(_cloneMesh, SourceMeshFilter.transform.position, new Vector3(_xRot, _yRot, _zRot));

                if (CreateMeshAsset)
                    SaveMesh(_cloneMesh, _meshName);

                var _newVolumePoint = CreateNewMesh(_cloneMesh, _meshName, out _, out _);

                if(CreatePrefabAsset)
                    SaveAsPrefab(_newVolumePoint, _meshName);
             
                _bit <<= 1;
            }
        }

        private void SaveMesh(Mesh _mesh, string _name)
        {
            AssetDatabase.CreateAsset(_mesh, $"{targetSavePath}{_name}.asset");
            AssetDatabase.SaveAssets();
        }

        private void SaveAsPrefab(GameObject _gameObject, string _meshName)
        {
            if (!Directory.Exists(volumePointPrefabFolder)) return;
            
            var _localPath = volumePointPrefabFolder + "/" + _meshName + ".prefab";

            _localPath = AssetDatabase.GenerateUniqueAssetPath(_localPath);

            PrefabUtility.SaveAsPrefabAsset(_gameObject, _localPath, out var _success);

            Debug.Log($"Save new volume point : {_success}");
        }

        private void RotateMesh(Mesh _mesh, Vector3 _pivot, Vector3 _angle)
        {
            var _vertices = new Vector3[_mesh.vertices.Length];

            for (int _j = 0; _j < _vertices.Length; _j++)
            {
                var _point = _mesh.vertices[_j];

                var _newPoint = RotatePointAroundPivot(_point, _pivot, _angle);

                _vertices[_j] = _newPoint;
            }

            _mesh.vertices = _vertices;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
        
        private GameObject CreateNewMesh(Mesh _mesh, string _name, out MeshFilter _meshFilter, out MeshRenderer _renderer)
        {
            var _gameObject = new GameObject(_name);
                
            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            _renderer   = _gameObject.AddComponent<MeshRenderer>();
            
            _gameObject.AddComponent<VolumePoint>();

            _meshFilter.sharedMesh = _mesh;
            _renderer  .material   = DefaultMaterial;

            return _gameObject;
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
