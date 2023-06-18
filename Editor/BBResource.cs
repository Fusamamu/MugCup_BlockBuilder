#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
    public static class BBResource
    {
        public static Texture[] Tabs;
        
        private static Texture tab1;
        private static Texture tab2;
        private static Texture tab3;

        static BBResource()
        {
            tab1 = (Texture)AssetDatabase.LoadAssetAtPath("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Isolated Block.png", typeof(Texture));
            tab2 = (Texture)AssetDatabase.LoadAssetAtPath("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Isolated Block.png", typeof(Texture));
            tab3 = (Texture)AssetDatabase.LoadAssetAtPath("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Isolated Block.png", typeof(Texture));

            Tabs = new Texture[]
            {
                tab1, tab2, tab3
            };
        }

        public static void Initialized()
        {
            tab1 = (Texture)AssetDatabase.LoadAssetAtPath("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Isolated Block.png", typeof(Texture));
            tab2 = (Texture)AssetDatabase.LoadAssetAtPath("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Isolated Block.png", typeof(Texture));
            tab3 = (Texture)AssetDatabase.LoadAssetAtPath("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Isolated Block.png", typeof(Texture));

            Tabs = new Texture[]
            {
                tab1, tab2, tab3
            };
        }
    }
}
#endif
