using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public static class NumberUtil 
    {
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
                throw new ArgumentOutOfRangeException(nameof(_cornerIndex), "Position must be between 1 and 8.");

            string _numberStr = _metaData.ToString();
            
            char _digitChar = _numberStr[_cornerIndex]; // Adjust position to zero-based index
           
            return int.Parse(_digitChar.ToString());
        }
    }
}
