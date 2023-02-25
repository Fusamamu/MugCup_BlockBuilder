using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [Serializable]
    public class FaceDetails
    {
        public HorizontalFaceDetails Forward;
        public HorizontalFaceDetails Right;
        public HorizontalFaceDetails Back;
        public HorizontalFaceDetails Left;
        
        public VerticalFaceDetails Up;
        public VerticalFaceDetails Down;

        public Dictionary<ModuleFace, IFaceDetails> Faces 
        {
            get
            {
                if (FaceMap == null || FaceMap.Count == 0)
                    InitFaceMap();
                
                return FaceMap;
            }
        }

        public Dictionary<ModuleFace, IFaceDetails> FaceMap = new Dictionary<ModuleFace, IFaceDetails>();

        private void InitFaceMap()
        {
            FaceMap = new Dictionary<ModuleFace, IFaceDetails>
            {
                { ModuleFace.FORWARD, Forward },
                { ModuleFace.BACK   , Back    },
                { ModuleFace.LEFT   , Left    },
                { ModuleFace.RIGHT  , Right   },
                { ModuleFace.UP     , Up      },
                { ModuleFace.DOWN   , Down    },
            };
        }

        public void RotateCounterClockWise()
        {
            var _previousForward = new HorizontalFaceDetails(Forward);

            Forward = Right;
            Right   = Back;
            Back    = Left;
            Left    = _previousForward;
            
            Up  .RotateDetailsCounterClockwise();
            Down.RotateDetailsCounterClockwise();
            
            InitFaceMap();
        }
          
        public void Reset() 
        {
            Forward = new HorizontalFaceDetails();
            Back    = new HorizontalFaceDetails();
            Left    = new HorizontalFaceDetails();
            Right   = new HorizontalFaceDetails();
            Up      = new VerticalFaceDetails  ();
            Down    = new VerticalFaceDetails  ();
        }
    }
}
