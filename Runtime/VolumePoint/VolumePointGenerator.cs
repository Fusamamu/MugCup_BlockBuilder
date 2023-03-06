using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Core;
using BlockBuilder.Runtime.Scriptable;
using MugCup_BlockBuilder;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;

namespace BlockBuilder.Runtime.Core
{
    [System.Serializable]
    public static class VolumePointGenerator
    {
        public static VolumePoint[] GetVolumePoints(Vector3Int _nodeCoord, Vector3Int _gridUnitSize, VolumePoint[] _points)
        {
            var _x = _nodeCoord.x;
            var _y = _nodeCoord.y;
            var _z = _nodeCoord.z;

            var _arrayWidth  = _gridUnitSize.x + 1;
            var _arrayHeight = _gridUnitSize.z + 1;
            
            var _swbPoint = _points[_z +     _arrayWidth * (_x +     _arrayHeight * _y)];
            var _nwbPoint = _points[_z + 1 + _arrayWidth * (_x +     _arrayHeight * _y)];
           
            var _sebPoint = _points[_z +     _arrayWidth * (_x + 1 + _arrayHeight * _y)];
            var _nebPoint = _points[_z + 1 + _arrayWidth * (_x + 1 + _arrayHeight * _y)];
           
            var _swtPoint = _points[_z +     _arrayWidth * (_x +     _arrayHeight * (_y + 1))];
            var _nwtPoint = _points[_z + 1 + _arrayWidth * (_x +     _arrayHeight * (_y + 1))];
            
            var _setPoint = _points[_z +     _arrayWidth * (_x + 1 + _arrayHeight * (_y + 1))];
            var _netPoint = _points[_z + 1 + _arrayWidth * (_x + 1 + _arrayHeight * (_y + 1))];

            var _volumePoints = new [] { _swbPoint, _nwbPoint, _sebPoint, _nebPoint, _swtPoint, _nwtPoint, _setPoint, _netPoint };
        
            return _volumePoints;
        }
    }
}
