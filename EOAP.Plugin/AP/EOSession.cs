using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.Behaviours;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                result = new LoginFailure("Exception"); // no idea why an exception message should be passed.
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result; // seriously wtf
                for (int i = 0; i < failure.Errors.Length; ++i)
                {
                    ErrorMessage += failure.Errors[i];
                }
                GDebug.Log(ErrorMessage);
            }
            else
            {
                LoginSuccessful success = (LoginSuccessful)result; // devs really need to study polymorphism use cases
                GDebug.Log($"Connected to AP Slot {success.Slot}");
            }

            Connected = result.Successful;
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


        // Sync (send/receive all locations already done)
        public void SyncItem(ItemInfo item, bool silent)
        {
            if (!silent)
            {
                APBehaviour.PushNotification($"Received {item.ItemDisplayName}", 3f);
            }

            if (item.ItemId > (long)ItemNoEnum.ITEM_NO.ITEM_NOT && item.ItemId < (long)ItemNoEnum.ITEM_NO.ITEM_END)
            {
                GoldItem.AddPartyItem((ItemNoEnum.ITEM_NO)item.ItemId);
            }
            else
            {
                GDebug.Log("Unsupported Item (yet): " + item.ItemDisplayName + " (" + item.ItemId + ")");
            }
        }

        public void SyncNewItems(EOPersistent persistent, bool silent)
        {
            ReadOnlyCollection<ItemInfo> received = Session.Items.AllItemsReceived;

            for (int i = persistent.LastIndex + 1; i < received.Count; ++i)
            {
                ItemInfo item = received[i];
                SyncItem(item, silent);
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
            foreach(var shopLocKVP in EO1.ItemIDToName)
            {
                long locID = Session.Locations.GetLocationIdFromName(EO1.WorldName, EO1.GetShopLocation(shopLocKVP.Key));
                if (Session.Locations.AllLocationsChecked.Contains(locID))
                {
                    GDebug.Log("Flagging " + shopLocKVP.Value);
                    EOMemory.ShopLocations[shopLocKVP.Key] = true;
                }
            }
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
    }
}
