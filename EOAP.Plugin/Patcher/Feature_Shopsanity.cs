using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Town;

namespace EOAP.Plugin.Patcher
{
    using ITEM_NO = ItemNoEnum.ITEM_NO;


    public static class ShopsanityFeature
    {
        public static void Patch(Harmony patcher)
        {
            GDebug.Log("Activating Shopsanity");
            patcher.Patch(typeof(ShopBuyMenu).GetMethod(nameof(ShopBuyMenu.Buy)),
                prefix: new HarmonyMethod(typeof(ShopsanityFeature), nameof(ShopsanityFeature.Buy_Prefix)));
            patcher.Patch(typeof(ShopBuyMenu).GetMethod(nameof(ShopBuyMenu.GetTabDataList)),
                postfix: new HarmonyMethod(typeof(ShopsanityFeature), nameof(ShopsanityFeature.GetTabDataList_Postfix)));
            patcher.Patch(typeof(ShopBuyMenu).GetMethod(nameof(ShopBuyMenu.GetEquippableItemsCount)),
                prefix: new HarmonyMethod(typeof(ShopsanityFeature), nameof(ShopsanityFeature.GetEquippableItemsCount_Prefix)));
        }

        public static bool Buy_Prefix(ShopBuyMenu __instance, int GBKKIPAKICJ, bool ACINHONNBOB, int MEJKMPOGAEN = 1)
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
                    uint price = Item_GetPrice.GetNewPrice(itemData.GMDGMCMPOHE);
                    sendLoc = GoldItem.PayGold(price);
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


        // Replace name in list
        public static void GetTabDataList_Postfix(Il2CppSystem.Collections.Generic.List<ShopItemListController.Data> __result)
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
                    item.PFDNJNEFKMK = true;
                }
            }
        }
        /// <summary>
        /// Patch used to prevent the "Select Slot" from appearing and breaking the flow
        /// Will allow display the "Cannot be equipped" notification, hinting this is a shop location
        /// </summary>
        public static bool GetEquippableItemsCount_Prefix(ShopBuyMenu __instance, ref int __result)
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
