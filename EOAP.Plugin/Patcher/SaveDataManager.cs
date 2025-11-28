using EOAP.Plugin;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Newtonsoft.Json;

namespace EOAP.Plugin.Patcher
{

    [HarmonyPatch(typeof(SaveDataManager), nameof(SaveDataManager.SaveGameDataSlotAsync))]
    public class AutoPatch_SaveDataManager_SaveGameDataSlotAsync 
    { 
        public static void Prefix()
        {
            APBehaviour.SavePersistentData();
        } 
    }

}
