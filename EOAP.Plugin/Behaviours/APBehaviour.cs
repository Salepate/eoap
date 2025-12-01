using EOAP.Plugin.AP;
using EOAP.Plugin.Dirt;
using EOAP.Plugin.EO;
using EOAP.Plugin.Patcher;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EOAP.Plugin.Behaviours
{
    public class APBehaviour : MonoBehaviour
    {
        public enum APState
        {
            Offline,
            Connected
        }
        private static APBehaviour s_Instance;

        // internal state
        private EOSession _session;
        private EOPersistent _persistent;
        private APUI _UI;
        private Dictionary<APUI.UIAction, Action> _actionMap;
        private APState _state;
        private APDebug _debug;
        private string _persistentFileName;
        private List<Harmony> _patchers;

        // API
        public static APUI UI => s_Instance._UI;
        public static EOSession GetSession() => s_Instance._session;
        public static EOPersistent GetPersistent() => s_Instance._persistent;

        public static void PushNotification(string notification, float duration = 2f) => s_Instance._UI.PushNotification(notification, duration);


        public APBehaviour(IntPtr ptr) : base(ptr) {}

        private void Start()
        {

            if (!APUserConfiguration.FileExists)
            {
                APUserConfiguration.CreateDefaultSaveFile();
            }

            APUserConfiguration connectionFile = APUserConfiguration.LoadConnectionFile();
            _persistent = new EOPersistent();
            s_Instance = this;
            // State
            _state = APState.Offline;
            _session = new EOSession();
            // DB 
            EO1.LoadDatabase();
            // UI Views
            _UI = new APUI();
            _UI.ShowDebug = connectionFile.DebugUtils;
            _UI.Hostname = connectionFile.Hostname;
            _UI.SlotName = connectionFile.Slotname;
            _UI.Password = connectionFile.Password;
            // UI Actions
            _actionMap = new Dictionary<APUI.UIAction, Action>();
            _actionMap.Add(APUI.UIAction.Connect, StartSession);
            // Harmony Stuff
            _patchers = new List<Harmony>();
            Harmony patcher = new Harmony("eaop.patch");
            patcher.PatchAll();
            Feature_ItemSync.Patch(patcher);
            _patchers.Add(patcher);
            // DBG
            _debug = new APDebug(_UI);
            UI.SetUIVisibility(true);

            //
            if (connectionFile.FastQuit)
            {
                Application.quitting = null;
                Application.wantsToQuit = null;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && (_session == null || !_session.Connected) && _UI != null)
            {
                APUserConfiguration connectionFile = APUserConfiguration.LoadConnectionFile();
                _UI.Hostname = connectionFile.Hostname;
                _UI.SlotName = connectionFile.Slotname;
                _UI.Password = connectionFile.Password;
                _UI.ShowDebug = connectionFile.DebugUtils;
                _UI.ResetStyle();
            }
            if (_UI != null && _UI.ShowUI)
                    InControl.InputManager.Enabled = false;
        }

        private void Update()
        {
            if (_UI != null && _UI.ShowUI && InControl.InputManager.Enabled)
                InControl.InputManager.Enabled = false;

            float dt = Time.deltaTime;
            _UI.Update(dt);
        }

        private void OnGUI()
        {
            _UI.DrawGUI(_state);
        }

        // UI Actions
        public static void ProcessAction(APUI.UIAction action)
        {
            if (s_Instance._actionMap.TryGetValue(action, out Action uiAction))
            {
                uiAction();
            }
        }

        private static void ChangeState(APState newState)
        {
            if (s_Instance._state != newState)
            {
                s_Instance._state = newState;
                switch (newState)
                {
                    case APState.Connected: s_Instance.OnState_Connected(); break;
                }
            }
        }

        private void StartSession()
        {
            string host = UI.Hostname;
            int port;
            if (host.Contains(":"))
            {
                string[] splitted = host.Split(':');
                string uri = splitted[0];
                if (int.TryParse(splitted[1], out port))
                {
                    _session.Start(_UI.SlotName, uri, port);
                    if (_session.Connected)
                    {
                        ChangeState(APState.Connected);
                    }
                }
            }
        }

        // States Changes
        private void OnState_Connected()
        {
            UI.DisplayMenu(-1);
            UI.SetUIVisibility(false);
            _persistentFileName = EOPersistent.GetFilePath(_session.Session.RoomState.Seed, _session.Session.ConnectionInfo.Slot);

            //
            if (EOConfig.ShopSanity)
            {
                Harmony patcher = new Harmony("feature.shopsanity");
                ShopsanityFeature.Patch(patcher);
                _patchers.Add(patcher);
            }

            LoadPersistentData();

            if (EOMemory.AllowLazyLoad)
                _session.LoadFlags(_persistent);
        }

        public static void SavePersistentData()
        {
            if (string.IsNullOrEmpty(s_Instance._persistentFileName))
            {
                GDebug.LogError("Not saving, persistent file name not set");
                return;
            }

            EOPersistent persistent = GetPersistent();
            string persistentData = JsonConvert.SerializeObject(persistent);
            System.IO.File.WriteAllText(s_Instance._persistentFileName, persistentData);
        }
        public static void LoadPersistentData()
        {
            bool loaded = false;
            // load save
            try
            {
                if (System.IO.File.Exists(s_Instance._persistentFileName))
                {
                    GDebug.Log("Load persistent data: " + s_Instance._persistentFileName);
                    string extraData = System.IO.File.ReadAllText(s_Instance._persistentFileName);
                    s_Instance._persistent = JsonConvert.DeserializeObject<EOPersistent>(extraData);
                    loaded = true;
                }
            }
            catch(System.Exception e)
            {
                GDebug.LogError("Failed to load custom save");
                GDebug.LogError(e.Message);
                s_Instance._persistent = new EOPersistent();
            }

            if (!loaded)
            {
                GDebug.Log("Creating custom save file");
                s_Instance._persistent = new EOPersistent();
            }
        }
    }
}
