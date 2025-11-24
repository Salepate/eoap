using EOAP.Plugin.AP;
using EOAP.Plugin.DB;
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
        public const float DebugHeight = 120f;
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
        private System.Func<APUI.UIAction>[] _UIActions;
        private Dictionary<APUI.UIAction, System.Action> _actionMap;
        private APState _state;


        private static APBehaviour s_Instance;


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
            _UIActions = new System.Func<APUI.UIAction>[2];
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
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _UI.Update(dt);
        }

        private void OnGUI()
        {
            _UI.DrawNotificationScreen();

            float uiHeight = _showDebug ? WindowHeight + DebugHeight : WindowHeight;
            Rect debugRect = new Rect(10f, Screen.height - (uiHeight + 10f), Screen.width - 20f, uiHeight);
            Rect toggleUIRect = debugRect;
            toggleUIRect.width = 16f;
            toggleUIRect.height = 16f;
            toggleUIRect.y = debugRect.y - 16f;


            GUI.BeginGroup(toggleUIRect, GUI.skin.box);
            if (GUI.Button(new Rect(0f, 0f, 16f, 16f), _UI.ShowUI ? "-" : "+"))
            {
                _UI.ShowUI =  !_UI.ShowUI;
            }
            GUI.EndGroup();


            if (!_UI.ShowUI)
            {
                return;
            }

            GUI.BeginGroup(debugRect, GUI.skin.box);
            int stateIndex = (int)_state;
            APUI.UIAction action = _UIActions[stateIndex]();

            if (_showDebug)
                ShowDebugUI();
            GUI.EndGroup();


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

        // Debugging
        private void ShowDebugUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            if (GUILayout.Button("Toggle Debug Menu"))
            {
                DebugManager.JAOFCFFEELF.Open();
            }

            bool inputState = InControl.InputManager.Enabled;

            if (GUILayout.Button("Block Inputs: " + (inputState.ToString())))
            {
                InControl.InputManager.Enabled = !inputState;
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            bool guiState = GUI.enabled;
            GUI.enabled = _session.Connected;
            if (GUILayout.Button("Sync New Items"))
            {
                _session.SyncNewItems(_persistent, false);
            }
            GUI.enabled = guiState;
            if (GUILayout.Button("Save Persistent"))
            {
                string persistentData = JsonConvert.SerializeObject(_persistent);
                System.IO.File.WriteAllText(EOPersistent.GetFilePath(), persistentData);
            }
            GUILayout.EndVertical();

            //GUILayout.BeginVertical();
            //if (GUILayout.Button("Load DynDB"))
            //{
            //    Builder.Load("dyndb.json");
            //}
            //if (GUILayout.Button("Save DynDB"))
            //{
            //    Builder.Dump("dyndb.json");
            //}
            //GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
