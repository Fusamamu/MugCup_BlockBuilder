using UnityEditor;
using UnityEngine;
using BlockBuilder.Scriptable;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;

namespace MugCup_BlockBuilder.Editor
{
    public static class BBEditorManager
    {
        public static InterfaceSetting   InterfaceSetting;
        public static GridDataSettingSO  GridDataSettingSo;
        
        public static BlockBuilderManager  BlockBuilderManager;
        public static BlockManager         BlockManager;
        public static GridBlockDataManager BlockDataManager;
        public static BlockEditorManager   BlockEditorManager;

        public static GridElementManager GridElementManager;

        public static void Initialize()
        {
            LoadBlockBuilderManager();
            
            InterfaceSetting  = AssetDatabase.LoadAssetAtPath<InterfaceSetting> ("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/InterfaceSetting.asset"       );
            GridDataSettingSo = AssetDatabase.LoadAssetAtPath<GridDataSettingSO>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/DefaultGridDataSetting.asset" );
        }
        
        private static void LoadBlockBuilderManager()
        {
            if (!BlockBuilderManager) BlockBuilderManager = Object.FindObjectOfType<BlockBuilderManager>();
            if (!BlockManager)        BlockManager        = Object.FindObjectOfType<BlockManager>();
            if (!BlockDataManager)    BlockDataManager    = Object.FindObjectOfType<GridBlockDataManager>();
            if (!BlockEditorManager)  BlockEditorManager  = Object.FindObjectOfType<BlockEditorManager>();

            if (!GridElementManager)  GridElementManager = Object.FindObjectOfType<GridElementManager>();
        }

        private static GameObject GetBlockParent()
        {
            return GameObject.Find(BlockBuilderPathName.BlockParentName);
        }

        public static bool TryGetBlockParent(out GameObject _parent)
        {
            _parent = GetBlockParent();
            
            if (!_parent)
                return false;

            return true;
        }
    }
}
