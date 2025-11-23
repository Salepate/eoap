using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    // Seem to be invoked only once per save
    [HarmonyPatch(typeof(TownManager), nameof(TownManager.OpenInn))]
    public class AutoPatch_TownManager_OpenInn 
    { 
        public static void Prefix() 
        {
            var persistent = APBehaviour.GetPersistent();
            var session = APBehaviour.GetSession();
            if (session.Connected)
            {
                session.LoadFlags(persistent);
            }
            else
            {
                GDebug.Log("Not connected to AP, cannot restore items");
            }
        }
    }
}
