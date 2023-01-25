using System.Collections;
using System.Collections.Generic;
using MugCup_PathFinder.Runtime;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class GridElementManager : MonoBehaviour, IGridManager
    {
        [field: SerializeField] public GridElementDataManager GridElementDataManager { get; private set; }
        
        public void Initialized()
        {
			
        }
        
        public void GenerateGrid()
        {
            GridElementDataManager.GenerateGrid();
        }
    }
}
