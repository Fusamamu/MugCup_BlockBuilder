using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBuilder.Scriptable
{
    [CreateAssetMenu(fileName = "GridDataSetting", menuName = "ScriptableObjects/GridDataSettingObject", order = 2)]
    [System.Serializable]
    public class GridDataSettingSO : ScriptableObject
    {
        [Header("Map Size Setting")]
        [SerializeField] public int Row;
        [SerializeField] public int Column;
        [SerializeField] public int Height;

        public Vector3Int MapSize {
            get => new Vector3Int(Row, Height, Column);
            set
            {
                Row    = value.x;
                Height = value.y;
                Column = value.z;
            }
        }

        [Header("Grid Unit Size Setting")]
        [SerializeField] public int UnitRow;
        [SerializeField] public int UnitColumn;
        [SerializeField] public int UnitHeight;

        public Vector3Int GridUnitSize {
            get => new Vector3Int(UnitRow, UnitHeight, UnitColumn);
            set
            {
                UnitRow    = value.x;
                UnitHeight = value.y;
                UnitColumn = value.z;
            }
        }

        public Vector3 GridWorldSize => new Vector3(GridWidth, GridHeight, GridDepth);

        public float GridWidth  => UnitRow    * BlockWidth;
        public float GridHeight => UnitHeight * BlockHeight;
        public float GridDepth  => UnitColumn * BlockDepth;

        [Header("Block Size")]
        [SerializeField] public float BlockWidth;
        [SerializeField] public float BlockHeight;
        [SerializeField] public float BlockDepth;

        [Header("Others")]
        [SerializeField] public Vector3 MapOrigin;
        [SerializeField] public float   TilePadding;

        public void SetRow()
        {
            
        }
        
        public void SetBlockWidth (float _width)  => BlockWidth  = _width;
        
        public void SetBlockHeight(float _height) => BlockHeight = _height;
        
        public void SetBlockSize(float _width, float _height)
        {
            SetBlockWidth (_width) ;
            SetBlockHeight(_height);
        }

        public Vector3 GetGridWorldNodePosition(Vector3Int _nodePos)
        {
            float _xPos = _nodePos.x * BlockWidth;
            float _yPos = _nodePos.y * BlockHeight;
            float _zPos = _nodePos.z * BlockDepth;

            return new Vector3(_xPos, _yPos, _zPos);
        }
        
        public static Vector3 GetGridWorldNodePosition(Vector3Int _nodePos, Vector3 _blockSize)
        {
            float _xPos = _nodePos.x * _blockSize.x;
            float _yPos = _nodePos.y * _blockSize.y;
            float _zPos = _nodePos.z * _blockSize.z;

            return new Vector3(_xPos, _yPos, _zPos);
        }
    }
}
