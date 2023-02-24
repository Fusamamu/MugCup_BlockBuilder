using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class Orientations
    {
        public static Dictionary<ModuleFace, Quaternion> Rotations 
        {
            get 
            {
                if (rotations == null) 
                    Initialize();
                
                return rotations;
            }
        }
        
        private static Dictionary<ModuleFace, Vector3> vectors;
        private static Dictionary<ModuleFace, Quaternion> rotations;
        
        private static void Initialize()
        {
            vectors = new Dictionary<ModuleFace, Vector3>
            {
                { ModuleFace.FORWARD, Vector3.forward },
                { ModuleFace.BACK   , Vector3.back    },
                { ModuleFace.LEFT   , Vector3.left    },
                { ModuleFace.RIGHT  , Vector3.right   },
                { ModuleFace.UP     , Vector3.up      },
                { ModuleFace.DOWN   , Vector3.down    }
            };

            rotations = vectors.ToDictionary(_x => _x.Key, _x => Quaternion.LookRotation(_x.Value));
        }
    }
}
