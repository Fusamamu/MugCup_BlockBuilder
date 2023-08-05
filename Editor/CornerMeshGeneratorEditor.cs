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

        private bool displayDebug;
        
        [SerializeField] public int testBit;

        private void OnEnable()
        {
            cornerMeshGenerator = (CornerMeshGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical("GroupBox");
            
            EditorGUILayout.LabelField("Corner mesh generator");
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
                cornerMeshGenerator.GenerateCornerMeshes();
            
            if (GUILayout.Button("Clear All"))
                cornerMeshGenerator.ClearGeneratedMeshes();
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Create and save corner mesh module data"))
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
                
                cornerMeshGenerator
                    .StoreModulesPossibleNeighbors()
                    .SaveCornerMeshModuleData(_targetPath);
                
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndVertical();

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
            
            displayDebug = EditorGUILayout.Foldout(displayDebug, "Debug");
            if (displayDebug)
            {
                testBit = EditorGUILayout.IntField("Test Bit", testBit);

                if (GUILayout.Button("Mirror Bit"))
                {
                    testBit = BitUtil.MirrorBitXAis(testBit);
                
                    Debug.Log($"{Convert.ToString(testBit, 2).PadLeft(8, '0').Insert(4, "_")}");
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
            }
        }
    }
}
#endif
