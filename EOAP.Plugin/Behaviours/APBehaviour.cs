using Archipelago.MultiClient.Net;
using DungeonData;
using EOAP.Plugin.AP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EOAP.Plugin.Behaviours
{
    public class APBehaviour : MonoBehaviour
    {
        public const float WindowHeight = 80f;
        public const float DebugHeight = 160f;
        public enum APState
        {
            Offline,
            Connected
        }

        // api
        private EOSession _session;

        // internal state
        private bool _showDebug;
        private APUI _UI;
        private System.Func<APUI.UIAction>[] _UIActions;
        private Dictionary<APUI.UIAction, System.Action> _actionMap;
        private APState _state;


        private static APBehaviour s_Instance;

        public static void ChangeState(APState newState)
        {
            if (s_Instance._state != newState)
            {
                s_Instance._state = newState;
            }
        }

        public static ArchipelagoSession GetSession() => s_Instance._session.Session;

        public APBehaviour(IntPtr ptr) : base(ptr)
        {

        }

        private void Start()
        {
            s_Instance = this;
            // State
            _showDebug = true;
            _state = APState.Offline;
            _session = new EOSession();

            // UI Views
            _UI = new APUI();
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

        private void OnGUI()
        {
            float uiHeight = _showDebug ? WindowHeight : WindowHeight + DebugHeight;
            Rect debugRect = new Rect(10f, Screen.height - (uiHeight + 10f), Screen.width - 20f, uiHeight);
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
        private void StartSession()
        {
            _session.Start(_UI.SlotName, _UI.HostnameNoPort, _UI.HostPort);
            if (_session.Connected)
            {
                ChangeState(APState.Connected);
            }
        }

        // Debugging
        private void ShowDebugUI()
        {
            if (GUILayout.Button("Toggle Debug Menu"))
            {
                DebugManager.JAOFCFFEELF.Open();
            }

            bool inputState = InControl.InputManager.Enabled;

            if (GUILayout.Button("Block Inputs: " + (inputState.ToString())))
            {
                InControl.InputManager.Enabled = !inputState;
            }
        }

    }
}
