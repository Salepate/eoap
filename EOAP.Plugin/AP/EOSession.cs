using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
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
                }
            }

            if (_idsBuffer.Count > 0)
            {
                if (_idsBuffer.Count == 1)
                    GDebug.Log($"Check - {firstCheckName}");
                else
                    GDebug.Log($"Check - {_idsBuffer.Count} checks found");
                Session.Locations.CompleteLocationChecks(_idsBuffer.ToArray());
            }

        }

        public void Start(string slotName, string hostname, int port)
        {
            if (Connected)
                return;

            ErrorMessage = string.Empty;
            Session = ArchipelagoSessionFactory.CreateSession(hostname, port);

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


        // Sync (send/receive all locations already done)
        public void LoadFlags(EOPersistent persistent)
        {
            ReadOnlyCollection<ItemInfo> received = Session.Items.AllItemsReceived;

            for(int i = persistent.LastIndex + 1; i < received.Count; ++i)
            {
                ItemInfo item = received[i];
                if (item.ItemId > (long) ItemNoEnum.ITEM_NO.ITEM_NOT && item.ItemId < (long)ItemNoEnum.ITEM_NO.ITEM_END)
                {
                    GoldItem.AddPartyItem((ItemNoEnum.ITEM_NO)item.ItemId);
                }
                else
                {
                    GDebug.Log("Unsupported Item (yet): " + item.ItemDisplayName + " ("+item.ItemId+")"); 
                }
                persistent.LastIndex = i;
            }
        }
    }
}
