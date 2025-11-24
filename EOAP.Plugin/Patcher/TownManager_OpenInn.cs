using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using System;

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
                // connect callback
                _session.Session.Items.ItemReceived += OnItemReceived;
            }
            else
            {
                GDebug.Log("Not connected to AP, cannot restore items");
            }
        }

        private static void OnItemReceived(ReceivedItemsHelper helper)
        {
            //ItemInfo nextItem;
            //while ( (nextItem = helper.PeekItem()) != null)
            //{
            //    _session.SyncItem(nextItem, false);

            //    helper.DequeueItem();
            //}
            //_persistent.LastIndex = helper.AllItemsReceived.Count - 1;
        }
    }
}
