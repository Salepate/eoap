using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using EOAP.Plugin.EO;
using HarmonyLib;
using Il2CppSystem.Runtime.InteropServices;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Steam.ButtonInput;
using System.Runtime.CompilerServices;
using UnityEngine;

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
                EOMemory.AllowLazyLoad = true;
            }
        }

        private static void Postfix()
        {
            if (!_ripped)
            {
                Shinigami.SetupTownReferences();
                _ripped = true;
                APBehaviour.UI.CreateNotificationSystem();
            }
        }
        public static void OhYeah(UIButton button)
        {
            GDebug.Log("OH NONONONO");
            GDebug.Log("button: " + (button != null).ToString());
        }
    }
}
