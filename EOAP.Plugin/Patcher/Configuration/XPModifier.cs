using EOAP.Plugin.EO;
using HarmonyLib;
using Master;

namespace EOAP.Plugin.Patcher.Configuration
{
    [HarmonyPatch(typeof(MasterManager), nameof(MasterManager.GetMasterEnemyData))]
    public class XPModifier
    {
        public static void Postfix(ref MasterTbbData.ENEMY_DATA __result, int PFODICHNECH)
        {
            uint xpValue = __result.FDFEGNNAEJL;
            xpValue = xpValue * EOConfig.XPPercent / 100;
            __result.FDFEGNNAEJL = xpValue;
        }
    }
}