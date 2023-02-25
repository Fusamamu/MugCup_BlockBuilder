using System;
using System.IO;
using System.Collections.Generic;
using BlockBuilder.Core.Scriptable;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public class CornerMeshGenerator : MonoBehaviour
    {
        public BaseModuleSet BaseModuleSet = new BaseModuleSet();

        [SerializeField] private MeshFilter CM_1111_0000;
        
        [SerializeField] private MeshFilter CM_0000_0001;
        [SerializeField] private MeshFilter CM_0001_0000;

        [SerializeField] private MeshFilter CM_0001_0001;
        [SerializeField] private MeshFilter CM_0010_0001;
        [SerializeField] private MeshFilter CM_0101_0000;
        
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

        [SerializeField] private MeshFilter CM_0111_0111;
        [SerializeField] private MeshFilter CM_0111_0001;

        [SerializeField] private MeshFilter CM_0111_0011;
        [SerializeField] private MeshFilter CM_1111_0101;

        [SerializeField] private MeshFilter CM_0101_0001;
        [SerializeField] private MeshFilter CM_0101_0101;
        [SerializeField] private MeshFilter CM_0111_0101;
        
        [SerializeField] private Material DefaultMaterial;

        private const string targetSavePath          = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Meshes/Corner Meshes/GeneratedCornerMesh/";
        private const string volumePointPrefabFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Prefabs/VolumePoints";

        [SerializeField] private int XInterval = 2;
        [SerializeField] private int ZInterval = 2;

        [SerializeField] public List<GameObject>      AllVolumePointObjects;
        [SerializeField] public List<Mesh>            AllGeneratedMeshes;
        [SerializeField] public List<ModulePrototype> AllPrototypes;
        [SerializeField] public List<Module>          AllModules;

        private Dictionary<int, bool> generatedMeshTracker = new Dictionary<int, bool>();

        public void ClearGeneratedMeshes()
        {
#if UNITY_EDITOR
            foreach (var _object in AllVolumePointObjects)
            {
                if(!Application.isPlaying)
                    DestroyImmediate(_object);
            }

            ClearData();
#endif
        }
        
#if UNITY_EDITOR
        public void AddPrototypeDataIntoCornerMeshData()
        {
            var _cornerMeshData = AssetDatabase.LoadAssetAtPath<CornerMeshData>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/CornerMeshData/NewCornerMeshData.asset");
            
            foreach (var _prototype in AllModules)
            {
                _cornerMeshData.AddPrototypeData(_prototype.BitMask, _prototype);
            }
            
            EditorUtility.SetDirty(_cornerMeshData);
        }
#endif

        private void ClearData()
        {
            AllVolumePointObjects = new List<GameObject>();
            AllGeneratedMeshes    = new List<Mesh>();
            AllPrototypes         = new List<ModulePrototype>();
            AllModules            = new List<Module>();
        }

        public void UpdatePrototypesData()
        {
            foreach(var _prototype in AllPrototypes)
                _prototype.TryUpdateData();
        }
        
        public void GenerateCornerMeshes()
        {
            var _bitMeshLut = new Dictionary<int, MeshFilter>()
            {
                //{ 0b_1111_0000, CM_1111_0000 },
                
                { 0b_0000_0001, CM_0000_0001 },
                { 0b_0001_0000, CM_0001_0000 },
                
                { 0b_0001_0001, CM_0001_0001 },
                { 0b_0010_0001, CM_0010_0001 },
                { 0b_0101_0000, CM_0101_0000 },
                
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
                
                { 0b_0111_0111, CM_0111_0111 },
                { 0b_0111_0001, CM_0111_0001 },
                
                { 0b_0111_0011, CM_0111_0011 },
                { 0b_1111_0101, CM_1111_0101 },
                
                { 0b_0101_0001, CM_0101_0001 },
                { 0b_0101_0101, CM_0101_0101 },
                { 0b_0111_0101, CM_0111_0101 },
            };

            generatedMeshTracker.Clear();
            for (var _i = 0; _i < 256; _i++)
                generatedMeshTracker.Add(_i, false);

            ClearData();

            var _cornerMesh = GenerateCornerMesh(0b_1111_0000, CM_1111_0000);
            _cornerMesh.transform.position += Vector3.back * ZInterval;

            var _j = 0;
            foreach (var _kvp in _bitMeshLut)
            {
                var _bit        = _kvp.Key;
                var _meshFilter = _kvp.Value;
                
                var _volumePoints = GenerateCornerMeshes_ByRotate_CounterClockwise(_bit, _meshFilter);

                for (int _i = 0; _i < _volumePoints.Count; _i++)
                    _volumePoints[_i].transform.position += new Vector3(_i * XInterval, 0, _j * ZInterval);

                _j++;
            }
            
            foreach (var _kvp in _bitMeshLut)
            {
                var _bit        = _kvp.Key;
                var _meshFilter = _kvp.Value;
                
                _bit = MirrorBitXAis(_bit);
            
                if (generatedMeshTracker.TryGetValue(_bit, out var _isGenerated))
                {
                    if(_isGenerated)
                        continue; 
                }
                
                var _volumePoints = GenerateCornerMeshes_ByMirror_XAxis(_bit, _meshFilter);
            
                for (int _i = 0; _i < _volumePoints.Count; _i++)
                    _volumePoints[_i].transform.position += new Vector3(_i * XInterval, 0, _j * ZInterval);
            
                _j++;
            }

            var _index = 0;
            foreach (var _prototype in AllPrototypes)
            {
                _prototype.Index = _index;
                _index++;
            }
        }

        private GameObject GenerateCornerMesh(int _bit, MeshFilter _sourceMesh)
        {
            var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
            var _cloneMesh = CloneMesh(_sourceMesh.sharedMesh);
            _cloneMesh.name = _meshName;

            var _newMeshObject = CreateNewMesh(_cloneMesh, _meshName, out _, out _, out var _volumePoint, out var _prototype);

            _volumePoint.SetBitMask(_bit);
            _prototype  .SetBitMask(_bit);

            if (generatedMeshTracker.ContainsKey(_bit))
                generatedMeshTracker[_bit] = true;
               
            AllVolumePointObjects.Add(_newMeshObject);
            AllGeneratedMeshes   .Add(_cloneMesh);
            AllPrototypes        .Add(_prototype);

            return _newMeshObject;
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
                _cloneMesh.name = _meshName;

                RotateMesh(_cloneMesh, _sourceMesh.transform.position, new Vector3(_xRot, _yRot, _zRot));

                var _newMeshObject = CreateNewMesh(_cloneMesh, _meshName, out _, out _, out var _volumePoint, out var _prototype);

                _volumePoint.SetBitMask(_bit);
                _prototype  .SetBitMask(_bit);

                if (generatedMeshTracker.ContainsKey(_bit))
                    generatedMeshTracker[_bit] = true;
               
                _bit = ShiftBit(_bit);
                
                _volumePoints.Add(_newMeshObject);
                
                AllVolumePointObjects.Add(_newMeshObject);
                AllGeneratedMeshes   .Add(_cloneMesh);
                AllPrototypes        .Add(_prototype);
            }

            return _volumePoints;
        }
        
        private List<GameObject> GenerateCornerMeshes_ByMirror_XAxis(int _bit, MeshFilter _sourceMesh)
        {
            var _volumePoints = new List<GameObject>();
            
            for (var _i = 0; _i < 4; _i++)
            {
                var _xRot = 0;
                var _yRot = _i * -90;
                var _zRot = 0;

                var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
                var _cloneMesh = CloneMesh(_sourceMesh.sharedMesh);
                _cloneMesh.name = _meshName;

                MirrorMeshXAxis(_cloneMesh);
                RotateMesh(_cloneMesh, _sourceMesh.transform.position, new Vector3(_xRot, _yRot, _zRot));

                var _newMeshObject = CreateNewMesh(_cloneMesh, _meshName, out _, out _, out var _volumePoint, out var _prototype);

                _volumePoint.SetBitMask(_bit);
                _prototype  .SetBitMask(_bit);

                if (generatedMeshTracker.ContainsKey(_bit))
                    generatedMeshTracker[_bit] = true;
               
                _bit = ShiftBit(_bit);
                
                _volumePoints.Add(_newMeshObject);
                
                AllVolumePointObjects.Add(_newMeshObject);
                AllGeneratedMeshes   .Add(_cloneMesh);
                AllPrototypes        .Add(_prototype);
            }

            return _volumePoints;
        }

        public void SaveAllGeneratedMeshes(string _targetFolderPath)
        {
            if (AllGeneratedMeshes is { Count: > 0 })
            {
                foreach (var _mesh in AllGeneratedMeshes)
                {
                    AssetDatabase.CreateAsset(_mesh, $"{_targetFolderPath}/{_mesh.name}.asset");
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void SaveAsPrefab(GameObject _gameObject, string _meshName)
        {
            if (!Directory.Exists(volumePointPrefabFolder)) return;
            
            var _localPath = volumePointPrefabFolder + "/" + _meshName + ".prefab";

            _localPath = AssetDatabase.GenerateUniqueAssetPath(_localPath);

            PrefabUtility.SaveAsPrefabAsset(_gameObject, _localPath, out var _success);

            Debug.Log($"Save new volume point : {_success}");
        }

        public void SaveModules(string _targetFolderPath)
        {
            var _index = 0;
            
            foreach (var _prototype in AllPrototypes)
            {
                _prototype.TryUpdateData();

                var _module = _prototype.CreateModule();
                
                var _fileName = $"{_prototype.name}.asset";
                
                AssetDatabase.CreateAsset(_module, $"{_targetFolderPath}/{_fileName}");
                AssetDatabase.SaveAssets();
                
                AllModules.Add(_module);
                
                EditorUtility.DisplayProgressBar("Creating module prototypes...", _module.Name, (float)_index/ AllModules.Count);
                _index++;
            }
            
            EditorUtility.ClearProgressBar();
        }

        public void SaveCornerMeshData(string _targetFolderPath)
        {
            var _cornerMeshData = ScriptableObject.CreateInstance<CornerMeshData>();
            
            var _fileName = $"CornerMeshData.asset";
            
            AssetDatabase.CreateAsset(_cornerMeshData, $"{_targetFolderPath}/{_fileName}");
            AssetDatabase.SaveAssets();
            
            foreach (var _module in AllModules)
            {
                _cornerMeshData.AddPrototypeData(_module.BitMask, _module);
            }
            
            EditorUtility.SetDirty(_cornerMeshData);
        }

        public CornerMeshGenerator StoreModulesPossibleNeighbors()
        {
            foreach (var _module in AllModules)
            {
                _module.StorePossibleNeighbors(AllModules);
            }
            return this;
        }
        
        public void SaveCornerMeshModuleData(string _targetFolderPath)
        {
            var _cornerMeshModuleData = ScriptableObject.CreateInstance<CornerMeshModuleData>();
            
            var _fileName = $"CornerMeshModuleData.asset";
            
            AssetDatabase.CreateAsset(_cornerMeshModuleData, $"{_targetFolderPath}/{_fileName}");
            AssetDatabase.SaveAssets();
            
            _cornerMeshModuleData.Modules = AllModules.ToArray();
            
            EditorUtility.SetDirty(_cornerMeshModuleData);
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

        private void MirrorMeshXAxis(Mesh _mesh)
        {
            var _vertices = new Vector3[_mesh.vertices.Length];

            for (int _j = 0; _j < _vertices.Length; _j++)
            {
                var _point = _mesh.vertices[_j];

                var _newPoint = new Vector3(_point.x * -1, _point.y, _point.z);

                _vertices[_j] = _newPoint;
            }
            
            _mesh.vertices = _vertices;
            
            var _triangles = _mesh.triangles;
            Array.Reverse(_triangles);
            _mesh.triangles = _triangles;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
        
        private GameObject CreateNewMesh(Mesh _mesh, string _name, 
            out MeshFilter _meshFilter, 
            out MeshRenderer _renderer,
            out VolumePoint _volumePoint,
            out ModulePrototype _modulePrototype)
        {
            var _gameObject = new GameObject(_name);
                
            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            _renderer   = _gameObject.AddComponent<MeshRenderer>();
            
            _volumePoint     = _gameObject.AddComponent<VolumePoint>();
            _modulePrototype = _gameObject.AddComponent<ModulePrototype>();
        
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

        public int MirrorBitXAis(int _bit)
        {
            var _upperNorthEastBit = _bit & 0b_0000_0001;
            var _upperNorthWestBit = _bit & 0b_0000_0010;

            var _temp = _upperNorthEastBit;
            
            _upperNorthEastBit = _upperNorthWestBit >> 1;
            _upperNorthWestBit = _temp << 1;

            var _upperSouthWestBit = _bit & 0b_0000_0100;
            var _upperSouthEastBit = _bit & 0b_0000_1000;
            
            _temp = _upperSouthWestBit;
            
            _upperSouthWestBit = _upperSouthEastBit >> 1;
            _upperSouthEastBit = _temp << 1;
            
            var _lowerNorthEastBit = _bit & 0b_0001_0000;
            var _lowerNorthWestBit = _bit & 0b_0010_0000;
            
            _temp = _lowerNorthEastBit;
            
            _lowerNorthEastBit = _lowerNorthWestBit >> 1;
            _lowerNorthWestBit = _temp << 1;
            
            var _lowerSouthWestBit = _bit & 0b_0100_0000;
            var _lowerSouthEastBit = _bit & 0b_1000_0000;
            
            _temp = _lowerSouthWestBit;
            
            _lowerSouthWestBit = _lowerSouthEastBit >> 1;
            _lowerSouthEastBit = _temp << 1;

            return _upperNorthEastBit + _upperNorthWestBit + _upperSouthWestBit + _upperSouthEastBit +
                   _lowerNorthEastBit + _lowerNorthWestBit + _lowerSouthWestBit + _lowerSouthEastBit;
        }
        
#if UNITY_EDITOR
        public void SetShowGizmos(bool _value)
        {
            foreach (var _prototype in AllPrototypes)
            {
                _prototype.SetShowGizmos  (_value);
                _prototype.SetShowPivot   (_value);
                _prototype.SetShowBoundBox(_value);
                
                EditorUtility.SetDirty(_prototype);
            }
        }

        public void SetShowDebugText(bool _value)
        {
            foreach(var _prototype in AllPrototypes)
                _prototype.SetShowDebugText(_value);
        }
#endif
    }
}
