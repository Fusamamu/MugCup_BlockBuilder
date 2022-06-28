using System.Collections;
using System.Collections.Generic;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
    public static class BlockBuilderEditorManager
    {
        public static InterfaceSetting   InterfaceSetting;
        public static GridDataSettingSO  GridDataSettingSo;
        
        private static BlockBuilderManager blockBuilderManager;
        private static BlockManager        blockManager;
        private static BlockEditorManager  blockEditorManager;

        // private static VolumePoint[] volumePoints;
        //
        // private static AnimBool displayBuilderMode;

        private static bool isBlockBuilderManagerInit = false;

        public static void Initialize()
        {
            GetBlockBuilderManager();
            
            InterfaceSetting  = AssetDatabase.LoadAssetAtPath<InterfaceSetting> ("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/InterfaceSetting.asset");
            GridDataSettingSo = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/DefaultGridDataSetting.asset" );
        }
        
        
        public static BlockBuilderManager GetBlockBuilderManager()
        {
            if (!blockBuilderManager)
                blockBuilderManager = Object.FindObjectOfType<BlockBuilderManager>();

            if (!isBlockBuilderManagerInit)
            {
                blockBuilderManager.Initialized();
                isBlockBuilderManagerInit = true;
                
                Debug.Log($"[Block Builder Manager Initialized]");
            }

            return blockBuilderManager;
        }

        public static BlockManager GetBlockManager()
        {
            if (!blockManager)
                blockManager = Object.FindObjectOfType<BlockManager>();

            return blockManager;
        }

        public static BlockEditorManager GetBlockEditorManager()
        {
            if (!blockEditorManager)
                blockEditorManager = Object.FindObjectOfType<BlockEditorManager>();

            return blockEditorManager;
        }
      
    }
}
