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
