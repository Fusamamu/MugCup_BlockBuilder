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
        public static VolumePoint[] GeneratedVolumePoints(Vector3Int _gridUnitSize, float _pointScale = 0.1f, GameObject _parent = null)
        {
            var _volumePointPrefab = AssetManager.AssetCollection.VolumePoint.gameObject;
            
            var _volumePoints = GeneratedVolumePoints(_gridUnitSize, _volumePointPrefab, _parent);
            
            return _volumePoints;
        }

        public static VolumePoint[] GeneratedVolumePoints(Vector3Int _gridUnitSize, GameObject _volumePointPrefab, GameObject _parent)
        {
            int _rowUnit    = _gridUnitSize.x + 1;
            int _columnUnit = _gridUnitSize.z + 1;
            int _levelUnit  = _gridUnitSize.y + 1;
            
            var _volumePoints = new VolumePoint[_rowUnit * _columnUnit * _levelUnit];
            
            for (var _y = 0; _y < _levelUnit; _y++)
            {
                for (var _x = 0; _x < _rowUnit; _x++)
                {
                    for (var _z = 0; _z < _columnUnit; _z++)
                    {
                        var _offset   = new Vector3(-0.5f, -0.5f, -0.5f);
                        var _position = new Vector3(_x, _y, _z) + _offset;

                        var _pointObject = Object.Instantiate(_volumePointPrefab, _position, Quaternion.identity);

                        if (_parent != null)
                        {
                            _pointObject.transform.position += _parent.transform.position;
                            _pointObject.transform.SetParent(_parent.transform);
                        }
                        
                        var _volumePoint = _pointObject.GetComponent<VolumePoint>();
                        
                        //_volumePoint.Init(new Vector3Int(_x, _y, _z));
                        _volumePoint.SetNodePosition(new Vector3Int(_x, _y, _z));
                         
                        _volumePoints[_z + (_gridUnitSize.x + 1) * (_x + (_gridUnitSize.z + 1) * _y)] = _volumePoint;
                    }
                }
            }

            return _volumePoints;
        }

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
