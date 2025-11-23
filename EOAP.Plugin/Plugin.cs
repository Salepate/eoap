using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using EOAP.Plugin.Behaviours;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace EOAP.Plugin
{
    [BepInPlugin(Plugin.Guid, Plugin.Name, Plugin.Version)]
    public class Plugin : BasePlugin
    {
        public const string Guid = "EOHD.AP";
        public const string Name = "EOHD-Archipelago";
        public const string Version = "0.0.1";
        internal static new ManualLogSource Log;


        public override void Load()
        {
            // Plugin startup logic
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            ClassInjector.RegisterTypeInIl2Cpp<APBehaviour>();
            GameObject apGO = new GameObject("AP");
            apGO.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(apGO);
            apGO.AddComponent<APBehaviour>();
        }
    }
}
