using System;
using UnityEngine;

namespace EOAP.Plugin.Dirt
{
    public static class StyleUI
    {
        //=============================================================
        // Config: Styling
        //=============================================================
        private static GUILayoutOption _styleM;
        private static GUILayoutOption _styleXS;
        private static Texture2D consoleBackground;

        public static void Setup(bool isDebug)
        {
            _styleM = GUILayout.Width(160f);
            _styleXS = GUILayout.Width(20f);

            if(!isDebug)
            {
                consoleBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                consoleBackground.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f, 0.8f));
                consoleBackground.Apply();
                GUI.skin.box.normal.background = consoleBackground;
            }
        }

        public static bool ButtonM(string label) => GUILayout.Button(label, _styleM);
        public static bool ButtonXS(string label) => GUILayout.Button(label, _styleXS);

        public static bool ButtonXL(string label, float buttonWidth, float totalWidth, float fontSize = 30f)
        {
            bool pressed;
            var button = GUI.skin.button;
            int btnFntSize = button.fontSize;
            button.fontSize = (int)fontSize;
            button.fontStyle = FontStyle.Bold;
            GUILayout.BeginHorizontal();
            float halfPad = (totalWidth - buttonWidth) * 0.5f;
            GUILayout.Label("", GUILayout.Width(halfPad));
            pressed = GUILayout.Button(label, GUILayout.Height(fontSize + 12f), GUILayout.Width(buttonWidth));
            GUILayout.Label("", GUILayout.Width(halfPad));
            GUILayout.EndHorizontal();
            button.fontSize = btnFntSize;
            button.fontStyle = FontStyle.Normal;
            return pressed;
        }

        public static void Label(string label, int size = 24, GUIStyle style = null) => Label(label, size, Color.white, style);
        public static void Label(string label, int size, Color c, GUIStyle style = null)
        {

            TextAnchor align = GUI.skin.label.alignment;
            int fontSize = GUI.skin.label.fontSize;
            Color guiColor = GUI.color;
            GUI.color = c;
            GUI.skin.label.fontSize = size;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            if (style == null)
                GUILayout.Label(label, GUILayout.Height((float)size + 12f));
            else
                GUILayout.Label(label, style, GUILayout.Height((float)size + 12f));
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = align;
            GUI.color = guiColor;
        }
        public static void FakeTextField(string label, float width, float totalWidth)
        {
            float deltaWidth = (totalWidth - width) * 0.5f;

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Empty, GUILayout.Width(deltaWidth));
            TextAnchor align = GUI.skin.label.alignment;
            int fontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(label, GUI.skin.box, GUILayout.Width(width));
            GUI.skin.label.fontSize = fontSize;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = align;
            GUILayout.Label(string.Empty, GUILayout.Width(deltaWidth));
            GUILayout.EndHorizontal();
        }
    }
}
