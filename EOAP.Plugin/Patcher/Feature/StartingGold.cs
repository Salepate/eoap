using EOAP.Plugin.EO;
using HarmonyLib;

namespace EOAP.Plugin.Patcher.Feature
{
    [HarmonyPatch(typeof(SQSceneManager), nameof(SQSceneManager.LoadedEntranceOpeningEvent))]
    internal class StartingGold
    {
        public static void Prefix()
        {
            int startGold = GoldItem.GetGold();

            if (EOConfig.StartingEntal != 0 && startGold != EOConfig.StartingEntal)
            {
                int delta = EOConfig.StartingEntal - startGold;
                if (delta > 0)
                    GoldItem.GiveGold((uint)delta);
                else if (delta < 0)
                    GoldItem.PayGold_WithoutSEPlay((uint)-delta);
            }
        }
    }
}
