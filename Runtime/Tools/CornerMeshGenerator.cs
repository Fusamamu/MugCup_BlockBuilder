using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlockBuilder.Core.Scriptable;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public class CornerMeshGenerator : MonoBehaviour
    {
        [SerializeField] private MeshFilter CM_0000_0001;
        [SerializeField] private MeshFilter CM_0001_0000;
        
        [SerializeField] private MeshFilter CM_0000_0011;
        [SerializeField] private MeshFilter CM_0011_0000;
        
        [SerializeField] private MeshFilter CM_0000_0111;
        [SerializeField] private MeshFilter CM_0111_0000;
        
        [SerializeField] private MeshFilter CM_0001_0011;
        [SerializeField] private MeshFilter CM_0011_0001;
        
        [SerializeField] private MeshFilter CM_0001_1011;
        [SerializeField] private MeshFilter CM_1011_0001;
        
        [SerializeField] private MeshFilter CM_0001_1111;
        [SerializeField] private MeshFilter CM_1111_0001;

        [SerializeField] private MeshFilter CM_0011_1111;
        [SerializeField] private MeshFilter CM_1111_0011;

        [SerializeField] private MeshFilter CM_0111_1111;
        [SerializeField] private MeshFilter CM_1111_0111;

        [SerializeField] private MeshFilter CM_0011_0011;
        
        [SerializeField] private Material DefaultMaterial;

        [SerializeField] private bool CreateMeshAsset;
        [SerializeField] private bool CreatePrefabAsset;
        [SerializeField] private bool CreatePrototypeAsset;
        [SerializeField] private bool CreateCornerMeshData;

        private const string targetSavePath          = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Meshes/Corner Meshes/GeneratedCornerMesh/";
        private const string volumePointPrefabFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Prefabs/VolumePoints";

        [SerializeField] private int XInterval = 2;
        [SerializeField] private int ZInterval = 2;

        [SerializeField] public List<GameObject> AllGeneratedMeshes = new List<GameObject>();
        [SerializeField] public List<PrototypeData> AllPrototypeData = new List<PrototypeData>();

        public List<Prototype> AllPrototypes => AllGeneratedMeshes.Select(_o => _o.GetComponent<Prototype>()).Where(_p => _p != null).ToList();

        public void ClearGeneratedMeshes()
        {
#if UNITY_EDITOR
            foreach (var _object in AllGeneratedMeshes)
            {
                if(!Application.isPlaying)
                    DestroyImmediate(_object);
            }

            AllGeneratedMeshes = new List<GameObject>();
#endif
        }

        public void UpdatePrototypesData()
        {
            foreach(var _prototype in AllPrototypes)
                _prototype.TryUpdateData();
        }

        public void SetShowGizmos(bool _value)
        {
            foreach(var _prototype in AllPrototypes)
                _prototype.SetShowGizmos(_value);
        }

        public void SetShowDebugText(bool _value)
        {
            foreach(var _prototype in AllPrototypes)
                _prototype.SetShowDebugText(_value);
        }

        private int ShiftBit(int _bit)
        {
            var _upperBit = _bit & 0b_0000_1111;
            var _lowerBit = _bit & 0b_1111_0000;

            _upperBit <<= 1;

            var _checkBit = _upperBit & (1 << 4);

            if (_checkBit == 0b_0001_0000)
            {
                _upperBit -= 0b_0001_0000;
                _upperBit += 0b_0000_0001;
            }

            _lowerBit <<= 1;

            _checkBit = _lowerBit & 1 << 8;
            
            if (_checkBit == 0b_1_0000_0000)
            {
                _lowerBit -= 0b_1_0000_0000;
                _lowerBit += 0b_0_0001_0000;
            }

            _bit = _upperBit | _lowerBit;

            return _bit;
        }
        
        public void GenerateCornerMeshes()
        {
            var _bitMeshLut = new Dictionary<int, MeshFilter>()
            {
                { 0b_0000_0001, CM_0000_0001 },
                { 0b_0001_0000, CM_0001_0000 },
                
                { 0b_0000_0011, CM_0000_0011 },
                { 0b_0011_0000, CM_0011_0000 },
                
                { 0b_0000_0111, CM_0000_0111 },
                { 0b_0111_0000, CM_0111_0000 },
                
                { 0b_0001_0011, CM_0001_0011 },
                { 0b_0011_0001, CM_0011_0001 },
                
                { 0b_0001_1011, CM_0001_1011 },
                { 0b_1011_0001, CM_1011_0001 },
                
                { 0b_0001_1111, CM_0001_1111 },
                { 0b_1111_0001, CM_1111_0001 },
                
                { 0b_0011_1111, CM_0011_1111 },
                { 0b_1111_0011, CM_1111_0011 },
                
                { 0b_0111_1111, CM_0111_1111 },
                { 0b_1111_0111, CM_1111_0111 },
                
                { 0b_0011_0011, CM_0011_0011 },
            };

            AllPrototypeData = new List<PrototypeData>();

            var _j = 0;
            foreach (var _kvp in _bitMeshLut)
            {
                var _bit        = _kvp.Key;
                var _meshFilter = _kvp.Value;
                
                var _volumePoints = GenerateCornerMeshes_ByRotate_CounterClockwise(_bit, _meshFilter);

                for (int _i = 0; _i < _volumePoints.Count; _i++)
                {
                    _volumePoints[_i].transform.position += new Vector3(_i * XInterval, 0, _j * ZInterval);
                }

                _j++;
            }

            if (CreateCornerMeshData)
            {
                var _cornerMeshData = ScriptableObject.CreateInstance<CornerMeshData>();
                
                var _targetFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/CornerMeshData";
                var _fileName     = $"NewCornerMeshData.asset";
                
                AssetDatabase.CreateAsset(_cornerMeshData, _targetFolder + "/" + _fileName);
                AssetDatabase.SaveAssets();
                
                foreach (var _prototype in AllPrototypeData)
                {
                    _cornerMeshData.AddPrototypeData(_prototype.BitMask, _prototype);
                }
            }
        }

        public void AddPrototypeDataIntoCornerMeshData()
        {
            var _cornerMeshData = AssetDatabase.LoadAssetAtPath<CornerMeshData>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/CornerMeshData/NewCornerMeshData.asset");
            
            foreach (var _prototype in AllPrototypeData)
            {
                _cornerMeshData.AddPrototypeData(_prototype.BitMask, _prototype);
            }
        }

        private List<GameObject> GenerateCornerMeshes_ByRotate_CounterClockwise(int _bit, MeshFilter _sourceMesh)
        {
            var _volumePoints = new List<GameObject>();
            
            for (var _i = 0; _i < 4; _i++)
            {
                var _xRot = 0;
                var _yRot = _i * -90;
                var _zRot = 0;

                var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
                var _cloneMesh = CloneMesh(_sourceMesh.sharedMesh);

                RotateMesh(_cloneMesh, _sourceMesh.transform.position, new Vector3(_xRot, _yRot, _zRot));

                if (CreateMeshAsset)
                    SaveMesh(_cloneMesh, _meshName);

                var _newVolumePoint = CreateNewMesh(_cloneMesh, _meshName, out _, out _);

                if(CreatePrefabAsset)
                    SaveAsPrefab(_newVolumePoint, _meshName);

                var _prototype = _newVolumePoint.AddComponent<Prototype>();
                if (CreatePrototypeAsset)
                {
                    _prototype.TryUpdateData();
                    _prototype.SetBitMask(_bit);
                    
                    AllPrototypeData.Add(_prototype.CreatePrototype());
                }

                _bit = ShiftBit(_bit);
                
                _volumePoints     .Add(_newVolumePoint);
                AllGeneratedMeshes.Add(_newVolumePoint);
            }

            return _volumePoints;
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
