using EOAP.Plugin.AP;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(SQSceneManager), nameof(SQSceneManager.LoadedEntranceOpeningEvent))]
    internal class Feature_StartingGold
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
                    GoldItem.PayGold_WithoutSEPlay((uint)(-delta));
            }
        }
    }
}
