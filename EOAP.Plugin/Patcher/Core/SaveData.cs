using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher.Core
{
    [HarmonyPatch(typeof(SaveDataManager), nameof(SaveDataManager.SaveGameDataSlotAsync))]
    public class SaveDataManager_SaveGameDataSlotAsync
    {
        public static void Prefix()
        {
            APBehaviour.SavePersistentData();
        }
    }
}
