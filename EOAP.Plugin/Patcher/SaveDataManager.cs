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
            var persistent = APBehaviour.GetPersistent();
            string persistentData = JsonConvert.SerializeObject(persistent);
            System.IO.File.WriteAllText(EOPersistent.GetFilePath(), persistentData);
        } 
    }

}
