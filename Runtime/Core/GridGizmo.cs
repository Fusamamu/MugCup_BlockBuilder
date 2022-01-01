using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Scriptable;

namespace BlockBuilder.Runtime.Core
{
    //Should Move To Util//
    public class GridGizmo : MonoBehaviour
    {
        #region Dependencies
        [SerializeField] private GridDataSettingSO gridData;
		#endregion
        
        [Range(1, 10)] 
        [SerializeField] private float space = 1;
        [SerializeField] private Vector3 offset;

        public Vector3 GetNearestPointOnGrid(Vector3 _position)
        {
            _position -= transform.position;

            int _xCount = Mathf.RoundToInt(_position.x / space);
            int _yCount = Mathf.RoundToInt(_position.y / space);
            int _zCount = Mathf.RoundToInt(_position.z / space);

            Vector3 _result = new Vector3(
                (float)_xCount * space,
                0.5f,
                (float)_zCount * space
            );

            _result = _result + transform.position + offset;
            
            return _result;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            for (float x = 0; x < gridData.UnitRow; x += space)
            {
                for (float z = 0; z < gridData.UnitColumn; z += space)
                {
                    var _point = GetNearestPointOnGrid(new Vector3(x, 0.5f, z));
                    Gizmos.DrawSphere(_point, 0.1f);
                }
            }
        }
    }
}
