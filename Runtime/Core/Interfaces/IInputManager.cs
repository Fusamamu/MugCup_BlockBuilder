using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core.Interfaces
{
    public interface IInputManager
    {
        public bool CheckLeftMouseClicked();
        public bool CheckRightMouseClicked();
        public bool CheckLeftMouseDown();
        public bool CheckLeftMouseUp();

    }
}
