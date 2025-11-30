using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.Behaviours;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EOAP.Plugin.AP
{
    public class EOSession
    {
        public string ErrorMessage { get; private set; }
        public bool Connected { get; private set; }
        public ArchipelagoSession Session { get; private set; }

        private List<long> _idsBuffer = new List<long>();

        public void SendLocation(params string[] locations)
        {
            if (!Connected)
            {
                GDebug.Log("Cannot send locations AP, not connected");
                return;
            }
            _idsBuffer.Clear();
            EOPersistent persistentData = APBehaviour.GetPersistent();

            string firstCheckName = string.Empty;
            for(int i = 0; i < locations.Length; ++i)
            {
                long id = Session.Locations.GetLocationIdFromName(EO1.WorldName, locations[i]);
                if (id <= 0)
                {
                    GDebug.Log($"Unknown Location: {locations[i]}");
                }
                else
                {
                    if (string.IsNullOrEmpty(firstCheckName))
                        firstCheckName = locations[i];
                    _idsBuffer.Add(id);
                    //if (!persistentData.CompleteLocations.Contains(id))
                    //    APBehaviour.PushNotification($"Checked {locations[i]}");
                }
            }

            if (_idsBuffer.Count > 0)
            {
                if (_idsBuffer.Count == 1)
                    GDebug.Log($"Check - {firstCheckName}");
                else
                    GDebug.Log($"Check - {_idsBuffer.Count} checks found");

                Session.Locations.CompleteLocationChecks(_idsBuffer.ToArray());
                for (int i = 0; i < _idsBuffer.Count; ++i)
                    persistentData.AddLocation(_idsBuffer[i]);
            }
        }

        public ItemInfo GetItemInfo(long itemID)
        {
            ReadOnlyCollection<ItemInfo> allItems = Session.Items.AllItemsReceived;
            for (int i = 0; i < allItems.Count; ++i)
            {
                if (allItems[i].ItemId == itemID)
                    return allItems[i];
            }
            return null;
        }


        public void Start(string slotName, string hostname, int port)
        {
            if (Connected)
                return;

            ErrorMessage = string.Empty;
            Session = ArchipelagoSessionFactory.CreateSession(hostname, port);
            Session.MessageLog.OnMessageReceived += OnMessageReceived;
            System.Version worldVersion = new System.Version(0, 6, 4);
            LoginResult result;
            try
            {
                result = Session.TryConnectAndLogin("Etrian Odyssey HD", slotName, ItemsHandlingFlags.AllItems, requestSlotData: true, version: worldVersion);
            }
            catch(System.Exception e)
            {
                ErrorMessage += e.Message + "\n";
                result = new LoginFailure("Exception");
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                for (int i = 0; i < failure.Errors.Length; ++i)
                {
                    ErrorMessage += failure.Errors[i];
                }
                GDebug.Log(ErrorMessage);
            }
            else
            {
                LoginSuccessful success = (LoginSuccessful)result;
                EOConfig.LoadSessionConfiguration(success.SlotData);
                Session.Items.ItemReceived += OnItemReceived;
            }

            Connected = result.Successful;
        }


        public long GetLocationId(string loc)
        {
            if (string.IsNullOrEmpty(loc))
            {
                GDebug.Log("Invalid Location name");
                return 0;
            }

            long locid = Session.Locations.GetLocationIdFromName(EO1.WorldName, loc);
            if (locid > 0)
                return locid;

            GDebug.Log("Location " + loc + " not found in AP World");
            return 0;
        }



        // Sync (send/receive all locations already done)

        private void SyncItem(EOPersistent persistent, ItemInfo item, bool silent)
        {
            if (!silent)
            {
                APBehaviour.PushNotification($"Received {item.ItemDisplayName}", 3f);
            }

            persistent.PendingItems.Add(item.ItemId);
        }

        public void SyncNewItems(EOPersistent persistent, bool silent)
        {
            ReadOnlyCollection<ItemInfo> received = Session.Items.AllItemsReceived;

            for (int i = persistent.LastIndex + 1; i < received.Count; ++i)
            {
                ItemInfo item = received[i];
                SyncItem(persistent, item, silent);
                persistent.LastIndex = i;
            }
        }

        public void LoadFlags(EOPersistent persistent)
        {
            SyncNewItems(persistent, true);

            // update persistent data 
            for(int i = 0; i < Session.Locations.AllLocationsChecked.Count; ++i)
            {
                persistent.AddLocation(Session.Locations.AllLocationsChecked[i]);
            }

            // resend all locations
            Session.Locations.CompleteLocationChecks(persistent.CompleteLocations.ToArray());

            // prepare fast memory for ingame patches
            EOMemory.PrepareMemory();
            Dictionary<long, int> reverseMap = new Dictionary<long, int>();
            foreach(var shopLocKVP in EOItems.GameItems)
            {
                long locID = Session.Locations.GetLocationIdFromName(EO1.WorldName, EO1.GetShopLocation(shopLocKVP.Key));
                if (locID < 0)
                    continue;

                if (Session.Locations.AllLocationsChecked.Contains(locID))
                {
                    EOMemory.ShopLocations[shopLocKVP.Key] = true;
                }
                else
                {
                    reverseMap.Add(locID, shopLocKVP.Key);
                }
            }

            System.Threading.Tasks.Task<Dictionary<long, ScoutedItemInfo>> req = Session.Locations.ScoutLocationsAsync(reverseMap.Keys.ToArray());
            req.Wait();
            Dictionary<long, ScoutedItemInfo> result = req.Result;
            foreach(var hintKVP in result)
            {
                int itemIndex = reverseMap[hintKVP.Key];
                ScoutedItemInfo itemHint = hintKVP.Value;
                EOMemory.ShopHint[itemIndex] = itemHint.ItemDisplayName;
            }
        }

        // Multiclient Callbacks

        
        private void OnItemReceived(ReceivedItemsHelper helper)
        {
            EOPersistent persistent = APBehaviour.GetPersistent();

            if (helper.Index - 1 <= persistent.LastIndex)
                return;

            ItemInfo nextItem;
            while ((nextItem = helper.PeekItem()) != null)
            {
                SyncItem(persistent, nextItem, false);
                helper.DequeueItem();
            }
            persistent.LastIndex = helper.AllItemsReceived.Count - 1;
        }

        private void OnMessageReceived(LogMessage message)
        {
            if (message is ItemSendLogMessage sendLogMessage)
            {
                if (sendLogMessage is not HintItemSendLogMessage && sendLogMessage.IsSenderTheActivePlayer && !sendLogMessage.IsReceiverTheActivePlayer)
                {
                    string? itemName = sendLogMessage.Item.ItemName;
                    string? messageText;
                    string? otherPlayer = Session.Players.GetPlayerAlias(sendLogMessage.Receiver.Slot);
                    messageText = $"Sent {itemName} to {otherPlayer}.";
                    APBehaviour.PushNotification(messageText);
                }
            }
        }

    }
}
