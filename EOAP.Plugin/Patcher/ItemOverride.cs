using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Town;
using static ItemNoEnum;

namespace EOAP.Plugin.Patcher
{
    public class ItemOverride
    {
        public static int OverrideNextPurchase = -1;

        public static uint GetNewPrice(uint currentPrice)
        {
            if (EOConfig.UseOverride)
            {
                if (EOConfig.PriceOverride >= 0)
                    return (uint)EOConfig.PriceOverride;
                return currentPrice;
            }
            else
            {
                long newPrice = (currentPrice * EOConfig.PriceScale) / 100;
                return (uint)newPrice;
            }
        }
    }

    [HarmonyPatch(typeof(ShopBuyMenu), nameof(ShopBuyMenu.Buy))]
    public static class ShopBuyMenu_Buy
    {
        public static void Prefix(int GBKKIPAKICJ)
        {
            int itemID = GBKKIPAKICJ;
            GoldItem.GetBasicItemData((ITEM_NO)itemID, out GoldItem.BASIC_ITEM_DATA itemData);
            uint price = ItemOverride.GetNewPrice(itemData.GMDGMCMPOHE);
            ItemOverride.OverrideNextPurchase = (int)price;
        }
    }
        
    [HarmonyPatch(typeof(GoldItem), nameof(GoldItem.PayGold))]
    public class GoldItem_PayGold
    {
        public static void Prefix(ref uint FHPBLHJDMGC)
        {
            if (ItemOverride.OverrideNextPurchase != -1)
            {
                FHPBLHJDMGC = (uint)ItemOverride.OverrideNextPurchase;
                ItemOverride.OverrideNextPurchase = -1;
            }
        }
    }

    [HarmonyPatch(typeof(GoldItem), nameof(GoldItem.PayGold_WithoutSEPlay))]
    public class GoldItem_PayGold_WithoutSEPlay
    {
        public static void Prefix(ref uint FHPBLHJDMGC)
        {
            if (ItemOverride.OverrideNextPurchase != -1)
            {
                FHPBLHJDMGC = (uint)ItemOverride.OverrideNextPurchase;
                ItemOverride.OverrideNextPurchase = -1;
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
                    item.ELLNBOJNCPG = ItemOverride.GetNewPrice(uint.Parse(item.ELLNBOJNCPG)).ToString();
                }
                catch (System.Exception e)
                {
                    item.ELLNBOJNCPG = price;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.GetPrice), [typeof(ItemNoEnum.ITEM_NO)])]
    public class Item_GetPrice2
    {
        public static void Postfix(ref uint __result, Item __instance, ItemNoEnum.ITEM_NO KIPKFLDGMFM)
        {
            EOMemory.ShopMenuItemIndex = (int)KIPKFLDGMFM;
            __result = ItemOverride.GetNewPrice(__result);
        }
    }
}
