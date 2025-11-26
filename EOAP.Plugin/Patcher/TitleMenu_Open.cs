using EOAP.Plugin.AP;
using HarmonyLib;
using Title;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(TitleMenu), nameof(TitleMenu.Open))]
    public class TitleMenu_Open
    {
        public static void Prefix()
        {
            APCanvasRipper.SetupTitleReferences();
        }
    }
}
