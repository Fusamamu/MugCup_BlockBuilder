using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class NumberUtil 
    {
        public static void GeneratePermutations(string characters, int length, string current, List<string> permutations)
        {
            if (current.Length == length)
            {
                permutations.Add(current);
                return;
            }

            foreach (char c in characters)
            {
                GeneratePermutations(characters, length, current + c, permutations);
            }
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
        
        public static int MirrorDigit(int _number)
        {
            var _a = SeparateDigit(_number, 2);

            var _i = ShiftDigitLeft(_a.Item1, 2);
            var _j = ShiftDigitLeft(_a.Item2, 2);

            return CombineDigit(_i, _j, 2);
        }

        public static int ShiftDigitLeft(int _number, int _digitCount)
        {
            int _divider = (int)Mathf.Pow(10, _digitCount - 1);

            int _firstDigit      = _number / _divider; // Extract the first digit
            int _remainingDigits = _number % _divider; // Extract the remaining three digits
        
            return  _remainingDigits * 10 + _firstDigit; // Shift the number to the left
        }
        
        public static (int, int) SeparateDigit(int _number, int _digitCount)
        {
            int _divider = (int)Mathf.Pow(10, _digitCount);
            
            int _firstFourDigits = _number / _divider; // Extract the first 4 digits
            int _lastFourDigits  = _number % _divider; // Extract the last 4 digits

            return (_firstFourDigits, _lastFourDigits);
        }

        public static int CombineDigit(int _first, int _second, int _digitCount)
        {
            int _modifier = (int)Mathf.Pow(10, _digitCount);
            
            return  _first * _modifier + _second;
        }        
        
        public static int RearrangeFourDigitNumber(int number)
        {
            // Convert the integer to a string
            string numberStr = number.ToString();

            // Rearrange the characters (digits) in the string
            char[] charArray = numberStr.ToCharArray();
            Array.Sort(charArray);

            // Convert the rearranged character array back to an integer
            string rearrangedNumberStr = new string(charArray);
            int rearrangedNumber = int.Parse(rearrangedNumberStr);

            return rearrangedNumber;
        }
        
        //https://gamedev.stackexchange.com/questions/132363/create-a-seemless-grid-based-polygonal-tileset
        public static int GenerateModuleID(int _metaData)
        {
            int _id = 0;

            for (var _cornerIndex = 0; _cornerIndex < 8; _cornerIndex++)
            {
                var _cornerLabel = GerCornerLabel(_cornerIndex, _metaData);

                _id += _cornerLabel * (int)Mathf.Pow(8, _cornerIndex);
            }

            return _id;
        }

        public static int GerCornerLabel(int _cornerIndex, int _metaData)
        {
            if (_cornerIndex < 0 || _cornerIndex > 7)
                throw new ArgumentOutOfRangeException(nameof(_cornerIndex), "Position must be between 0 and 7.");

            string _numberStr = _metaData.ToString();
            
            char _digitChar = _numberStr[_cornerIndex]; // Adjust position to zero-based index
           
            return int.Parse(_digitChar.ToString());
        }
        
        public static int ReplaceDigitAtPosition(int _cornerIndex, int _metaData,  int _replacementDigit)
        {
            if (_cornerIndex < 0 || _cornerIndex >= 8)
                throw new ArgumentOutOfRangeException(nameof(_cornerIndex), "Index must be between 0 and 7.");

            if (_replacementDigit < 0 || _replacementDigit > 9)
                throw new ArgumentOutOfRangeException(nameof(_replacementDigit), "Replacement digit must be between 0 and 9.");

            string _numberStr   = _metaData.ToString();
            char[] _numberChars = _numberStr.ToCharArray();

            // Update the character at the specified index
            _numberChars[_cornerIndex] = (char)(_replacementDigit + '0');

            // Convert the modified character array back to an integer
            return int.Parse(new string(_numberChars));
        }
    }
}
