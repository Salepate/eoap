using EOAP.Plugin.Behaviours;
using EOAP.Plugin.DB;
using EOAP.Plugin.EO;
using HarmonyLib;
using System.Linq;
using Town;

namespace EOAP.Plugin.Patcher.Feature
{
    using ITEM_NO = ItemNoEnum.ITEM_NO;
    public static class ShopsanityFeature
    {
        public static void Patch(Harmony patcher)
        {
            GDebug.Log("Activating Shopsanity");
            patcher.Patch(typeof(ShopBuyMenu).GetMethod(nameof(ShopBuyMenu.Buy)),
                prefix: new HarmonyMethod(typeof(ShopsanityFeature), nameof(Buy_Prefix)));
            patcher.Patch(typeof(ShopBuyMenu).GetMethod(nameof(ShopBuyMenu.GetTabDataList)),
                postfix: new HarmonyMethod(typeof(ShopsanityFeature), nameof(GetTabDataList_Postfix)));
            patcher.Patch(typeof(ShopBuyMenu).GetMethod(nameof(ShopBuyMenu.GetEquippableItemsCount)),
                prefix: new HarmonyMethod(typeof(ShopsanityFeature), nameof(GetEquippableItemsCount_Prefix)));
        }

        public static bool Buy_Prefix(ShopBuyMenu __instance, int GBKKIPAKICJ, bool ACINHONNBOB, int MEJKMPOGAEN = 1)
        {
            EOSession session = APBehaviour.GetSession();
            // TODO: Apply this Patch only after connecting to a session
            if (session == null || !session.Connected)
                return true;

            bool invokeOriginal = true;
            int itemID = GBKKIPAKICJ;
            ref ItemCustom itemData = ref EOMemory.GetItem(itemID);

            
            if (EO1.GameItems.TryGetValue(itemID, out Entry locInfo))
            {
                bool sendLoc = false;
                EOPersistent persistent = APBehaviour.GetPersistent();
                if (!persistent.CompleteLocations.Contains(locInfo.Location))
                {
                    GoldItem.GetBasicItemData((ITEM_NO)itemID, out GoldItem.BASIC_ITEM_DATA itemDataBase);
                    uint price = EO1.GetNewPrice(itemDataBase.GMDGMCMPOHE);
                    sendLoc = GoldItem.PayGold(price);
                    invokeOriginal = false; // no buy
                }

                if (sendLoc)
                {
                    itemData.Checked = true;
                    APBehaviour.GetSession().SendLocation(locInfo.LocationName); // force resend loc anyway
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
                if (EO1.GameItems.TryGetValue(itemID, out Entry locInfo) && !persistent.CompleteLocations.Contains(locInfo.Location))
                {
                    ref ItemCustom itemData = ref EOMemory.GetItem(itemID);
                    string hint = !string.IsNullOrEmpty(itemData.Hint) ? itemData.Hint : $"AP Item {locInfo.Location}";
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
            if (EOMemory.IsMissingLocation((ITEM_NO)EOMemory.ShopMenuItemIndex))
            {
                __result = 0; // will display "Cannot be equipped"
                return false;
            }
            return true;
        }
    }
}
