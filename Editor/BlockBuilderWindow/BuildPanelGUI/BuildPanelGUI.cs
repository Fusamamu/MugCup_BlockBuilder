using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

using MugCup_BlockBuilder.Editor;
using MugCup_BlockBuilder.Runtime.Core;
using MugCup_PathFinder.Runtime;
using BlockBuilder.Scriptable;

namespace MugCup_BlockBuilder
{
    public static class BuildPanelGUI
    {
        private static InterfaceSetting   interfaceSetting;
        private static GridDataSettingSO  gridDataSettingSo;
     
        private static AnimBool displayBuilderMode;
        
        public static void Display()
        {
            GridDataSettingGUI     .Display();
            BlockGeneratorGUI      .Display();
            GridElementGeneratorGUI.Display();
            WaveFunctionCollapseGUI.Display();
            BlockEditorGUI         .Display();
        }
        
        private static void DisplayBuilderModeSelectionInApplication()
        {
            displayBuilderMode.target = Application.isPlaying;

            if (EditorGUILayout.BeginFadeGroup(displayBuilderMode.faded))
            {
                GUILayout.Label("Builder Mode", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Edit"   , EditorStyles.miniButton))
                {
                    var _stateManager = BlockBuilderManager.Instance.GetManager<StateManager>();
                    _stateManager.RequestChangeState(BuilderMode.EditMode);
                }

                if (GUILayout.Button("Handler", EditorStyles.miniButton))
                {
                    var _stateManager = BlockBuilderManager.Instance.GetManager<StateManager>();
                    _stateManager.RequestChangeState(BuilderMode.HandlerMode);
                }
                    
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
        }
    }
}
