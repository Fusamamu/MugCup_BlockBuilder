#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MugCup_BlockBuilder
{
	public static class GridDataSettingGUI
	{
		private const string resourcesFolderPath = "Assets/Resources/";
		private const string generatedTextureName = "GeneratedTexture";
		
		private static GUIContent[] contents = new GUIContent[]
		{
			new GUIContent("Row"), new GUIContent("Column"), new GUIContent("Height")
		};
	   
		public static void Display()
        {
            BBEditorManager.InterfaceSetting.MapSettingFoldout 
                = BBEditorStyling.DrawHeader(Color.clear, "Grid Data Setting", BBEditorManager.InterfaceSetting.MapSettingFoldout);

            if (BBEditorManager.InterfaceSetting.MapSettingFoldout)
            {
                GUILayout.BeginVertical("GroupBox");
                    
                EditorGUILayout.LabelField("Map size setting");
                    
                GUILayout.BeginVertical("GroupBox");
                    
                if (BBEditorManager.MapTextureDataSettingSo.UseTextureAsGridSizeSetting)
	                EditorGUILayout.HelpBox($"Grid data setting has been overriden by texture size.", MessageType.Info);
                
                GUILayout.BeginHorizontal();

                var _rect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
                var _fieldRect = _rect;
                _fieldRect.xMin = 150;
                    
                EditorGUI.LabelField(_rect, new GUIContent("Map Size", "Set map size here"));
                EditorGUI.MultiIntField(_fieldRect, contents, BBEditorManager.GridDataSettingSo.MapSizeArray);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();

                if (BBEditorManager.MapTextureDataSettingSo.UseTextureAsGridSizeSetting)
                {
	                if (BBEditorManager.MapTextureDataSettingSo.GeneratedTexture != null)
	                {
		                var _row    = BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.height;
		                var _column = BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.width;
		                var _height = BBEditorManager.MapTextureDataSettingSo.TerrainLevel + 3;
		                BBEditorManager.GridDataSettingSo.GridUnitSize = new Vector3Int(_row, _height, _column);
	                }
                }
                    
                var _rect1 = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
                var _fieldRect1 = _rect1;
                _fieldRect1.xMin = 150;
                    
                EditorGUI.LabelField(_rect1, new GUIContent("Grid Unit Size", "Set grid unit size here"));
                EditorGUI.MultiIntField(_fieldRect1, contents, BBEditorManager.GridDataSettingSo.GridUnitSizeArray);
                    
                GUILayout.EndHorizontal();
                
                BBEditorManager.GridDataSettingSo.GridOffset = EditorGUILayout.IntField("Grid Offset", BBEditorManager.GridDataSettingSo.GridOffset);
                        
                GUILayout.EndVertical();
                
                EditorGUILayout.LabelField("Terrain Map Generator");
                
                BBEditorManager.MapTextureDataSettingSo.UseTextureAsGridSizeSetting  = EditorGUILayout.Toggle("Use texture size as grid setting", BBEditorManager.MapTextureDataSettingSo.UseTextureAsGridSizeSetting);
                
                BBEditorManager.MapTextureDataSettingSo.TextureWidth  = EditorGUILayout.IntField("Width", BBEditorManager.MapTextureDataSettingSo.TextureWidth);
                BBEditorManager.MapTextureDataSettingSo.TextureHeight = EditorGUILayout.IntField("Height", BBEditorManager.MapTextureDataSettingSo.TextureHeight);
                
                BBEditorManager.MapTextureDataSettingSo.TerrainLevel        = EditorGUILayout.IntField  ("Terrain Level"       , BBEditorManager.MapTextureDataSettingSo.TerrainLevel       );
                BBEditorManager.MapTextureDataSettingSo.NoiseScale          = EditorGUILayout.FloatField("Noise Scale"         , BBEditorManager.MapTextureDataSettingSo.NoiseScale         );
                BBEditorManager.MapTextureDataSettingSo.NoiseFrequency      = EditorGUILayout.FloatField("Noise Frequency"     , BBEditorManager.MapTextureDataSettingSo.NoiseFrequency     );
                BBEditorManager.MapTextureDataSettingSo.NoiseRedistribution = EditorGUILayout.FloatField("Noise Redistribution", BBEditorManager.MapTextureDataSettingSo.NoiseRedistribution);
                BBEditorManager.MapTextureDataSettingSo.NoiseSeed           = EditorGUILayout.IntField  ("Noise Seed"          , BBEditorManager.MapTextureDataSettingSo.NoiseSeed          );

                if (GUILayout.Button("Generate Noise Texture by Levels"))
                {
	                BBEditorManager.MapTextureDataSettingSo.GeneratedTexture = TextureGenerator.GeneratePerlinNoiseTextureByLevels
	                (
		                BBEditorManager.MapTextureDataSettingSo.TextureWidth, 
		                BBEditorManager.MapTextureDataSettingSo.TextureHeight, 
		                BBEditorManager.MapTextureDataSettingSo.NoiseScale,
		                BBEditorManager.MapTextureDataSettingSo.NoiseFrequency,
		                BBEditorManager.MapTextureDataSettingSo.NoiseRedistribution,
		                BBEditorManager.MapTextureDataSettingSo.TerrainLevel
	                );
                }
                
                if (GUILayout.Button("Generate Perlin Noise Texture"))
                {
	                BBEditorManager.MapTextureDataSettingSo.GeneratedTexture = TextureGenerator.GeneratePerlinNoiseTexture
	                (
		                BBEditorManager.MapTextureDataSettingSo.TextureWidth, 
		                BBEditorManager.MapTextureDataSettingSo.TextureHeight, 
		                BBEditorManager.MapTextureDataSettingSo.NoiseScale,
		                BBEditorManager.MapTextureDataSettingSo.NoiseFrequency,
		                BBEditorManager.MapTextureDataSettingSo.NoiseRedistribution
	                );
                }
                
                if (GUILayout.Button("Generate Falloff Texture"))
                {
	                BBEditorManager.MapTextureDataSettingSo.GeneratedTexture = TextureGenerator.GenerateFalloffTexture
	                (
		                BBEditorManager.MapTextureDataSettingSo.TextureWidth, 
		                BBEditorManager.MapTextureDataSettingSo.TextureHeight
	                );
                }

                if (GUILayout.Button("Save Texture"))
                {
	                if (BBEditorManager.MapTextureDataSettingSo.GeneratedTexture != null)
						SaveTextureAsAsset(BBEditorManager.MapTextureDataSettingSo.GeneratedTexture);
                }

                if (GUILayout.Button("Generate Block from Texture"))
                {
	                var _parent = new GameObject("Parent");

	                for (var _i = 0; _i < BBEditorManager.MapTextureDataSettingSo.TerrainLevel - 1; _i++)
	                {
		                var _targetGridPos = TextureGenerator.GetSolidGridPosFromTexture(
			                BBEditorManager.MapTextureDataSettingSo.GeneratedTexture, _i,
			                BBEditorManager.MapTextureDataSettingSo.TerrainLevel);
		                
		                foreach (var _pos in _targetGridPos)
		                {
			                var _newBlock = Object.Instantiate(AssetManager.AssetCollection.DefaultBlock, _pos, Quaternion.identity, _parent.transform);
		                }
	                }
                }

                if (GUILayout.Button("Populate Tree on Terrain"))
                {
	                foreach (var _result in TextureGenerator.ReadGrayScaleFromTexture(BBEditorManager.MapTextureDataSettingSo.GeneratedTexture))
	                {
		                if (_result.Item2 > 0f)
		                {
			                //Object.Instantiate(AssetManager.AssetCollection.DefaultBlock, _pos, Quaternion.identity, _parent.transform);
		                }
	                }
                }

                if (BBEditorManager.MapTextureDataSettingSo.GeneratedTexture != null)
                {
	                EditorGUILayout.Space(10);
	                EditorGUILayout.LabelField("Generated Texture:");

	                //Need ability to change filter
	                BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.filterMode = FilterMode.Point;
	                
	                Rect _previewRect = GUILayoutUtility.GetAspectRect(BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.width / (float)BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.height);
	                EditorGUI.DrawPreviewTexture(_previewRect, BBEditorManager.MapTextureDataSettingSo.GeneratedTexture);
                }
                
                GUILayout.EndVertical();
            }

            Undo.RecordObject(BBEditorManager.GridDataSettingSo,"Undo");
        }
		
		private static void SaveTextureAsAsset(Texture2D _texture)
		{
			string _filePath = resourcesFolderPath + generatedTextureName + ".asset";

			// Make sure the Resources folder exists
			if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
			{
				AssetDatabase.CreateFolder("Assets", "Resources");
			}

			// Save the texture as an asset
			AssetDatabase.CreateAsset(_texture, _filePath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log("Texture saved to Resources folder as: " + _filePath);
		}
	}
}
#endif
