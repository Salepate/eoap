using Camp;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher.Core
{
    [HarmonyPatch(typeof(CampMenuController), nameof(CampMenuController.SetDecideKeepList))]
    public class CheckDisplay
    {
        private static bool _ripped = false;
        public static void Prefix()
        {
            if (!_ripped)
            {
                Shinigami.SetupPauseMenuReferences();
                APBehaviour.UI.CreateCheckText();
                _ripped = true;
            }
            APBehaviour.UI.RefreshChecks();
        }
    }
}
