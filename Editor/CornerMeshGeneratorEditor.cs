using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Tools;
using UnityEditor.VersionControl;

namespace MugCup_BlockBuilder.Editor
{
    [CustomEditor(typeof(CornerMeshGenerator))]
    public class CornerMeshGeneratorEditor : UnityEditor.Editor
    {
        private CornerMeshGenerator cornerMeshGenerator;

        [SerializeField] private bool ShowGizmos;
        [SerializeField] private bool ShowDebugText;

        private const string TargetFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Package Resources/Meshes/Corner Meshes/Data";
        private const string NewCornerMeshDataFolder = "NewCornerMeshData";

        [SerializeField] public int testBit;

        private void OnEnable()
        {
            cornerMeshGenerator = (CornerMeshGenerator)target;
            
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (GUILayout.Button("Generate Corner Meshes"))
            {
                cornerMeshGenerator.GenerateCornerMeshes();
            }
            
            if (GUILayout.Button("Clear All Corner Meshes"))
            {
                cornerMeshGenerator.ClearGeneratedMeshes();
            }
            
            if (GUILayout.Button("Update Prototypes Data"))
            {
                cornerMeshGenerator.UpdatePrototypesData();
            }
            
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Create New Corner Mesh Data"))
            {
                var _targetPath = $"{TargetFolder}/{NewCornerMeshDataFolder}";
                
                if (!Directory.Exists(_targetPath))
                {
                    Directory.CreateDirectory(_targetPath);
                }
                else
                {
                    _targetPath = AssetDatabase.GenerateUniqueAssetPath(_targetPath);
                    Directory.CreateDirectory(_targetPath);
                }

                var _generatedMeshFolderPath = $"{_targetPath}/GeneratedMeshes";
                var _prototypeDataFolderPath = $"{_targetPath}/PrototypeData";

                Directory.CreateDirectory(_generatedMeshFolderPath);
                Directory.CreateDirectory(_prototypeDataFolderPath);
                
                cornerMeshGenerator.SaveAllGeneratedMeshes(_generatedMeshFolderPath);
                cornerMeshGenerator.SavePrototypes        (_prototypeDataFolderPath);
                cornerMeshGenerator.SaveCornerMeshData    (_targetPath);
                
                AssetDatabase.Refresh();
            }
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Add Prototype Data into CornerMeshData"))
            {
                cornerMeshGenerator.AddPrototypeDataIntoCornerMeshData();
            }

            if (GUILayout.Button("Print All Possible Bit Permutation"))
            {
                var _allBits = CornerMeshPermutation.GetBitMaskPermutation(256);

                var _text = string.Empty;
                foreach (var _bit in _allBits)
                {
                    _text += $"0b_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}\n";
                }

                _text += "\n";
                foreach (var _bit in _allBits)
                {
                    _text += $"B_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}\n";
                }
                
                _text += "\n";
                foreach (var _bit in _allBits)
                {
                    _text += $"P_{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}\n";
                }

                if (!Directory.Exists(Application.dataPath + "/GeneratedText"))
                {
                    Directory.CreateDirectory(Application.dataPath + "/GeneratedText");
                }
                    
                File.WriteAllText(Application.dataPath + "/GeneratedText/AllBitPermutation.txt", _text);
            }

            ShowGizmos    = EditorGUILayout.Toggle("Show Gizmos"    , ShowGizmos   );
            ShowDebugText = EditorGUILayout.Toggle("Show Debug Text", ShowDebugText);
            
            cornerMeshGenerator.SetShowGizmos   (ShowGizmos);
            cornerMeshGenerator.SetShowDebugText(ShowDebugText);

            testBit = EditorGUILayout.IntField("Test Bit", testBit);

            if (GUILayout.Button("Mirror Bit"))
            {
                testBit = cornerMeshGenerator.MirrorBitXAis(testBit);
                
                Debug.Log($"{Convert.ToString(testBit, 2).PadLeft(8, '0').Insert(4, "_")}");
            }
        }
        
        
    }
}
