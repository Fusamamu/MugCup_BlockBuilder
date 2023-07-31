using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MugCup_BlockBuilder;
using NUnit.Framework;
using UnityEditor.VersionControl;

public class Bit_Manipulation_Test : MonoBehaviour
{
    [Test]
    public void Print_8_Bit_Text_Format()
    {
        var _resultText = BitUtil.Get8BitTextFormat(1);

        Assert.AreEqual(_resultText, $"0000_0001");
    }

    [Test]
    public void Shift_Bit_After_Rotate_Around_Y_Axis_CCW()
    {
        var _originBits = new List<int>()
        {
            0b_0000_0001,
            0b_0000_1000,
            0b_0001_0000,
            0b_1000_0000,
            0b_1110_1100
        };
        
        var _expectBits = new List<int>()
        {
            0b_0000_0010,
            0b_0000_0001,
            0b_0010_0000,
            0b_0001_0000,
            0b_1101_1001
        };

        var _resultBits = new List<int>();

        foreach (var _bit in _originBits)
        {
            var _resultBit = BitUtil.ShiftBit(_bit);
            _resultBits.Add(_resultBit);
        }

        var _testSummary = $"<color=green>[ Bit shift test summary ]</color>\n";

        for (var _i = 0; _i < _resultBits.Count; _i++)
        {
            var _originBit = _originBits[_i];
            var _resultBit = _resultBits[_i];
            var _expectBit = _expectBits[_i];

            _testSummary += 
                $"<color=yellow>Origin bit</color> : {BitUtil.Get8BitTextFormat(_originBit)}, " +
                $"<color=yellow>Result bit</color> : {BitUtil.Get8BitTextFormat(_resultBit)}\n";

            Assert.AreEqual(_resultBit, _expectBit);
        }

        Debug.Log(_testSummary);
    }

    [Test]
    public void Mirror_Bit_After_Flip_By_Axis_X()
    {
        var _originBits = new List<int>()
        {
            0b_0000_0001,
            0b_0000_0100,
            0b_0000_0010,
            0b_0000_1000,
        };
        
        var _expectBits = new List<int>()
        {
            0b_0000_0010,
            0b_0000_1000,
            0b_0000_0001,
            0b_0000_0100,
        };
        
        var _resultBits = new List<int>();

        foreach (var _bit in _originBits)
        {
            var _resultBit = BitUtil.MirrorBitXAis(_bit);
            _resultBits.Add(_resultBit);
        }

        var _testSummary = $"<color=green>[ Bit shift test summary ]</color>\n";

        for (var _i = 0; _i < _resultBits.Count; _i++)
        {
            var _originBit = _originBits[_i];
            var _resultBit = _resultBits[_i];
            var _expectBit = _expectBits[_i];

            _testSummary += 
                $"<color=yellow>Origin bit</color> : {BitUtil.Get8BitTextFormat(_originBit)}, " +
                $"<color=yellow>Result bit</color> : {BitUtil.Get8BitTextFormat(_resultBit)}\n";

            Assert.AreEqual(_resultBit, _expectBit);
        }

        Debug.Log(_testSummary);
    }

    [Test]
    public void Shift_Meta_Data_After_Rotate_Around_Y_Axis_CCW()
    {
        var _metaData = 1234;

        var _modifiedMetaData = NumberUtil.ShiftDigitLeft(_metaData, _digitCount: 4);
        
        Assert.AreEqual(_modifiedMetaData, 2341);
    }

    [Test]
    public void Split_8_Digit_Into_4_Digit()
    {
        var _originDigit = 12345678;

        var _result = NumberUtil.SeparateDigit(_originDigit, _digitCount: 4);
      
        Assert.AreEqual(_result.Item1, 1234);
        Assert.AreEqual(_result.Item2, 5678);
        
        Debug.Log($"<Color=green>Origin digit</color> : {_originDigit}\n" +
            $"First 4 digit : {_result.Item1}\n" +
            $"Last  4 digit : {_result.Item2}");
    }

    [Test]
    public void Split_Digit_n_ShiftToLeft_n_Combine_Together()
    {
        var _originDigit = 12345678;

        var _result = NumberUtil.SeparateDigit(_originDigit, _digitCount: 4);

        var _modifiedFirst4Digit = NumberUtil.ShiftDigitLeft(_result.Item1 , _digitCount: 4);
        var _modifiedLast4Digit  = NumberUtil.ShiftDigitLeft(_result.Item2 , _digitCount: 4);

        var _combinedDigit = NumberUtil.CombineDigit(_modifiedFirst4Digit, _modifiedLast4Digit, _digitCount: 4);

        Assert.AreEqual(_combinedDigit, 23416785);
    }

    [Test]
    public void Mirror_4_Digit()
    {
        var _originDigit = 1234;
        var _expectDigit = 2143;

        var _result = NumberUtil.MirrorDigit(_originDigit);

        Assert.AreEqual(_result, _expectDigit);
    }

    [Test]
    public void Get_Specific_Corner_Digit_from_MetaData()
    {
        var _metaData = 12345678;

        var _selectedDigit = NumberUtil.GerCornerLabel(2, _metaData);

        Assert.AreEqual(_selectedDigit, 3);
    }

    [Test]
    public void Generate_Unique_ID_for_Labeled_Vertices()
    {
        var _metaData = 11120000;
        
        var _uniqueID = NumberUtil.GenerateModuleID(_metaData);
        
        Assert.AreEqual(_uniqueID, 1097);
        
        Debug.Log(_uniqueID);
    }

    [Test]
    public void Shift_Char_in_MetaData_to_the_Left()
    {
        string _metaData = "1234";

        var _result = NumberUtil.ShiftCharLeft(_metaData);
        
        Assert.AreEqual(_result, "2341");
    }

    [Test]
    public void Split_8_String_Into_2_Substring()
    {
        string _metaData = "12345678";

        var _result = NumberUtil.SeparateStringIfEven(_metaData);
        
        Assert.AreEqual(_result.Item1, "1234");
        Assert.AreEqual(_result.Item2, "5678");
    }
    
    [Test]
    public void Split_String_n_ShiftToLeft_n_Combine_Together()
    {
        string _metaData = "12345678";
        
        var _result = NumberUtil.SeparateStringIfEven(_metaData);

        var _modifiedFirstPart  = NumberUtil.ShiftCharLeft(_result.Item1);
        var _modifiedSecondPart = NumberUtil.ShiftCharLeft(_result.Item2);

        var _combinedString = _modifiedFirstPart + _modifiedSecondPart;

        Assert.AreEqual(_combinedString, "23416785");
    }
    
    [Test]
    public void Mirror_4_Char()
    {
        var _originDigit = "1234";
        var _expectDigit = "2143";

        var _result = NumberUtil.MirrorChar(_originDigit);

        Assert.AreEqual(_result, _expectDigit);
    }

    [Test]
    public void Mirror_MetaData()
    {
        string _metaData = "12345678";
        
        var _result = NumberUtil.MirrorMetaData(_metaData);

        Assert.AreEqual(_result, "21436587");
    }
}


