using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher.Core
{
    [HarmonyPatch(typeof(TownManager), nameof(TownManager.OpenInn))]
    public class InnReaper
    {
        private static bool _ripped;

        private static void Postfix()
        {
            if (!_ripped)
            {
                Shinigami.SetupTownReferences();
                _ripped = true;
                APBehaviour.UI.CreateNotificationSystem();
            }
        }
    }
}
