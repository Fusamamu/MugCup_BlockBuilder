using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class TextureGenerator
    {
        public static Texture2D GeneratePerlinNoiseTexture(int _width, int _height, float _scale, float _frequency, float _redistribution, int _level, int _seed)
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

		public static IEnumerable<Vector3Int> GetSolidGridPosFromTexture(Texture2D _texture, int _level, int _levels)
		{
			Color[] _pixels = _texture.GetPixels();

			int _height = _texture.height;
			int _width  = _texture.width;
			
			for (var _y = 0; _y < _height; _y++)
				for (var _x = 0; _x < _width; _x++)
				{
					byte _grayscale = (byte)(_pixels[_y * _width + _x].grayscale * 255);

					if (CheckLevel(_grayscale, _level, _levels))
						yield return new Vector3Int(_x, _level, _y); 
				}
		}

		public static IEnumerable<(Vector2Int, byte)> ReadGrayScaleFromTexture(Texture2D _texture)
		{
			Color[] _pixels = _texture.GetPixels();

			int _height = _texture.height;
			int _width  = _texture.width;
			
			for (var _y = 0; _y < _height; _y++)
				for (var _x = 0; _x < _width; _x++)
					yield return (new Vector2Int(_x, _y), (byte)(_pixels[_y * _width + _x].grayscale * 255));
		}
		
		private static bool CheckLevel(byte _grayScale, int _level, int _levels)
		{
			var _checkedLevel = GetGrayScaleLevels(_levels)[_level];
				
			if (_grayScale > _checkedLevel)
				return true;

			return false;
		}
		
		private static byte[] GetGrayScaleLevels(int _levels)
		{
			byte[] _levelValues = new byte[_levels + 1];
			
			for (var _i = 0; _i <= _levels; _i++)
				_levelValues[_i] = (byte)(255 * _i / _levels);

			return _levelValues;
		}
		
		private static Texture2D GenerateGrayscaleTexture(int _width, int _height)
		{
			Texture2D _texture = new Texture2D(_width, _height);

			Color32[] _pixels = new Color32[_width * _height];
			
			for (var _y = 0; _y < _height; _y++)
			{
				for (var _x = 0; _x < _width; _x++)
				{
					byte _grayscaleValue = (byte)(255 * (_x / (float)_width));
					
					_pixels[_y * _width + _x] = new Color32(_grayscaleValue, _grayscaleValue, _grayscaleValue, 255);
				}
			}

			_texture.SetPixels32(_pixels);
			_texture.Apply();

			return _texture;
		}
    }
}
