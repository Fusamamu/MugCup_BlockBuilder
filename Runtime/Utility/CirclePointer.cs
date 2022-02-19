using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CirclePointer : MonoBehaviour
{
    [SerializeField] private Material circlePointerMat;
    
    private const string PointerPosition   = "_PointerPos";
    private static readonly int pointerPos = Shader.PropertyToID(PointerPosition);
    
    private void Start()
    {
        
    }

    private void Update()
    {
        Ray _ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(_ray, out var _hit))
        {
            circlePointerMat.SetVector(pointerPos, new Vector4(_hit.point.x, _hit.point.z, 0));
        }
    }
}
