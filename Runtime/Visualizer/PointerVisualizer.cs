using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace MugCup_BlockBuilder.Runtime.Core
{
    //Can be changed to Observer object.
    public class PointerVisualizer : MonoBehaviour
    {
        [SerializeField] private GridBlockSelection gridBlockSelection;
        
        [ReadOnly] [SerializeField] private GameObject pointer;

        private void Start()
        {
#if UNITY_EDITOR
            pointer = Visualizer.CreateBlockTypeV1Pointer();
            pointer.SetActive(false);

            gridBlockSelection = FindObjectOfType<GridBlockSelection>();
            
            gridBlockSelection.RegisterEvent(GridEvent.ON_GRID_HIT,          _o => pointer.SetActive(true));
            gridBlockSelection.RegisterEvent(GridEvent.ON_GIRD_OUT_OF_BOUND, _o => pointer.SetActive(false));
            
            gridBlockSelection.RegisterEvent(GridEvent.ON_GRID_HIT, _o =>
            {
                var _hitPosition = ((GridBlockSelection)_o).HitPosition;
                pointer.transform.position = _hitPosition;
            });
#endif
        }

        public void ShowPointer() => pointer.SetActive(true );
        public void HidePointer() => pointer.SetActive(false);

        public void ChangePointerType()
        {
            
        }

        public void OnBuilderModeChanged()
        {
            //Change Pointer?
        }
    }
}
