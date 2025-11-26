using System.Collections.Generic;
using UnityEngine;

namespace EOAP.Plugin.AP
{

    // IMGUI UI
    public class APUI
    {
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
            ActiveNotifications = new List<Notification>();
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


        // Debug Notification System
        private float _time;
        private float _delay;
        private List<Notification> ActiveNotifications;
        private Queue<Notification> PendingNotifications;
        private struct Notification
        {
            public string Text;
            public float Timestamp;
        }

        public void DrawNotificationScreen()
        {
            float height = ActiveNotifications.Count * 32f;
            Rect pos = new Rect(Screen.width - 250f, 10f, 240f, height);

            if (ActiveNotifications.Count == 0)
                return;

            GUI.BeginGroup(pos, GUI.skin.box);
            for (int i = 0; i < ActiveNotifications.Count; ++i)
            {
                Rect labelRect = new Rect(0f, i * 32f, 240f, 32f);
                GUI.Label(labelRect, ActiveNotifications[i].Text);
            }
            GUI.EndGroup();
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
                if (ActiveNotifications[i].Timestamp < _time)
                {
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

            while (ActiveNotifications.Count < ConcurrentNotifications && PendingNotifications.Count > 0)
            {
                Notification notification = PendingNotifications.Dequeue();
                notification.Timestamp += _time;
                ActiveNotifications.Add(notification);
            }
        }
    }
}
