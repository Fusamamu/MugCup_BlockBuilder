using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBuilder.Scriptable
{
    [CreateAssetMenu(fileName = "InterfaceSetting", menuName = "ScriptableObjects/InterfaceSettingObject", order = 1)]
    public class InterfaceSetting: ScriptableObject
    {
        public enum Mode { Building, Painting, Setting, Tools }

        [SerializeField] public Mode CurrentMode = Mode.Building;
        
        [SerializeField] public int CurrentMainTapSelection = -1;
        [SerializeField] public int BuildToolTabSelection   = -1;

        [SerializeField] public bool MapSettingFoldout = false;
    }
} 
