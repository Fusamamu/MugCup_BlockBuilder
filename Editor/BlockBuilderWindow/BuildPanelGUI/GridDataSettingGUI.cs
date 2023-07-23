#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Editor;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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
                    
                GUILayout.BeginHorizontal();
                
                var _rect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
                var _fieldRect = _rect;
                _fieldRect.xMin = 150;
                    
                EditorGUI.LabelField(_rect, new GUIContent("Map Size", "Set map size here"));
                EditorGUI.MultiIntField(_fieldRect, contents, BBEditorManager.GridDataSettingSo.MapSizeArray);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                    
                var _rect1 = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
                var _fieldRect1 = _rect1;
                _fieldRect1.xMin = 150;
                    
                EditorGUI.LabelField(_rect1, new GUIContent("Grid Unit Size", "Set grid unit size here"));
                EditorGUI.MultiIntField(_fieldRect1, contents, BBEditorManager.GridDataSettingSo.GridUnitSizeArray);
                    
                GUILayout.EndHorizontal();
                
                BBEditorManager.GridDataSettingSo.GridOffset = EditorGUILayout.IntField("Grid Offset", BBEditorManager.GridDataSettingSo.GridOffset);
                        
                GUILayout.EndVertical();
                
                
                EditorGUILayout.LabelField("Terrain Map Generator");
                
                BBEditorManager.MapTextureDataSettingSo.TextureWidth  = EditorGUILayout.IntField("Width", BBEditorManager.MapTextureDataSettingSo.TextureWidth);
                BBEditorManager.MapTextureDataSettingSo.TextureHeight = EditorGUILayout.IntField("Height", BBEditorManager.MapTextureDataSettingSo.TextureHeight);
                
                BBEditorManager.MapTextureDataSettingSo.TerrainLevel        = EditorGUILayout.IntField("Terrain Level", BBEditorManager.MapTextureDataSettingSo.TerrainLevel);
                BBEditorManager.MapTextureDataSettingSo.NoiseScale          = EditorGUILayout.FloatField("Noise Scale", BBEditorManager.MapTextureDataSettingSo.NoiseScale);
                BBEditorManager.MapTextureDataSettingSo.NoiseFrequency      = EditorGUILayout.FloatField("Noise Frequency", BBEditorManager.MapTextureDataSettingSo.NoiseFrequency);
                BBEditorManager.MapTextureDataSettingSo.NoiseRedistribution = EditorGUILayout.FloatField("Noise Redistribution", BBEditorManager.MapTextureDataSettingSo.NoiseRedistribution);
                BBEditorManager.MapTextureDataSettingSo.NoiseSeed           = EditorGUILayout.IntField("Noise Seed", BBEditorManager.MapTextureDataSettingSo.NoiseSeed);

                if (GUILayout.Button("Generate Texture"))
                {
	                BBEditorManager.MapTextureDataSettingSo.GeneratedTexture = GeneratePerlinNoiseTexture
	                (
		                BBEditorManager.MapTextureDataSettingSo.TextureWidth, 
		                BBEditorManager.MapTextureDataSettingSo.TextureHeight, 
		                BBEditorManager.MapTextureDataSettingSo.NoiseScale,
		                BBEditorManager.MapTextureDataSettingSo.NoiseFrequency,
		                BBEditorManager.MapTextureDataSettingSo.NoiseRedistribution,
		                BBEditorManager.MapTextureDataSettingSo.TerrainLevel,
		                BBEditorManager.MapTextureDataSettingSo.NoiseSeed
	                );
                }

                if (GUILayout.Button("Save Texture"))
                {
	                if (BBEditorManager.MapTextureDataSettingSo.GeneratedTexture != null)
						SaveTextureAsAsset(BBEditorManager.MapTextureDataSettingSo.GeneratedTexture);
                }

                if (BBEditorManager.MapTextureDataSettingSo.GeneratedTexture != null)
                {
	                EditorGUILayout.Space(10);
	                EditorGUILayout.LabelField("Generated Texture:");

	                BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.filterMode = FilterMode.Point;
	                
	                Rect _previewRect = GUILayoutUtility.GetAspectRect(BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.width / (float)BBEditorManager.MapTextureDataSettingSo.GeneratedTexture.height);
	                EditorGUI.DrawPreviewTexture(_previewRect, BBEditorManager.MapTextureDataSettingSo.GeneratedTexture);
                }
                
                
                
                
                
                GUILayout.EndVertical();
            }

            Undo.RecordObject(BBEditorManager.GridDataSettingSo,"Undo");
        }
		
		private static Texture2D GeneratePerlinNoiseTexture(int _width, int _height, float _scale, float _frequency, float _redistribution, int _level, int _seed)
		{
			Texture2D _texture = new Texture2D(_width, _height);

			Color32[] _pixels = new Color32[_width * _height];

			Random.InitState(_seed); // Set the seed value for deterministic noise

			for (int _y = 0; _y < _height; _y++)
			{
				for (int _x = 0; _x < _width; _x++)
				{
					float _xCoord = (float)_x / _width * _frequency * _scale;
					float _yCoord = (float)_y / _height * _frequency * _scale;

					float _sample = Mathf.PerlinNoise(_xCoord, _yCoord);

					_sample = Mathf.Pow(_sample, _redistribution);

					byte _grayscaleValue = GetQuantizedGrayscaleValue(_sample, _level);

					_pixels[_y * _width + _x] = new Color32(_grayscaleValue, _grayscaleValue, _grayscaleValue, 255);
				}
			}

			_texture.SetPixels32(_pixels);
			_texture.Apply();

			return _texture;
		}
		private static byte GetQuantizedGrayscaleValue(float _sample, int _levels)
		{
			// Quantize grayscale values based on the specified number of levels
			byte[] _levelValues = new byte[_levels + 1];
			
			for (var _i = 0; _i <= _levels; _i++)
				_levelValues[_i] = (byte)(255 * _i / _levels);

			float _quantizedValue = Mathf.Floor(_sample * _levels);
			byte _grayscaleValue = _levelValues[Mathf.FloorToInt(_quantizedValue)];

			return _grayscaleValue;
		}
		
		private static Texture2D GenerateGrayscaleTexture(int _width, int _height)
		{
			Texture2D texture = new Texture2D(_width, _height);

			Color32[] pixels = new Color32[_width * _height];
			
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					byte grayscaleValue = (byte)(255 * (x / (float)_width));
					pixels[y * _width + x] = new Color32(grayscaleValue, grayscaleValue, grayscaleValue, 255);
				}
			}

			texture.SetPixels32(pixels);
			texture.Apply();

			return texture;
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
