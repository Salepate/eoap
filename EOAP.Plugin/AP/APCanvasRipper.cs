using Dirt.Hackit;
using UnityEngine;
using UnityEngine.UI;

namespace EOAP.Plugin.AP
{
    public static class APCanvasRipper
    {
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
