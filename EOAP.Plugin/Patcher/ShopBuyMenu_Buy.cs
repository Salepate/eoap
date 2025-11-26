using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Town;
using UnityEngine;

namespace EOAP.Plugin.Patcher
{
    using ITEM_NO = ItemNoEnum.ITEM_NO;

    [HarmonyPatch(typeof(ShopBuyMenu), nameof(ShopBuyMenu.Buy))]
    public class ShopBuyMenu_Buy
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
}
