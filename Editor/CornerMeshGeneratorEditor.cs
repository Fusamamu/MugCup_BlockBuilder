#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Tools;

namespace MugCup_BlockBuilder.Editor
{
    [CustomEditor(typeof(CornerMeshGenerator))]
    public class CornerMeshGeneratorEditor : UnityEditor.Editor
    {
        private CornerMeshGenerator cornerMeshGenerator;

        private static bool showGizmos;
        private static bool showDebugText;
        private static bool showBitInBinary;

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
                
                cornerMeshGenerator.SaveAllGeneratedMeshes  (_generatedMeshFolderPath);
                cornerMeshGenerator.SaveModules             (_prototypeDataFolderPath);
                cornerMeshGenerator.SaveCornerMeshData      (_targetPath);
                
                cornerMeshGenerator
                    .StoreModulesPossibleNeighbors()
                    .SaveCornerMeshModuleData(_targetPath);
                
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

            EditorGUI.BeginChangeCheck();
            
            showGizmos      = EditorGUILayout.Toggle("Show Gizmos"       , showGizmos     );
            showDebugText   = EditorGUILayout.Toggle("Show Debug Text"   , showDebugText  );
            showBitInBinary = EditorGUILayout.Toggle("Show Bit In Binary", showBitInBinary);
            
            if (EditorGUI.EndChangeCheck())
            {
                cornerMeshGenerator.SetShowGizmos     (showGizmos);
                cornerMeshGenerator.SetShowDebugText  (showDebugText);
                cornerMeshGenerator.SetShowBitInBinary(showBitInBinary);
            }

            testBit = EditorGUILayout.IntField("Test Bit", testBit);

            if (GUILayout.Button("Mirror Bit"))
            {
                testBit = BitUtil.MirrorBitXAis(testBit);
                
                Debug.Log($"{Convert.ToString(testBit, 2).PadLeft(8, '0').Insert(4, "_")}");
            }
        }
    }
}
#endif
