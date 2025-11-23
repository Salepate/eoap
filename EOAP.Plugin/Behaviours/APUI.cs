using UnityEngine;

namespace EOAP.Plugin.Behaviours
{

    // IMGUI UI
    public class APUI
    {
        public string Hostname { get; private set; }
        public string HostnameNoPort { get; private set; }
        public string SlotName { get; private set; }
        public int HostPort { get; private set; }
        public enum UIAction
        {
            None,
            Connect
        }

        public APUI()
        {
            // Debug
            Hostname = "archipelago.gg:59375";
            SlotName = "Player1";
        }


        public UIAction DrawSessionMenu()
        {
            GUILayout.Label("Connected");
            return UIAction.None;
        }

        public UIAction DrawConnectionMenu()
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
    }
}
