using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime
{
    public static class Utilities
    {
        public static Vector3Int CastVec3ToVec3Int(Vector3 _pos)
        {
            return new Vector3Int((int)_pos.x, (int)_pos.y, (int)_pos.z);
        }
    }
}
