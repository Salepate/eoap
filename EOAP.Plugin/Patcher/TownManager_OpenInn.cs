using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    // Seem to be invoked only once per save
    [HarmonyPatch(typeof(TownManager), nameof(TownManager.OpenInn))]
    public class TownManager_OpenInn 
    {
        private static bool _ripped;
        private static EOPersistent _persistent;
        private static EOSession _session;
        public static void Prefix() 
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
        private static void Postfix()
        {
            if (!_ripped)
            {
                APCanvasRipper.SetupTownReferences();
                _ripped = true;
                APBehaviour.UI.CreateNotificationSystem();
            }
            //rectTr.anchoredPosition = new Vector2(rect.width - rectTr.sizeDelta.x - 10f, rect.height * 0.5f - rectTr.sizeDelta.y);
        }
    }
}
