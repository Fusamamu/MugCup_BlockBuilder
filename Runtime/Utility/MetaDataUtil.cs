using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class MetaDataUtil
    {
        public static string ReplaceMetaTypeAt(char _type, int _index, string _metaData)
        {
            char[] _charArray = _metaData.ToCharArray();
            _charArray[_index] = _type;
            return new string(_charArray);
        }
        
        public static string GetMetaDataName(string _metaData)
        {
            if (_metaData.Length != 8)
                throw new ArgumentException("Input string must have 8 characters.");

            return "M_" + _metaData.Substring(0, 4) + "_" + _metaData.Substring(4);
        }
        
        public static int BinaryStringToInt(string _binaryString)
        {
            return Convert.ToInt32(_binaryString, 2);
        }
        
        public static string TurnMetaDataIntoBinaryStringFormat(string _metaData)
        {
            StringBuilder _result = new ();

            foreach (char _c in _metaData)
            {
                if (_c > '0')
                    _result.Append('1');
                else
                    _result.Append('0');
            }

            return _result.ToString();
        }
        
        public static string MirrorMetaData(string _metaData)
        {
            var _splitData = SeparateStringIfEven(_metaData);

            var _firstPart  = MirrorChar(_splitData.Item1);
            var _secondPart = MirrorChar(_splitData.Item2);

            return _firstPart + _secondPart;
        }
        
        public static string MirrorChar(string _input)
        {
            var _splitString = SeparateStringIfEven(_input);

            var _modifiedFirstPart  = ShiftCharLeft(_splitString.Item1);
            var _modifiedSecondPart = ShiftCharLeft(_splitString.Item2);

            return _modifiedFirstPart + _modifiedSecondPart;
        }

        public static string ShiftMetaDataWhenRotated(string _metaData)
        {
            var _result = SeparateStringIfEven(_metaData);

            var _modifiedFirstPart  = ShiftCharLeft(_result.Item1);
            var _modifiedSecondPart = ShiftCharLeft(_result.Item2);

            return _modifiedFirstPart + _modifiedSecondPart;
        }
        
        public static string ShiftCharLeft(string _metaData)
        {
            if (string.IsNullOrEmpty(_metaData))
                throw new ArgumentException("Input string cannot be null or empty.");

            char   _firstChar    = _metaData[0];
            string _restOfString = _metaData.Substring(1);
           
            return _restOfString + _firstChar;
        }
        
        public static (string, string) SeparateStringIfEven(string _metaData)
        {
            if (string.IsNullOrEmpty(_metaData))
                throw new ArgumentException("Input string cannot be null or empty.");

            int _length = _metaData.Length;

            // Calculate whether the length is even or odd
            bool _isEvenLength = _length % 2 == 0;

            // If the length is even, split the string into two parts
            if (_isEvenLength)
            {
                int _halfLength = _length / 2;
                string _part1 = _metaData.Substring(0, _halfLength);
                string _part2 = _metaData.Substring(_halfLength);
                return (_part1, _part2);
            }
            else
            {
                // Decide what to do if the length is odd (e.g., drop one character or include it in either part)
                // For this example, we drop the last character if the length is odd
                string _part1 = _metaData.Substring(0, _length - 1);
                string _part2 = _metaData[_length - 1].ToString();
                return (_part1, _part2);
            }
        }
    }
}
