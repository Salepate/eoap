using Dirt.Hackit;
using UnityEngine;
using UnityEngine.UI;

namespace EOAP.Plugin.AP
{
    public static class APCanvasRipper
    {
        // 
        // Sounds
        //
        public enum SFX
        {
            SystemOk,
            SystemOkAlt,
            SystemCancel,
            CursorHover,
            GamePurchase,
            ClockTick,
            ClockBell,
            UseStairs,
            GridPan,
            SelectTool,
            DrawLine,
            ClearMap,
        }

        public static string[] SFXPath = 
        [
            "SE_SYS_OK2",
            "SE_SYS_OK1",
            "SE_SYS_CANCEL",
            "SE_SYS_CURSOR",
            "SE_SYS_PAY",
            "SE_SYS_CLOCK1",
            "SE_SYS_CLOCK2",
            "SE_DNG_STAIR",
            "SE_SYS_GRID",
            "SE_SYS_TOOL",
            "SE_SYS_PEN",
            "SE_SYS_FLOOR",
        ];
        //
        // UI
        //
        public static RectTransform NotificationRoot;
        public static Text NotificationText;
        


        public static Canvas TitleUI;
        public static Canvas GameHUD;
        public static Canvas InnUI;

        public static Font Font01;

        public static Sprite NotificationSprite;
        public static Color NotificationSpriteColor;

        public static void SetupTitleReferences()
        {
            GOResolver.ResetScene();
            TitleUI = GOResolver.Resolve<Canvas>("Canvas");
            Text textObj = GOResolver.Resolve<Text>("Canvas.TitleMenu.SelectList.SelectLoad.Base.Name");
            Font01 = textObj.font;
        }

        internal static void SetupTownReferences()
        {
            GameHUD = GOResolver.Resolve<Canvas>("ScreenInterface(Clone).Canvas");
            var img = GOResolver.Resolve<Image>("ScreenInterface(Clone).Canvas.ShakeGroup.PartyArea.col1.PartyMember.ShakePart.Base");
            NotificationSprite = img?.sprite;
            NotificationSpriteColor = img.color;
            //InnUI = GOResolver.Resolve<Canvas>("Inn(Clone)");
        }
    }
}
