using EOAP.Plugin.Behaviours;
using EOAP.Plugin.EO;
using HarmonyLib;
using Title;

namespace EOAP.Plugin.Patcher.Core
{
    // Seem to be invoked only once per save
    [HarmonyPatch(typeof(TitleMenu), nameof(TitleMenu.GameInit))]
    public class LoadData
    {
        private static EOPersistent _persistent;
        private static EOSession _session;

        public static void TryLoadData()
        {
            EOMemory.LoadFixedDatabase();
            _persistent = APBehaviour.GetPersistent();
            _session = APBehaviour.GetSession();
            EO1.LoadAutoFlags();
            if (_session.Connected)
            {
                GDebug.Log("Sync AP Data");
                _session.LoadFlags(_persistent);
            }
            else
            {
                EOMemory.AllowLazyLoad = true;
                GDebug.LogError("Not connected to AP, cannot restore items");
            }
        }
        public static void Postfix()
        {
            TryLoadData();
        }
    }

    [HarmonyPatch(typeof(TownManager), nameof(TownManager.OpenInn))]
    public class LoadDataTown
    {
        public static void Prefix()
        {
            LoadData.TryLoadData();
        }
    }
}
