using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime.Core;
using UnityEngine;

namespace BlockBuilder.Scriptable
{
    [CreateAssetMenu(fileName = "InterfaceSetting", menuName = "ScriptableObjects/InterfaceSettingObject", order = 1)]
    public class InterfaceSetting: ScriptableObject
    {
        public enum Mode     { Building, Painting, Setting, Tools }
        
        public enum EditMode { None, BlockPlacement, EditBlocks, EditRoads }

        [SerializeField] public Mode     CurrentMode     = Mode.Building;
        [SerializeField] public EditMode CurrentEditMode = EditMode.EditBlocks;
        
        [SerializeField] public int CurrentMainTapSelection        = -1;
        [SerializeField] public int BlockPlacementToolTabSelection = -1;
        [SerializeField] public int BuildToolTabSelection          = -1;
        [SerializeField] public int RoadBuildToolTabSelection      = -1;

        [SerializeField] public bool MapSettingFoldout = false;

        [SerializeField] public BuilderMode BuilderMode;
    }
} 
