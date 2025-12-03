using EOAP.Plugin.AP;
using HarmonyLib;
using Title;

namespace EOAP.Plugin.Patcher.Soulreaper
{
    [HarmonyPatch(typeof(TitleMenu), nameof(TitleMenu.Open))]
    public class RootReaper
    {
        public static void Prefix()
        {
            Shinigami.SetupTitleReferences();
        }
    }
}
