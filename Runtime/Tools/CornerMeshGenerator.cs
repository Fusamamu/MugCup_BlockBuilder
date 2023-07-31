#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using BlockBuilder.Core.Scriptable;

namespace MugCup_BlockBuilder.Runtime
{
    public class CornerMeshGenerator : MonoBehaviour
    {
        [Serializable]
        public class CornerObjectData
        {
            public GameObject GameObject;

            public Mesh         Mesh;
            public MeshFilter   MeshFilter;
            public MeshRenderer MeshRenderer;

            public VolumePoint     VolumePoint;
            public ModulePrototype ModulePrototype;
        }

        public BaseModuleSetSo BaseModuleSetSo;
        
        [SerializeField] private Material DefaultMaterial;

        private const string targetSavePath          = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Meshes/Corner Meshes/GeneratedCornerMesh/";
        private const string volumePointPrefabFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Prefabs/VolumePoints";

        [SerializeField] private int XInterval = 2;
        [SerializeField] private int ZInterval = 2;

        [SerializeField] private List<CornerObjectData> AllCornerObjectData = new List<CornerObjectData>();
   
        [SerializeField] public List<Module> AllModules;

        private readonly HashSet<string> generatedCornerDataTracker = new HashSet<string>();

        public void ClearGeneratedMeshes()
        {
            foreach (var _cornerObjectData in AllCornerObjectData)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(_cornerObjectData.GameObject);
            }

            ClearData();
            
            EditorUtility.SetDirty(this);
        }
        
        private void ClearData()
        {
            AllCornerObjectData = new List<CornerObjectData>();
            AllModules = new List<Module>();
            
            generatedCornerDataTracker.Clear();
        }
        
        public void AddPrototypeDataIntoCornerMeshData()
        {
            var _cornerMeshData = AssetDatabase.LoadAssetAtPath<CornerMeshData>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/CornerMeshData/NewCornerMeshData.asset");
          
            foreach (var _prototype in AllModules)
                _cornerMeshData.AddModuleData(_prototype.BitMask, _prototype);
            
            EditorUtility.SetDirty(_cornerMeshData);
        }

        public void UpdatePrototypesData()
        {
            foreach (var _cornerObjectData in AllCornerObjectData)
                _cornerObjectData.ModulePrototype.TryUpdateData();
        }
        
        public void GenerateCornerMeshes()
        {
            ClearData();

            var _baseModuleGroupTable = BaseModuleSetSo.GetBaseModuleGroupTable();

            var _zIntervalIndex = 0;
            
            foreach (var _baseModuleGroup in _baseModuleGroupTable)
            {
                var _group          = _baseModuleGroup.Key;
                var _baseModuleList = _baseModuleGroup.Value;

                var _xIntervalIndex = 0;
                
                foreach (var _baseModule in _baseModuleList)
                {
                    if (_baseModule.MeshFilterPrototype == null)
                        continue;

                    var _bitMask    = _baseModule.BitMask;
                    var _metaData   = _baseModule.MetaData;
                    var _targetMesh = _baseModule.MeshFilterPrototype.sharedMesh;
                    var _isFlip     = _baseModule.HasFlipXVariant;
                    var _isRotated  = _baseModule.HasRotateVariant;
                   
                    var _meshName = NumberUtil.GetMetaDataName(_metaData);
                
                    var _cloneMesh = CloneMesh(_targetMesh);
                    _cloneMesh.name = _meshName;
                        
                    var _targetPos = new Vector3(_xIntervalIndex * XInterval, 0, _zIntervalIndex * ZInterval);
                        
                    CreateNewCornerObjectData(_cloneMesh, _meshName, _bitMask, _targetPos);
                        
                    if(!generatedCornerDataTracker.Contains(_metaData))
                        generatedCornerDataTracker.Add(_metaData);
                        
                    _xIntervalIndex++;

                    for (var _i = 1; _i < 4; _i++)
                    {
                        _bitMask  = _isRotated ? BitUtil.ShiftBit(_bitMask) : _bitMask;
                        //_metaData = _isFlip ? _metaData : _metaData;
                        _metaData = _isRotated ? NumberUtil.ShiftMetaDataWhenRotated(_metaData) : _metaData;

                        if (generatedCornerDataTracker.Contains(_metaData))
                        {
                            _xIntervalIndex++;
                            continue;
                        }
                        
                        _meshName = NumberUtil.GetMetaDataName(_metaData);
                        
                        _cloneMesh = CloneMesh(_targetMesh);
                        _cloneMesh.name = _meshName;

                        if (_isFlip)
                            MirrorMeshXAxis(_cloneMesh);

                        if (_isRotated)
                            RotateMesh(_cloneMesh, _baseModule.MeshFilterPrototype.transform.position, new Vector3(0, _i * -90, 0));
                        
                        _targetPos = new Vector3(_xIntervalIndex * XInterval, 0, _zIntervalIndex * ZInterval);
                        
                        CreateNewCornerObjectData(_cloneMesh, _meshName, _bitMask, _targetPos);
                        
                        if(!generatedCornerDataTracker.Contains(_metaData))
                            generatedCornerDataTracker.Add(_metaData);
                        
                        _xIntervalIndex++;
                    }
                }

                _zIntervalIndex++;
            }

            //This is for WFC will Refactor to somewhere else 
            //init all index to module prototype
            // var _index = 0;
            // foreach (var _prototype in AllPrototypes)
            // {
            //     _prototype.Index = _index;
            //     _index++;
            // }
            //
            // foreach (var _prototype in AllPrototypes)
            // {
            //     _prototype.StorePossibleNeighbors(AllPrototypes);
            // }
        }

        private CornerObjectData CreateNewCornerObjectData(Mesh _mesh, string _name, int _bitMask, Vector3 _targetPos)
        {
            var _gameObject = new GameObject(_name)
            {
                transform = { position = _targetPos }
            };
                
            var _meshFilter = _gameObject.AddComponent<MeshFilter>();
            var _renderer   = _gameObject.AddComponent<MeshRenderer>();
            
            var _volumePoint     = _gameObject.AddComponent<VolumePoint>();
            var _modulePrototype = _gameObject.AddComponent<ModulePrototype>();
        
            _meshFilter.sharedMesh = _mesh;
            _renderer  .material   = DefaultMaterial;
            
            _volumePoint    .SetBitMask(_bitMask);
            _modulePrototype.SetBitMask(_bitMask);
            
            var _cornerObjectData = new CornerObjectData
            {
                GameObject      = _gameObject,
                Mesh            = _mesh,
                MeshFilter      = _meshFilter,
                MeshRenderer    = _renderer,
                VolumePoint     = _volumePoint,
                ModulePrototype = _modulePrototype
            };

            AllCornerObjectData.Add(_cornerObjectData);

            return _cornerObjectData;
        }

        private List<GameObject> GenerateCornerMeshes_ByRotate_CounterClockwise(int _bit, MeshFilter _sourceMesh)
        {
            var _volumePoints = new List<GameObject>();
            
            for (var _i = 0; _i < 4; _i++)
            {
                var _xRot = 0;
                var _yRot = _i * -90;
                var _zRot = 0;

                //--------------------------------//
                var _meshName = $"CM_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
                
                var _cloneMesh = CloneMesh(_sourceMesh.sharedMesh);
                _cloneMesh.name = _meshName;
                //--------------------------------//

                RotateMesh(_cloneMesh, _sourceMesh.transform.position, new Vector3(_xRot, _yRot, _zRot));

                CornerObjectData _newMeshObject = null;
                
                _newMeshObject.VolumePoint.SetBitMask(_bit);
                _newMeshObject.ModulePrototype
                    .SetBitMask(_bit)
                    .UpdateFaceBits();
                
                // if (generatedMeshTracker.ContainsKey(_bit))
                //     generatedMeshTracker[_bit] = true;
                
                //-------------------------------//
                _bit = BitUtil.ShiftBit(_bit);
                //-------------------------------//
                
                _volumePoints.Add(_newMeshObject.GameObject);
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

                CornerObjectData _newMeshObject = null;
        
                _newMeshObject.VolumePoint.SetBitMask(_bit);
                _newMeshObject.ModulePrototype  
                    .SetBitMask(_bit)
                    .UpdateFaceBits();
        
                // if (generatedMeshTracker.ContainsKey(_bit))
                //     generatedMeshTracker[_bit] = true;
               
                _bit = BitUtil.ShiftBit(_bit);
                
                _volumePoints.Add(_newMeshObject.GameObject);
             
            }
        
            return _volumePoints;
        }
 
#region Save Data Section
        public void SaveAllGeneratedMeshes(string _targetFolderPath)
        {
            var _allGeneratedMeshes = AllCornerObjectData.Select(_o => _o.Mesh).ToList();
            
            if (_allGeneratedMeshes is { Count: > 0 })
            {
                foreach (var _mesh in _allGeneratedMeshes)
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

            var _allPrototypes = AllCornerObjectData.Select(_o => _o.ModulePrototype);
            
            foreach (var _prototype in _allPrototypes)
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
                _cornerMeshData.AddModuleData(_module.BitMask, _module);
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
#endregion
        
#region Mesh Modification
        private static void RotateMesh(Mesh _mesh, Vector3 _pivot, Vector3 _angle)
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

        private static void MirrorMeshXAxis(Mesh _mesh)
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

        private static Mesh CloneMesh(Mesh _mesh, bool _isRotated = false, bool _isFlip = false)
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
        
        private static Vector3 RotatePointAroundPivot(Vector3 _point, Vector3 _pivot, Vector3 _angles)
        {
            return Quaternion.Euler(_angles) * (_point - _pivot) + _pivot;
        }
#endregion

#region Gizmos
        public void SetShowGizmos(bool _value)
        {
            var _allPrototypes = AllCornerObjectData.Select(_o => _o.ModulePrototype);
            
            foreach (var _prototype in _allPrototypes)
            {
                _prototype.SetShowGizmos  (_value);
                _prototype.SetShowPivot   (_value);
                _prototype.SetShowBoundBox(_value);
                _prototype.SetShowFaceBits(_value);
                
                EditorUtility.SetDirty(_prototype);
            }
        }

        public void SetShowDebugText(bool _value)
        {
            var _allPrototypes = AllCornerObjectData.Select(_o => _o.ModulePrototype);
            
            foreach(var _prototype in _allPrototypes)
                _prototype.SetShowDebugText(_value);
        }

        public void SetShowBitInBinary(bool _value)
        {
            var _allPrototypes = AllCornerObjectData.Select(_o => _o.ModulePrototype);
            
            foreach (var _prototype in _allPrototypes)
            {
                _prototype.SetShowBitInBinary(_value);
            }
        }
#endregion
    }
}
#endif
