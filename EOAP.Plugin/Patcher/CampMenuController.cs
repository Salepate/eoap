using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(Camp.CampMenuController), nameof(Camp.CampMenuController.SetDecideKeepList))]
    public class AutoPatch_CampMenuController_SetDecideKeepList 
    { 
        public static void Prefix()
        {
            APBehaviour.GetPersistent().ProcessPendingItems();
        } 
    }
}
