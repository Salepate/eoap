using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(DungeonTreasureState), nameof(DungeonTreasureState.GetItemFunc))]
    public class DungeonTreasureState_GetItemFunc
    {
        public static bool Prefix(DungeonTreasureState __instance)
        {
            DungeonData.TBData treasureBox = __instance.KOHPIHBMMIC;
            if (EO1.TryGetTreasureBoxLocation(treasureBox.TbId, out string loc))
            {
                APBehaviour.GetSession().SendLocation(loc);
                __instance.OnEnd();
                return false;
            }
            return true;
        }
    }
}
