using HarmonyLib;

namespace EOAP.Plugin.Patcher.Feature
{
    [HarmonyPatch(typeof(DungeonTreasureState), nameof(DungeonTreasureState.GetItemFunc))]
    public class TreasureBox
    {
        public static bool Prefix(DungeonTreasureState __instance)
        {
            return false; // given through event 
        }
    }
}
