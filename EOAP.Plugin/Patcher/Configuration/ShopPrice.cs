using EOAP.Plugin.EO;
using HarmonyLib;
using Town;

namespace EOAP.Plugin.Patcher.Configuration
{
    using ITEM_NO = ItemNoEnum.ITEM_NO;

    [HarmonyPatch(typeof(ShopBuyMenu), nameof(ShopBuyMenu.Buy))]
    public static class ShopBuyMenu_Buy
    {
        public static void Prefix(int GBKKIPAKICJ)
        {
            int itemID = GBKKIPAKICJ;
            GoldItem.GetBasicItemData((ITEM_NO)itemID, out GoldItem.BASIC_ITEM_DATA itemData);
            uint price = EO1.GetNewPrice(itemData.GMDGMCMPOHE);
            EO1.OverrideNextPurchase = (int)price;
        }
    }

    [HarmonyPatch(typeof(GoldItem), nameof(GoldItem.PayGold))]
    public class GoldItem_PayGold
    {
        public static void Prefix(ref uint FHPBLHJDMGC)
        {
            if (EO1.OverrideNextPurchase != -1)
            {
                FHPBLHJDMGC = (uint)EO1.OverrideNextPurchase;
                EO1.OverrideNextPurchase = -1;
            }
        }
    }

    [HarmonyPatch(typeof(GoldItem), nameof(GoldItem.PayGold_WithoutSEPlay))]
    public class GoldItem_PayGold_WithoutSEPlay
    {
        public static void Prefix(ref uint FHPBLHJDMGC)
        {
            if (EO1.OverrideNextPurchase != -1)
            {
                FHPBLHJDMGC = (uint)EO1.OverrideNextPurchase;
                EO1.OverrideNextPurchase = -1;
            }
        }
    }

    [HarmonyPatch(typeof(ShopSellMenu), nameof(ShopSellMenu.GetTabDataList))]
    public class Item_ShowSellPrice
    {
        // Replace price in list
        public static void Postfix(Il2CppSystem.Collections.Generic.List<ShopItemListController.Data> __result, ShopBuyMenu __instance)
        {
            for (int i = 0; i < __result.Count; ++i)
            {
                ShopItemListController.Data item = __result[i];
                string price = item.ELLNBOJNCPG;
                try
                {
                    item.ELLNBOJNCPG = EO1.GetNewPrice(uint.Parse(item.ELLNBOJNCPG)).ToString();
                }
                catch (System.Exception e)
                {
                    item.ELLNBOJNCPG = price;
                    GDebug.LogError(e.Message);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.GetPrice), [typeof(ITEM_NO)])]
    public class Item_GetPrice2
    {
        public static void Postfix(ref uint __result, Item __instance, ITEM_NO KIPKFLDGMFM)
        {
            EOMemory.ShopMenuItemIndex = (int)KIPKFLDGMFM;
            __result = EO1.GetNewPrice(__result);
        }
    }
}
