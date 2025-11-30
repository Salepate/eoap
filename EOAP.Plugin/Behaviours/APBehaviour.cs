using Archipelago.MultiClient.Net;
using Dirt.Hackit;
using EOAP.Plugin.AP;
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
        public const float WindowHeight = 28f;
        public const float DebugHeight = 28f;
        public enum APState
        {
            Offline,
            Connected
        }


        // internal state
        private EOSession _session;
        private EOPersistent _persistent;
        private bool _showDebug;
        private APUI _UI;
        private System.Func<Rect, APUI.UIAction>[] _UIActions;
        private Dictionary<APUI.UIAction, System.Action> _actionMap;
        private APState _state;


        private static APBehaviour s_Instance;
        private APDebug _debug;
        private string _persistentFileName;

        private List<Harmony> _patchers;

        // API
        public static APUI UI => s_Instance._UI;
        public static EOSession GetSession() => s_Instance._session;
        public static EOPersistent GetPersistent() => s_Instance._persistent;

        public static void PushNotification(string notification, float duration = 2f) => s_Instance._UI.PushNotification(notification, duration);

        public APBehaviour(IntPtr ptr) : base(ptr)
        {

        }

        private void Start()
        {
            if (!APConnection.FileExists)
            {
                APConnection.CreateDefaultSaveFile();
            }

            APConnection connectionFile = APConnection.LoadConnectionFile();
            _persistent = new EOPersistent();
            s_Instance = this;
            // State
            _showDebug = true;
            _state = APState.Offline;
            _session = new EOSession();
            // DB 
            EOItems.LoadDatabase();
            // UI Views
            _UI = new APUI();
            _UI.Hostname = connectionFile.Hostname;
            _UI.SlotName = connectionFile.Slotname;
            _UI.Password = connectionFile.Password;
            _UIActions = new System.Func<Rect, APUI.UIAction>[2];
            _UIActions[0] = _UI.DrawConnectionMenu; // Offline
            _UIActions[1] = _UI.DrawSessionMenu; // Connected
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
            _debug = new APDebug();

            InControl.InputManager.Enabled = false;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && (_session == null || !_session.Connected) && _UI != null)
            {
                APConnection connectionFile = APConnection.LoadConnectionFile();
                _UI.Hostname = connectionFile.Hostname;
                _UI.SlotName = connectionFile.Slotname;
                _UI.Password = connectionFile.Password;
                InControl.InputManager.Enabled = false;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _UI.Update(dt);
        }

        private void OnGUI()
        {
            float height = WindowHeight;
            if (_showDebug)
                height += DebugHeight;

            APUI.UIAction action = APUI.UIAction.None;

            Rect containerPanel = new Rect(10f, 10f, Screen.width - 20f, height);
            Rect archipelagoPanel = containerPanel;
            archipelagoPanel.height = WindowHeight;

            Rect debugPanel = containerPanel;
            debugPanel.y = archipelagoPanel.yMax;
            debugPanel.height = DebugHeight;

            Rect windowScreen = containerPanel;
            windowScreen.y = containerPanel.yMax + 10f;
            windowScreen.yMax = Screen.height - 10f;

            Rect toggleRect = containerPanel;
            toggleRect.width = WindowHeight;
            toggleRect.height = WindowHeight;
            archipelagoPanel.xMin = toggleRect.xMax;


            if (GUI.Button(toggleRect, _UI.ShowUI ? "-" : "+"))
            {
                _UI.ShowUI =  !_UI.ShowUI;
                InControl.InputManager.Enabled = !_UI.ShowUI;
            }

            if (_UI.ShowUI && !InControl.InputManager.Enabled)
                InControl.InputManager.Enabled = false;


            if (_state != APState.Connected || _UI.ShowUI)
            {
                int stateIndex = (int)_state;
                StrippedUI.BeginArea(archipelagoPanel, GUI.skin.box);
                action = _UIActions[stateIndex](archipelagoPanel);
                StrippedUI.EndArea();
            }

            if (_UI.ShowUI && _showDebug)
            {
                _debug.DrawUI(debugPanel);
                _debug.DrawWindow(windowScreen);
            }

            if (_actionMap.TryGetValue(action, out Action uiAction))
            {
                uiAction();
            }
        }

        // UI Actions

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
            _session.Start(_UI.SlotName, _UI.HostnameNoPort, _UI.HostPort);
            if (_session.Connected)
            {
                ChangeState(APState.Connected);
            }
        }

        // States Changes
        private void OnState_Connected()
        {
            _debug.SwapToWindow(-1);
            UI.ShowUI = false;
            _persistentFileName = EOPersistent.GetFilePath(_session.Session.RoomState.Seed, _session.Session.ConnectionInfo.Slot);

            //
            if (EOConfig.ShopSanity)
            {
                Harmony patcher = new Harmony("feature.shopsanity");
                ShopsanityFeature.Patch(patcher);
                _patchers.Add(patcher);
            }

            LoadPersistentData();
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
