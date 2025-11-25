using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace Dirt.Hackit
{
    internal class StrippedUI
    {
        public static void BeginArea(Rect screenRect)
        {
            BeginArea(screenRect, GUIContent.none, GUIStyle.none);
        }

        public static void BeginArea(Rect screenRect, string text)
        {
            BeginArea(screenRect, GUIContent.Temp(text), GUIStyle.none);
        }

        public static void BeginArea(Rect screenRect, Texture image)
        {
            BeginArea(screenRect, GUIContent.Temp(image), GUIStyle.none);
        }

        public static void BeginArea(Rect screenRect, GUIContent content)
        {
            BeginArea(screenRect, content, GUIStyle.none);
        }

        public static void BeginArea(Rect screenRect, GUIStyle style)
        {
            BeginArea(screenRect, GUIContent.none, style);
        }

        public static void BeginArea(Rect screenRect, string text, GUIStyle style)
        {
            BeginArea(screenRect, GUIContent.Temp(text), style);
        }

        public static void BeginArea(Rect screenRect, Texture image, GUIStyle style)
        {
            BeginArea(screenRect, GUIContent.Temp(image), style);
        }

        public static void BeginArea(Rect screenRect, GUIContent content, GUIStyle style)
        {
            var il2type = Il2CppInterop.Runtime.Il2CppType.Of<GUILayoutGroup>();

            GUIUtility.CheckOnGUI();
            GUILayoutGroup guiLayoutGroup = GUILayoutUtility.BeginLayoutArea(style, il2type);
            if (Event.current.type == EventType.Layout)
            {
                guiLayoutGroup.resetCoords = true;
                guiLayoutGroup.minWidth = guiLayoutGroup.maxWidth = screenRect.width;
                guiLayoutGroup.minHeight = guiLayoutGroup.maxHeight = screenRect.height;
                guiLayoutGroup.rect = Rect.MinMaxRect(screenRect.xMin, screenRect.yMin, guiLayoutGroup.rect.xMax, guiLayoutGroup.rect.yMax);
            }
            GUI.BeginGroup(guiLayoutGroup.rect, content, style);
        }

        public static void EndArea()
        {
            GUILayout.EndArea();
        }
    }
}
