using System;
using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime.Core.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MugCup_BlockBuilder.Runtime.Core.Managers
{
    public class InputManager : MonoBehaviour, IInputManager
    {
        private void Awake()
        {
            var _cameraController = new CameraControllerInputAction();
            _cameraController.CameraMovement.Enable();
            _cameraController.CameraMovement.Move.performed += _context =>
            {
                var _inputVector = _context.ReadValue<Vector2>();
                Debug.Log(_context);
            };
        }

        public bool CheckLeftMouseClicked()
        {
            return Mouse.current.leftButton.wasPressedThisFrame;
        }
        public bool CheckRightMouseClicked()
        {
            return Mouse.current.rightButton.wasPressedThisFrame;
        }
    }
}
