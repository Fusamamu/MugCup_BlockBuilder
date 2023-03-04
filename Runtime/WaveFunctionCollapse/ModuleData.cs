using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [DisallowMultipleComponent]
    public class ModuleData : MonoBehaviour
    {
        public static CornerMeshModuleData Data;

        [SerializeField] private CornerMeshModuleData CornerMeshModuleData;
        
        public short[][] InitialModuleHealth { get; private set; }

        private void Awake()
        {
            Initialized();
        }

        public void Initialized()
        {
            Data = CornerMeshModuleData;

            InitialModuleHealth = CreateInitialModuleHealth(Data.Modules);
        }

        public void StoreCornerMeshModuleData(CornerMeshModuleData _data)
        {
            CornerMeshModuleData = _data;
        }
        
        public void StoreModulesPossibleNeighbors()
        {
            var _modulesInScene = CornerMeshModuleData.Modules.Where(_module => _module != null).ToList();
            
            foreach (var _module in _modulesInScene)
            {
                _module.StorePossibleNeighbors(_modulesInScene);
            }
        }
        
        private short[][] CreateInitialModuleHealth(Module[] _modules) 
        {
            var _initialModuleHealth = new short[6][];

            foreach (ModuleFace _face in Enum.GetValues(typeof(ModuleFace)))
            {
                var _faceIndex         = (int)_face;
                var _oppositeFaceIndex = (int)Orientations.GetOppositeFace(_face);
                
                _initialModuleHealth[_faceIndex] = new short[_modules.Length];
                
                foreach (var _module in _modules) 
                {
                    if (_module == null)
                        continue;
                    
                    foreach (var _neighbor in _module.PossibleNeighbors[_oppositeFaceIndex])
                    {
                        _initialModuleHealth[_faceIndex][_neighbor.Index]++;
                    }
                }
            }
		
            return _initialModuleHealth;
        }
        
        public short[][] CopyInitialModuleHealth()
        {
            return InitialModuleHealth.Select(_a => _a.ToArray()).ToArray();
        }
        
#if UNITY_EDITOR
        public void DebugCornerMeshModuleData()
        {
            GUIStyle _style = new GUIStyle
            {
                richText = true, 
                //padding  = new RectOffset(10, 10, 10, 10)
            };

            string _info = string.Empty;

            if (CornerMeshModuleData == null)
                _info = $"Missing Corner Mesh Module Data";
            else
            {
                if (CornerMeshModuleData.Modules is { Length: > 0 })
                {
                    _info = $"<color=yellow>[Module Count]</color> : {CornerMeshModuleData.Modules.Length}";
                }
            }
            
            EditorGUILayout.LabelField(_info, _style);
        }

        public void DebugModuleHealth()
        {
            
        }
#endif
    }
}
