using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Dirt.Hackit;
using EOAP.Plugin.Behaviours;
using EOAP.Plugin.Dirt;
using EOAP.Plugin.EO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace EOAP.Plugin.AP
{
    using APState = APBehaviour.APState;
    // IMGUI UI
    public class APUI
    {
        public const float WindowHeight = 28f;
        public const float DebugHeight = 28f;
        // Notifications Data
        private float _delay;
        private List<NotificationBehaviour> ActiveNotifications;
        private Queue<Notification> PendingNotifications;
        private Queue<NotificationBehaviour> _notificationPool;
        private CheckTextBehaviour _checksInfo;
        private bool[] _notificationSlots;
        private System.Action<Rect>[] _UIActions;

        public const int ConcurrentNotifications = 10;
        public string Password { get; set; }
        public string Hostname { get; set; }
        public string SlotName { get; set; }
        public bool ShowUI { get; set; }
        public bool ShowDebug { get; internal set; }
        public bool ConnectionError { get; internal set; }
        public string ErrorMessage { get; internal set; }

        private int _subMenuIndex;
        private List<Action<Rect, APUI>> _subMenus { get; set; }
        private List<string> _subMenuLabels;
        private bool _styleReady;
        private int _homeScreenIndex;
        public enum UIAction
        {
            None,
            Connect
        }

        public APUI()
        {
            ActiveNotifications = new List<NotificationBehaviour>();
            PendingNotifications = new Queue<Notification>();
            ShowUI = true;
            _UIActions = new System.Action<Rect>[2];
            _UIActions[0] = DrawConnectionMenu; // Offline
            _UIActions[1] = DrawSessionMenu; // Connected

            _notificationPool = new Queue<NotificationBehaviour>();
            _subMenuLabels = new List<string>();
            _subMenus = new List<Action<Rect, APUI>>();
            _styleReady = false;
            _homeScreenIndex = AddMenu(APHomeScreen.DrawHomescreen);
            SetWindow(_homeScreenIndex);
        }

        public int AddMenu(System.Action<Rect, APUI> menu, string menuName = "")
        {
            _subMenuLabels.Add(menuName);
            _subMenus.Add(menu);
            return _subMenus.Count - 1;
        }

        public void SetWindow(int newWindow, bool force = false)
        {
            if ((_subMenuIndex != newWindow && newWindow != -1) || force)
            {
                _subMenuIndex = newWindow;
            }
            else
            {
                _subMenuIndex = newWindow;
                SetUIVisibility(false);
            }
        }

        public void PushNotification(string text, float duration = 1f)
        {
            Notification notification = new Notification()
            {
                Text = text,
                Timestamp = duration
            };

            PendingNotifications.Enqueue(notification);
        }

        public void RefreshChecks()
        {
            if (_checksInfo != null)
                _checksInfo.FetchChecksData();
        }

        public void Update(float deltaTime)
        {
            ProcessNotifications(deltaTime);
        }

        public void DrawGUI(APState state)
        {
            if (!_styleReady)
            {
                StyleUI.Setup(ShowDebug);
                _styleReady = true;
            }

            float height = WindowHeight;
            if (ShowDebug)
                height += DebugHeight;

            Rect containerPanel = new Rect(10f, 10f, Screen.width - 20f, height);
            Rect archipelagoPanel = containerPanel;
            archipelagoPanel.height = WindowHeight;
            archipelagoPanel.width = 360f;

            Rect tabsPanel = containerPanel;
            tabsPanel.y = archipelagoPanel.yMax;
            tabsPanel.height = DebugHeight;

            Rect uiWindow = containerPanel;
            uiWindow.y = containerPanel.yMax + 10f;
            uiWindow.yMax = Screen.height - 10f;

            Rect toggleRect = containerPanel;
            toggleRect.width = WindowHeight;
            toggleRect.height = WindowHeight;
            archipelagoPanel.xMin = toggleRect.xMax;

            if (_subMenuIndex != _homeScreenIndex)
            {
                if (GUI.Button(toggleRect, ShowUI ? "-" : "+"))
                {
                    SetUIVisibility(!ShowUI);
                }
            }


            if (ShowUI && !InControl.InputManager.Enabled)
            {
                InControl.InputManager.Enabled = false;
            }

            if (state != APState.Connected || ShowUI)
            {
                int stateIndex = (int)state;
                StrippedUI.BeginArea(archipelagoPanel, GUI.skin.box);
                _UIActions[stateIndex](archipelagoPanel);
                StrippedUI.EndArea();
            }

            if (ShowUI)
            {
                DrawMenuTabs(tabsPanel);
                DrawWindow(uiWindow);
            }
        }

        public void SetUIVisibility(bool visibility)
        {
            ShowUI = visibility;
            InControl.InputManager.Enabled = !ShowUI;
        }

        public void DrawSessionMenu(Rect rect)
        {
            EOSession eoSession = APBehaviour.GetSession();
            EOPersistent persistent = APBehaviour.GetPersistent();

            ArchipelagoSession apSession = eoSession.Session;
            GUILayout.BeginHorizontal();
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label("Connected", GUILayout.Width(80f));
            GUI.skin.label.fontStyle = FontStyle.Normal;

            IRoomStateHelper room = apSession.RoomState;
            bool guiState = GUI.enabled;
            bool canRelease = room.ReleasePermissions == Permissions.Enabled || (room.ReleasePermissions == Permissions.Goal && persistent.IsGoal);
            bool canCollect = room.CollectPermissions == Permissions.Enabled || (room.CollectPermissions == Permissions.Goal && persistent.IsGoal);
            GUI.enabled = guiState && canRelease;
            if (GUILayout.Button("Release", GUILayout.Width(100f)))
            {
                apSession.Locations.CompleteLocationChecks(apSession.Locations.AllMissingLocations.ToArray());
            }
            GUI.enabled = guiState && canCollect;
            if (GUILayout.Button("Collect", GUILayout.Width(100f)))
            {
                apSession.Say("!collect");
            }
            GUILayout.EndHorizontal();
            GUI.enabled = guiState;
        }

        public void DrawConnectionMenu(Rect rect)
        {
            var session = APBehaviour.GetSession();
            if (session == null || !session.Connected)
            {
                if (_subMenuIndex != _homeScreenIndex)
                {
                    if (GUILayout.Button("Open Homescreen"))
                    {
                        SetWindow(_homeScreenIndex, true);
                        SetUIVisibility(true);
                    }
                }
            }
        }

        private void DrawMenuTabs(Rect pos)
        {
            int nextWindow = -1;
            if (APBehaviour.UI.ShowDebug)
            {
                StrippedUI.BeginArea(pos, GUI.skin.box);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Game Debug"))
                {
                    APBehaviour.UI.ShowUI = false;
                    InControl.InputManager.Enabled = true;
                    DebugManager.JAOFCFFEELF.Open();
                }

                for (int i = 0; i < _subMenuLabels.Count; ++i)
                {
                    if (string.IsNullOrEmpty(_subMenuLabels[i]))
                        continue;

                    if (GUILayout.Button(_subMenuLabels[i]))
                    {
                        nextWindow = i;
                    }
                }

                GUILayout.EndHorizontal();
                StrippedUI.EndArea();
            }

            if (nextWindow != -1)
            {
                SetWindow(nextWindow);
            }
        }

        private void DrawWindow(Rect pos)
        {
            if (_subMenuIndex < 0 || _subMenuIndex >= _subMenus.Count)
            {
                return;
            }

            StrippedUI.BeginArea(pos, GUI.skin.box);
            _subMenus[_subMenuIndex](pos, this);
            StrippedUI.EndArea();
        }

        private void ProcessNotifications(float deltaTime)
        {
            for (int i = 0; i < ActiveNotifications.Count; ++i)
            {
                if (ActiveNotifications[i].IsComplete)
                {
                    _notificationSlots[ActiveNotifications[i].Slot] = false;
                    _notificationPool.Enqueue(ActiveNotifications[i]);
                    ActiveNotifications.RemoveAt(i--);
                }
            }

            if (ActiveNotifications.Count >= ConcurrentNotifications)
            {
                _delay = 0.5f;
            }

            if (_delay > 0f)
            {
                _delay -= deltaTime;
                return;
            }

            bool playSound = ActiveNotifications.Count == 0;

            while (_notificationPool.Count > 0 && PendingNotifications.Count > 0)
            {
                if (playSound)
                {
                    EO1.PlaySFX(Shinigami.SFX.GamePurchase);
                    playSound = false;
                }
                int slot = -1;

                for(int j = 0; j < _notificationSlots.Length; ++j)
                {
                    if (!_notificationSlots[j])
                    {
                        slot = j;
                        break;
                    }
                }

                if (slot == -1)
                    slot = 0;

                Notification notification = PendingNotifications.Dequeue();
                NotificationBehaviour bhv = _notificationPool.Dequeue();
                bhv.SetNotification(notification);
                ActiveNotifications.Add(bhv);
                _notificationSlots[slot] = true;
                bhv.ShowNotification(slot, 2f);
            }
        }

        internal void CreateNotificationSystem()
        {
            for (int i = 0; i < ConcurrentNotifications; ++i)
            {
                _notificationPool.Enqueue(NotificationBehaviour.Create());
            }
            _notificationSlots = new bool[ConcurrentNotifications];
        }

        internal void CreateCheckText()
        {
            if (_checksInfo == null)
            {
                _checksInfo = CheckTextBehaviour.Create();
            }
        }

        internal void ResetStyle()
        {
            _styleReady = false;
        }

        // structs
        public struct Notification
        {
            public string Text;
            public float Timestamp;
        }
    }
}
