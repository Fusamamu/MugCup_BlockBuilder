using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class BitUtil
    {
        
#if UNITY_EDITOR
        public static void Log8Bit(int _bit)
        {
            var _bitText = $"{Convert.ToString(_bit, 2).PadLeft(8, '0').Insert(4, "_")}";
            Debug.Log($"<color=yellow>[Bit result]</color> : {_bitText}");
        }
        
        public static void Log4Bit(short _bit)
        {
            var _bitText = $"{Convert.ToString(_bit, 2).PadLeft(4, '0')}";
            Debug.Log($"<color=yellow>[Bit result]</color> : {_bitText}");
        }

        public static string Get4BitTextFormat(short _bit)
        {
            return  $"{Convert.ToString(_bit, 2).PadLeft(4, '0')}";
        }
 #endif
        
        public static short GetForwardFaceBit(int _bit)
        {
            var _upperBit = _bit & 0b_0000_0011;
            
            var _a = (_bit & 0b_0001_0000) << 1;
            var _b = (_bit & 0b_0010_0000) >> 1;
            var _lowerBit = (_a + _b) >> 2;

            return (short)(_upperBit + _lowerBit);
        }

        public static short GetRightFaceBit(int _bit)
        {
            var _a = (_bit & 0b_0000_1000) >> 3;
            var _b = (_bit & 0b_0000_0001) << 1;
            var _c = (_bit & 0b_0001_0000) >> 2;
            var _d = (_bit & 0b_1000_0000) >> 4;

            return (short)(_a + _b + _c + _d);
        }
        
        public static short GetBackFaceBit(int _bit)
        {
            var _a = (_bit & 0b_0000_0100) >> 2;
            var _b = (_bit & 0b_0000_1000) >> 2;
            var _c = (_bit & 0b_1000_0000) >> 5;
            var _d = (_bit & 0b_0100_0000) >> 3;

            return (short)(_a + _b + _c + _d);
        }
        
        public static short GetLeftFaceBit(int _bit)
        {
            var _a = (_bit & 0b_0000_0010) >> 1;
            var _b = (_bit & 0b_0000_0100) >> 1;
            var _c = (_bit & 0b_0100_0000) >> 4;
            var _d = (_bit & 0b_0010_0000) >> 2;

            return (short)(_a + _b + _c + _d);
        }
        
        public static short GetUpFaceBit(int _bit)
        {
            return (short)(_bit & 0b_0000_1111);
        }
        
        public static short GetDownFaceBit(int _bit)
        {
            return (short)((_bit & 0b_1111_0000) >> 4);
        }

        public static short FlipHorizontalFaceBIt(short _bit)
        {
            var _a = (_bit & 0b_0001) << 1;
            var _b = (_bit & 0b_0010) >> 1;
            var _c = (_bit & 0b_0100) << 1;
            var _d = (_bit & 0b_1000) >> 1;
             
            return (short)(_a + _b + _c + _d);
        }
        
        public static int ShiftBit(int _bit)
        {
            var _upperBit = _bit & 0b_0000_1111;
            var _lowerBit = _bit & 0b_1111_0000;

            _upperBit <<= 1;

            var _checkBit = _upperBit & (1 << 4);

            if (_checkBit == 0b_0001_0000)
            {
                _upperBit -= 0b_0001_0000;
                _upperBit += 0b_0000_0001;
            }

            _lowerBit <<= 1;

            _checkBit = _lowerBit & 1 << 8;
            
            if (_checkBit == 0b_1_0000_0000)
            {
                _lowerBit -= 0b_1_0000_0000;
                _lowerBit += 0b_0_0001_0000;
            }

            _bit = _upperBit | _lowerBit;

            return _bit;
        }

        public static int MirrorBitXAis(int _bit)
        {
            var _upperNorthEastBit = _bit & 0b_0000_0001;
            var _upperNorthWestBit = _bit & 0b_0000_0010;

            var _temp = _upperNorthEastBit;
            
            _upperNorthEastBit = _upperNorthWestBit >> 1;
            _upperNorthWestBit = _temp << 1;

            var _upperSouthWestBit = _bit & 0b_0000_0100;
            var _upperSouthEastBit = _bit & 0b_0000_1000;
            
            _temp = _upperSouthWestBit;
            
            _upperSouthWestBit = _upperSouthEastBit >> 1;
            _upperSouthEastBit = _temp << 1;
            
            var _lowerNorthEastBit = _bit & 0b_0001_0000;
            var _lowerNorthWestBit = _bit & 0b_0010_0000;
            
            _temp = _lowerNorthEastBit;
            
            _lowerNorthEastBit = _lowerNorthWestBit >> 1;
            _lowerNorthWestBit = _temp << 1;
            
            var _lowerSouthWestBit = _bit & 0b_0100_0000;
            var _lowerSouthEastBit = _bit & 0b_1000_0000;
            
            _temp = _lowerSouthWestBit;
            
            _lowerSouthWestBit = _lowerSouthEastBit >> 1;
            _lowerSouthEastBit = _temp << 1;

            return _upperNorthEastBit + _upperNorthWestBit + _upperSouthWestBit + _upperSouthEastBit +
                   _lowerNorthEastBit + _lowerNorthWestBit + _lowerSouthWestBit + _lowerSouthEastBit;
        }
    }
}
