using Dirt.Hackit;
using EOAP.Plugin.AP;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EOAP.Plugin.Behaviours
{
    public class APBehaviour : MonoBehaviour
    {
        public const float WindowHeight = 40f;
        public const float DebugHeight = 40f;
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


        // API
        public static EOSession GetSession() => s_Instance._session;
        public static EOPersistent GetPersistent() => s_Instance._persistent;

        public static void PushNotification(string notification, float duration = 2f) => s_Instance._UI.PushNotification(notification, duration);

        public APBehaviour(IntPtr ptr) : base(ptr)
        {

        }

        private void Start()
        {
            APConnection connectionFile = APConnection.LoadConnectionFile();
            _persistent = new EOPersistent();
            s_Instance = this;
            // State
            _showDebug = true;
            _state = APState.Offline;
            _session = new EOSession();

            // UI Views
            _UI = new APUI();
            _UI.Hostname = connectionFile.Hostname;
            _UI.SlotName = connectionFile.Slotname;
            _UIActions = new System.Func<Rect, APUI.UIAction>[2];
            _UIActions[0] = _UI.DrawConnectionMenu; // Offline
            _UIActions[1] = _UI.DrawSessionMenu; // Connected
            // UI Actions
            _actionMap = new Dictionary<APUI.UIAction, Action>();
            _actionMap.Add(APUI.UIAction.Connect, StartSession);
            // Harmony Stuff
            Harmony patcher = new Harmony("eaop.patch");
            patcher.PatchAll();
            // tmp
            InControl.InputManager.Enabled = false;

            // DBG
            _debug = new APDebug();
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _UI.Update(dt);
        }

        private void OnGUI()
        {
            _UI.DrawNotificationScreen();

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
            toggleRect.width = 16f;
            toggleRect.height = 16f;
            archipelagoPanel.xMin = toggleRect.xMax;


            if (GUI.Button(toggleRect, _UI.ShowUI ? "-" : "+"))
            {
                _UI.ShowUI =  !_UI.ShowUI;
            }

            if (_UI.ShowUI)
            {
                int stateIndex = (int)_state;

                StrippedUI.BeginArea(archipelagoPanel);
                action = _UIActions[stateIndex](archipelagoPanel);
                StrippedUI.EndArea();

                if (_showDebug)
                {
                    _debug.DrawUI(debugPanel);
                    _debug.DrawWindow(windowScreen);
                }
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
            bool loaded = false;
            // load save
            try
            {
                if (System.IO.File.Exists(EOPersistent.GetFilePath()))
                {
                    string extraData = System.IO.File.ReadAllText(EOPersistent.GetFilePath());
                    _persistent = JsonConvert.DeserializeObject<EOPersistent>(extraData);
                    loaded = true;
                }
            }
            catch(System.Exception e)
            {
                GDebug.Log("Failed to load custom save");
                _persistent = new EOPersistent();
            }

            if (!loaded)
            {
                GDebug.Log("Creating custom save file");
                _persistent = new EOPersistent();
            }
        }
    }
}
