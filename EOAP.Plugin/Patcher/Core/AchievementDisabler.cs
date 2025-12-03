using HarmonyLib;

namespace EOAP.Plugin.Patcher.Core
{
    [HarmonyPatch(typeof(SteamAchievementClient), nameof(SteamAchievementClient.UnlockAchievement))]
    public class AchievementDisabler
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}
