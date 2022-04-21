using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Tools
{
    public class CornerMeshPermutation : MonoBehaviour
    {
        private GameObject unitBlock;

        [SerializeField] private float intervalSpace = 4f;

        private Dictionary<string, Vector3> offsetTable = new Dictionary<string, Vector3>();

        private void Start()
        {
            unitBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            unitBlock.transform.localScale *= 0.85f;
            
            InitializeOffsetTable();
            GenerateAllDualBlockPermutations();
            
            DestroyImmediate(unitBlock);
        }

        private void InitializeOffsetTable()
        {
            offsetTable = new Dictionary<string, Vector3>
            {
                { "Q1", new Vector3(  0.5f,  0.5f,  0.5f) },
                { "Q2", new Vector3( -0.5f,  0.5f,  0.5f) },
                { "Q3", new Vector3( -0.5f,  0.5f, -0.5f) },
                { "Q4", new Vector3(  0.5f,  0.5f, -0.5f) },
                { "Q5", new Vector3(  0.5f, -0.5f,  0.5f) },
                { "Q6", new Vector3( -0.5f, -0.5f,  0.5f) },
                { "Q7", new Vector3( -0.5f, -0.5f, -0.5f) },
                { "Q8", new Vector3(  0.5f, -0.5f, -0.5f) },
            };
        }

        private void GenerateAllDualBlockPermutations()
        {
            var _allPossibleBits = GetBitMaskPermutation(256);
            
            var _width = Mathf.Sqrt(256);

            var _currentBitIndex = 0;

            for (var _i = 0; _i < _width; _i++)
            {
                for (var _j = 0; _j < _width; _j++)
                {
                    var _targetPos = new Vector3(intervalSpace * _i, 0, intervalSpace * _j);
                    
                    var _dualBlockData = CreateDualBlocks(_targetPos);

                    var _parent       = _dualBlockData.GroupParent ;
                    var _bitMaskTable = _dualBlockData.BitMaskTable;

                    var _checkedBit = _allPossibleBits[_currentBitIndex];

                    _parent.name = Convert.ToString(_checkedBit, 2).PadLeft(8, '0');

                    foreach (var _kvp in _bitMaskTable)
                    {
                        var _bit   = _kvp.Key  ;
                        var _block = _kvp.Value;

                        var _hasBit = (_bit & _checkedBit) == _bit;

                        if (!_hasBit)
                            _block.SetActive(false);
                    }

                    _currentBitIndex++;
                }
            }
        }

        private List<int> GetBitMaskPermutation(int _maxPermutation)
        {
            var _bits = new List<int>();
            
            for (var _i = 0; _i < _maxPermutation; _i++)
                _bits.Add(_i);
            
            return _bits;
        }

        private (GameObject GroupParent, Dictionary<int, GameObject> BitMaskTable) CreateDualBlocks(Vector3 _pos)
        {
            var _blockGroup = new GameObject {
                name      = $"Mockup",
                transform = { position = _pos }
            };

            var _bitMaskTable = new Dictionary<int, GameObject>();

            var _bit = 0b_0000_0001;
            
            foreach (var _kvp in offsetTable)
            {
                var _offset = _kvp.Value;
                var _block  = Instantiate(unitBlock, Vector3.zero, Quaternion.identity, _blockGroup.transform);
                _block.transform.localPosition = _offset;
                
                _block.name = $"[{_kvp.Key}]_[{_bit.ToString().PadLeft(3,'_')}]_" + Convert.ToString(_bit, 2).PadLeft(8,'0');

                if (!_bitMaskTable.ContainsKey(_bit))
                    _bitMaskTable.Add(_bit, _block);

                _bit <<= 1;
            }

            return (_blockGroup, _bitMaskTable);
        }

        private void DebugBit(int _bit)
        {
            Debug.Log(Convert.ToString(_bit, 2).PadLeft(8,'0'));
        }
    }
}
