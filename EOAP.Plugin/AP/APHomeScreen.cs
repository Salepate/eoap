using EOAP.Plugin.Behaviours;
using EOAP.Plugin.Dirt;
using UnityEngine;

namespace EOAP.Plugin.AP
{
    public static class APHomeScreen
    {
        public static void DrawHomescreen(Rect pos, APUI ui)
        {
            bool ignoreWait = false;
            bool tryConnect = false;
            bool ignoreQuit = false;

            GUILayout.Label(string.Empty);
            GUILayout.Label(string.Empty);
            StyleUI.Label("Connect to Archipelago to start playing", 48);
            GUILayout.BeginVertical(GUI.skin.box);
            StyleUI.Label("Host");
            StyleUI.FakeTextField(APBehaviour.UI.Hostname, 160f, pos.width);
            StyleUI.Label("Slot");
            StyleUI.FakeTextField(APBehaviour.UI.SlotName, 160f, pos.width);
            StyleUI.Label("Password");
            StyleUI.Label(!string.IsNullOrEmpty(APBehaviour.UI.Password) ? "Yes" : "No", 24);

            if (StyleUI.ButtonXL("Connect", 200f, pos.width))
            {
                tryConnect = true;
            }

            if (StyleUI.ButtonXL("Edit Auth", 200f, pos.width))
            {
                System.Diagnostics.Process.Start("notepad.exe", APUserConfiguration.GetFilePath());
            }


            if (!APBehaviour.UI.ConnectionError)
            {
                GUILayout.Label(" ");
                GUILayout.Label(" ");
            }
            else
            {
                StyleUI.Label("Failed to connect", 30, new Color(0.9f, 0.4f, 0.2f));
                StyleUI.Label(APBehaviour.UI.ErrorMessage ?? "Unknown Error", 24, new Color(0.8f, 0.4f, 0.2f));
            }

            GUILayout.EndVertical();
            if (APBehaviour.UI.ShowDebug)
            {
                ignoreWait = StyleUI.ButtonXL("Ignore", 200f, pos.width);
            }
            
            ignoreQuit = StyleUI.ButtonXL("Quit", 200f, pos.width);

            GUILayout.Label("");


            if (ignoreWait)
            {
                ui.DisplayMenu(-1);
                ui.SetUIVisibility(false);
            }
            if (ignoreQuit)
            {
                Application.quitting = null;
                Application.wantsToQuit = null;
                Application.Quit();
            }

            if (tryConnect)
            {
                APBehaviour.ProcessAction(APUI.UIAction.Connect);
            }
        }
    }
}
