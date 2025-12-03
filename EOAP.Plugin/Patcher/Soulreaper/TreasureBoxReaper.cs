using DungeonData;
using EOAP.Plugin.AP;
using HarmonyLib;

namespace EOAP.Plugin.Patcher.Soulreaper
{
    [HarmonyPatch(typeof(DungeonTreasureState), nameof(DungeonTreasureState.TreasureOpenCheckFunc))]
    public class DungeonTreasureState_TreasureOpenCheckFunc
    {
        public static void Prefix(DungeonTreasureState __instance)
        {
            TBData treasureBox = __instance.KOHPIHBMMIC;
#if AP_DEBUG
            APDebug.PrintTreasureBox((int)treasureBox.TbId);
#endif
        }
    }
}
