using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using InControl.UnityDeviceProfiles;
using System.Collections.Generic;
using Town;
using UnityEngine;

namespace EOAP.Plugin.Patcher
{
    using ITEM_NO = ItemNoEnum.ITEM_NO;

    [HarmonyPatch(typeof(ShopBuyMenu), nameof(ShopBuyMenu.Buy))]
    public class ShopBuyMenu_Patches
    {
        public static bool Prefix(ShopBuyMenu __instance, int GBKKIPAKICJ, bool ACINHONNBOB, int MEJKMPOGAEN = 1)
        {
            AP.EOSession session = APBehaviour.GetSession();
            // TODO: Apply this Patch only after connecting to a session
            if (session == null || !session.Connected)
                return true;

            bool invokeOriginal = true;

            int itemID = GBKKIPAKICJ;
            string loc = EO1.GetShopLocation(itemID);
            long locID = session.GetLocationId(loc);
            if (locID > 0)
            {
                bool sendLoc = false;
                AP.EOPersistent persistent = APBehaviour.GetPersistent();
                if (!persistent.CompleteLocations.Contains(locID))
                {
                    GoldItem.GetBasicItemData((ITEM_NO)itemID, out GoldItem.BASIC_ITEM_DATA itemData);
                    sendLoc = GoldItem.PayGold(itemData.GMDGMCMPOHE);
                    invokeOriginal = false; // no buy
                }

                if (sendLoc)
                {
                    EOMemory.ShopLocations[itemID] = true; // update fast mem
                    APBehaviour.GetSession().SendLocation(loc); // force resend loc anyway
                }
            }

            if (!invokeOriginal) // simulate non buy flow
            {
                __instance._BuyYesNo_b__60_0();
                __instance.alphaFadeController.FadeIn(0.2f);
            }

            return invokeOriginal;
        }
    }


    [HarmonyPatch(typeof(Town.ShopBuyMenu), nameof(Town.ShopBuyMenu.GetTabDataList))]
    public class ShopBuyMenu_GetTabDataList 
    { 
        public static async void Postfix(Il2CppSystem.Collections.Generic.List<ShopItemListController.Data> __result) 
        {
            EOSession session = APBehaviour.GetSession();
            EOPersistent persistent = APBehaviour.GetPersistent();

            if (session == null || !session.Connected)
                return;

            for (int i = 0; i < __result.Count; ++i)
            {
                ShopItemListController.Data item = __result[i];
                int itemID = item.GOGABMGPOND;
                string loc = EO1.GetShopLocation(itemID);
                long locID = session.GetLocationId(loc);
                if (locID > 0 && !persistent.CompleteLocations.Contains(locID))
                {
                    string hint = EOMemory.ShopHint[itemID] ?? $"AP Item {locID}";
                    item.CHMMJIGJPPC = hint;
                    item.CFMADGGDPDB = ItemType.eITEM_KIND.ITEM_KIND_ETC;
                    //session.Session.Items.GetItemName
                }
            }
            //Dictionary<long, ScoutedItemInfo> items = await session.Session.Locations.ScoutLocationsAsync(listLocs.ToArray());

            //for(int i = 0; i < locItems.Count; ++i)
            //{
            //    __result[locItems[i]].CHMMJIGJPPC = items[listLocs[i]].ItemDisplayName;
            //}

            //var dummyItem = new ShopItemListController.Data()
            //{
            //    CHMMJIGJPPC = "AP Item 01",  // Item Name
            //    ELLNBOJNCPG = "100",    // Cost
            //    CFMADGGDPDB = ItemType.eITEM_KIND.ITEM_KIND_SWORD,
            //    CHIIMALLGHF = false,     // Is Golden
            //    GGIPKAALBHF = -1,       // ??
            //    GOGABMGPOND = 4,        // Item ID
            //    MPHBKDOBEKA = false,     // Is Missing?
            //};
        } 
    }

    /// <summary>
    /// Patch used to prevent the "Select Slot" from appearing and breaking the flow
    /// Will allow display the "Cannot be equipped" notification, hinting this is a shop location
    /// </summary>
    [HarmonyPatch(typeof(ShopBuyMenu), nameof(ShopBuyMenu.GetEquippableItemsCount))]
    public class ShopBuyMenu_GetEquippableItemsCount
    {
        public static bool Prefix(ShopBuyMenu __instance, ref int __result)
        {
            if (EOMemory.IsMissingLocation((ItemNoEnum.ITEM_NO)EOMemory.ShopMenuItemIndex))
            {
                __result = 0; // will display "Cannot be equipped"
                return false;
            }
            return true;
        }
    }
}
