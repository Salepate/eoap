using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Il2CppInterop.Runtime;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EOAP.Plugin.Patcher
{
    // Seem to be invoked only once per save
    [HarmonyPatch(typeof(TownManager), nameof(TownManager.OpenInn))]
    public class TownManager_OpenInn 
    {
        private static EOPersistent _persistent;
        private static EOSession _session;
        public static void Prefix() 
        {
            _persistent = APBehaviour.GetPersistent();
            _session = APBehaviour.GetSession();
            if (_session.Connected)
            {
                GDebug.Log("Sync AP Data");
                _session.LoadFlags(_persistent);
            }
            else
            {
                GDebug.Log("Not connected to AP, cannot restore items");
            }
        }
        private static void Postfix()
        {
            APCanvasRipper.SetupTownReferences();

            //GameObject imgTest = new GameObject("NotificationRoot", Il2CppType.Of<RectTransform>());
            //var rectTr = imgTest.GetComponent<RectTransform>();
            //var img = imgTest.AddComponent<Image>();
            //rectTr.SetParent(APCanvasRipper.GameHUD.transform);
            //img.sprite = APCanvasRipper.NotificationSprite;
            //img.color = APCanvasRipper.NotificationSpriteColor;
            //img.SetNativeSize();
            //rectTr.sizeDelta = new Vector2(600f, 120f);
            //GameObject imgText = new GameObject("NotificationText", Il2CppType.Of<RectTransform>());
            //var textRectr = imgText.GetComponent<RectTransform>();
            //textRectr.SetParent(rectTr);
            //textRectr.anchorMin = Vector2.zero;
            //textRectr.anchorMax = Vector2.one;
            //textRectr.localPosition = Vector3.zero;
            //var text = imgText.AddComponent<Text>();
            //text.text = "Received Scramasax";
            //text.font = APCanvasRipper.Font01;
            //text.alignment = TextAnchor.MiddleCenter;
            //text.fontSize = 30;
            //text.horizontalOverflow = HorizontalWrapMode.Overflow;
            //text.verticalOverflow = VerticalWrapMode.Overflow;
            //var rect = APCanvasRipper.GameHUD.pixelRect;
            //rectTr.anchoredPosition = new Vector2(rect.width - rectTr.sizeDelta.x - 10f, rect.height * 0.5f - rectTr.sizeDelta.y);
        }
    }
}
