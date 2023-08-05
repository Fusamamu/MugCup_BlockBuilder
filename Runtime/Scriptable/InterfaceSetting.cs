using System.Collections;
using System.Collections.Generic;
using MugCup_BlockBuilder.Runtime.Core;
using UnityEngine;

namespace BlockBuilder.Scriptable
{
    [CreateAssetMenu(fileName = "InterfaceSetting", menuName = "ScriptableObjects/InterfaceSettingObject", order = 1)]
    public class InterfaceSetting: ScriptableObject
    {
        public enum BuildType   
        { 
            BLOB_TILE,
            MARCHING_CUBE 
        }
        
        public enum Mode
        {
            BUILDING, 
            PAINTING, 
            SETTING, 
            TOOLS
        }

        public enum EditMode
        {
            NONE,
            BLOCK_PLACEMENT, 
            EDIT_BLOCKS, 
            EDIT_ROADS,
            CHANGE_BLOCK_TYPE
        }

        [SerializeField] public BuildType SelectedBuildType = BuildType.MARCHING_CUBE;
        [SerializeField] public Mode      CurrentMode       = Mode.BUILDING;
        [SerializeField] public EditMode  CurrentEditMode   = EditMode.EDIT_BLOCKS;
        
        [SerializeField] public int CurrentMainTapSelection        = -1;
        [SerializeField] public int BlockPlacementToolTabSelection = -1;
        [SerializeField] public int BuildToolTabSelection          = -1;
        [SerializeField] public int RoadBuildToolTabSelection      = -1;
        [SerializeField] public int EditBlockTypeToolTabSelection  = -1;

        [SerializeField] public int BlockTypeTabSelection = -1;

        [SerializeField] public bool MapSettingFoldout = false;

        [SerializeField] public BuilderMode BuilderMode;
    }
} 
