using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Tools
{
    public class CornerMeshPermutation : MonoBehaviour
    {
        private GameObject cornerMockup;

        private void Start()
        {
            cornerMockup = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }
}
