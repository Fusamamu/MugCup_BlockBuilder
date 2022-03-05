using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightTileArea : MonoBehaviour
{
    [SerializeField] private Material highlightTileAreaMat;
    
    private static readonly int tilesDataArray = Shader.PropertyToID("_TilesDataArray");

    private void Start()
    {
        //Pass Calculated Tile Type into here.
        var _data = new float[4] {0.0f, 0.25f, 0.5f, 1.0f};

        highlightTileAreaMat.SetFloatArray(tilesDataArray, _data);
    }
}
