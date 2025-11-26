using EOAP.Plugin.Behaviours;
using System.Collections.Generic;
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
            ShowUI = true;
        }

        public void Update(float deltaTime)
        {
            _time += deltaTime;
            ProcessNotifications(deltaTime);
        }


        public UIAction DrawSessionMenu(Rect rect)
        {
            GUILayout.Label("Connected");
            return UIAction.None;
        }

        public UIAction DrawConnectionMenu(Rect rect)
        {
            UIAction action = UIAction.None;
            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("Server");
            GUILayout.Label(Hostname);
            // Crash (Stripped calls)
            //Hostname = GUILayout.TextField(Hostname, GUILayout.Width(200f));
            GUILayout.Label("Slot");
            GUILayout.Label(SlotName);
            //SlotName =  GUILayout.TextField(SlotName, GUILayout.Width(120f));

            if (GUILayout.Button("Connect"))
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
