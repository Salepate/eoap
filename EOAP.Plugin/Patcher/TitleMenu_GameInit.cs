using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Title;

namespace EOAP.Plugin.Patcher
{
    // Seem to be invoked only once per save
    [HarmonyPatch(typeof(TitleMenu), nameof(TitleMenu.GameInit))]
    public class TitleMenu_GameInit
    {
        private static EOPersistent _persistent;
        private static EOSession _session;
        public static void Postfix() 
        {
            _persistent = APBehaviour.GetPersistent();
            _session = APBehaviour.GetSession();
            if (_session.Connected)
            {
                GDebug.Log("Sync AP Data");
                _session.LoadFlags(_persistent);
            }
            else
            {
                GDebug.Log("Not connected to AP, cannot restore items");
            }
        }
    }
}
