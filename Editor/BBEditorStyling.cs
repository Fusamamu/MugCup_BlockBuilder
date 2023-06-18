#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder.Editor
{
    public static class BBEditorStyling
    {
        private static Rect splitterRect;
        
        static readonly Color headerBackgroundColorDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        
        private static readonly Color splitterdark  = new Color(0.12f, 0.12f, 0.12f, 1.333f);
        private static readonly Color splitterlight = new Color(0.6f, 0.6f, 0.6f, 1.333f);
        
        private static readonly Texture2D paneOptionIcon;
        
        private static Rect backgroundRect;
        private static Rect headerColorRect;
        private static Rect labelRect;
        
        static BBEditorStyling()
        {
            paneOptionIcon = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
        }

        public static bool DrawHeader(Color _headerColor, string _title, bool _foldout)
        {
            backgroundRect = GUILayoutUtility.GetRect(1f, 17f);
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            EditorGUI.DrawRect(backgroundRect, headerBackgroundColorDark);

            headerColorRect = backgroundRect;
            headerColorRect.x = backgroundRect.xMin;
            headerColorRect.y = backgroundRect.yMin;
            headerColorRect.xMin = 0f;
            headerColorRect.width = 5f;
            
            EditorGUI.DrawRect(headerColorRect, _headerColor);

            labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 32f;
            
            EditorGUI.LabelField(labelRect, _title, EditorStyles.boldLabel);

            var e = Event.current;

            var toggleRect = new Rect(headerColorRect.x + 4f, headerColorRect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, _foldout, false);
            }

            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition)) {
                _foldout = !_foldout;
                e.Use();
            }


            return _foldout;

        }

        public static void DrawSome()
        {
            UnityEngine.GUI.DrawTexture(new Rect(0, 0, paneOptionIcon.width, paneOptionIcon.height), paneOptionIcon);
        }
        
        // public static bool Foldout(string title, bool display)
        // {
        //     var style = new GUIStyle("ShurikenModuleTitle");
        //     style.font = new GUIStyle(EditorStyles.label).font;
        //     style.border = new RectOffset(15, 7, 4, 4);
        //     style.fixedHeight = 22;
        //     style.contentOffset = new Vector2(20f, -2f);
        //
        //     var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        //     GUI.Box(rect, title, style);
        //
        //     var e = Event.current;
        //
        //     var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        //     if (e.type == EventType.Repaint) {
        //         EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        //     }
        //
        //     if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
        //         display = !display;
        //         e.Use();
        //     }
        //
        //     return display;
        // }

        public static void DrawCustomButton()
        {
            var _buttonRect = GUILayoutUtility.GetRect(1f, 30f);

            _buttonRect.width  = 30f;
            _buttonRect.height = 30f;

            EditorGUI.DrawRect(_buttonRect, Color.black);

            var _e = Event.current;

            if (_buttonRect.Contains(_e.mousePosition))
            {
                EditorGUI.DrawRect(_buttonRect, Color.blue);
            }
        }

        public static void DrawSection(string _title)
        {
            EditorGUILayout.Space();
            DrawSplitter();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(_title, EditorStyles.boldLabel);
        }
    
        public static void DrawSplitter()
        {
            splitterRect = GUILayoutUtility.GetRect(1f, 1f);

            splitterRect.xMin   = 0f;
            splitterRect.width += 4f;

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            EditorGUI.DrawRect(splitterRect, splitterdark);
        }
    }
}
#endif
