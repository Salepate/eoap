using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using EOAP.Plugin.Behaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EOAP.Plugin.AP
{

    // IMGUI UI
    public class APUI
    {
        // Notifications Data
        private float _time;
        private float _delay;
        private List<NotificationBehaviour> ActiveNotifications;
        private Queue<Notification> PendingNotifications;
        private Queue<NotificationBehaviour> _notificationPool;
        private bool[] _notificationSlots;
        public const int ConcurrentNotifications = 10;
        public string Hostname { get; set; }
        public string SlotName { get; set; }
        public bool ShowUI { get; set; }

        public string HostnameNoPort { get; private set; }
        public int HostPort { get; private set; }
        public enum UIAction
        {
            None,
            Connect
        }

        public APUI()
        {
            ActiveNotifications = new List<NotificationBehaviour>();
            _notificationPool = new Queue<NotificationBehaviour>();
            PendingNotifications = new Queue<Notification>();
            _time = 0f;
            ShowUI = false;
        }

        public void Update(float deltaTime)
        {
            _time += deltaTime;
            ProcessNotifications(deltaTime);
        }


        public UIAction DrawSessionMenu(Rect rect)
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
            return UIAction.None;
        }

        public UIAction DrawConnectionMenu(Rect rect)
        {
            UIAction action = UIAction.None;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Server: ", GUILayout.Width(50f));
            GUILayout.Label(Hostname, GUI.skin.box, GUILayout.Width(150f));
            // Crash (Stripped calls)
            //Hostname = GUILayout.TextField(Hostname, GUILayout.Width(200f));
            GUILayout.Label("Slot:", GUILayout.Width(40f));
            GUILayout.Label(SlotName, GUI.skin.box, GUILayout.Width(150f));
            GUILayout.Label("Password: No", GUILayout.Width(90f));
            //SlotName =  GUILayout.TextField(SlotName, GUILayout.Width(120f));

            if (GUILayout.Button("Connect", GUILayout.Width(80f)))
            {
                if (Hostname.Contains(":"))
                {
                    string[] splitted = Hostname.Split(':');
                    HostnameNoPort = splitted[0];
                    if (int.TryParse(splitted[1], out int port))
                    {
                        HostPort = port;
                        action = UIAction.Connect;
                    }
                }
            }

            GUILayout.EndHorizontal();
            return action;
        }


        public struct Notification
        {
            public string Text;
            public float Timestamp;
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
                    EO1.PlaySFX(APCanvasRipper.SFX.GamePurchase);
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
    }
}
