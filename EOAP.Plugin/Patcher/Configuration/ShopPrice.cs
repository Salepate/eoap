using EOAP.Plugin.EO;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Town;

namespace EOAP.Plugin.Patcher.Configuration
{
    using ITEM_NO = ItemNoEnum.ITEM_NO;


    public class ShopPrice
    {
        private static Il2CppSystem.Reflection.FieldInfo TupleField = Il2CppInterop.Runtime.Il2CppType.Of<Il2CppSystem.ValueTuple<ItemNoEnum.ITEM_NO, int>>().GetField("Item1");

        public static void Patch(Harmony patcher)
        {
            patcher.Patch(typeof(GoldItem).GetMethod(nameof(GoldItem.PayGold)),
                prefix: new HarmonyMethod(typeof(ShopPrice), nameof(GoldItem_PayGold_Prefix)));
            patcher.Patch(typeof(Item).GetMethod(nameof(Item.GetPrice), [typeof(ITEM_NO)]),
                postfix: new HarmonyMethod(typeof(ShopPrice), nameof(Item_GetPrice_Postfix)));
            patcher.Patch(typeof(GoldItem).GetMethod(nameof(GoldItem.PayGold_WithoutSEPlay)),
                prefix: new HarmonyMethod(typeof(ShopPrice), nameof(GoldItem_PayGold_Prefix)));
            patcher.Patch(typeof(Item).GetMethod(nameof(Item.GetSellPrice)),
                prefix: new HarmonyMethod(typeof(ShopPrice), nameof(Item_GetSellPrice_Prefix)));
            patcher.Patch(typeof(ShopSellMenu).GetMethod(nameof(ShopSellMenu.GetTabDataList)),
                postfix: new HarmonyMethod(typeof(ShopPrice), nameof(Item_ShowSellPrice_Postfix)));
            patcher.Patch(typeof(ShopBuyMenu).GetMethod(nameof(ShopBuyMenu.Buy)),
                prefix: new HarmonyMethod(typeof(ShopPrice), nameof(ShopBuyMenu_Buy_Prefix)));
            patcher.Patch(typeof(ShopSellMenu).GetMethod(nameof(ShopSellMenu.SellYesNo)),
                prefix: new HarmonyMethod(typeof(ShopPrice), nameof(ShopSellMenu_SellYesNo_Prefix)));

        }

        public static void GoldItem_PayGold_Prefix(ref uint FHPBLHJDMGC)
        {
            if (EO1.OverrideNextPurchase != -1)
            {
                FHPBLHJDMGC = (uint)EO1.OverrideNextPurchase;
                EO1.OverrideNextPurchase = -1;
            }
        }

        public static void Item_GetPrice_Postfix(ref uint __result, Item __instance, ITEM_NO KIPKFLDGMFM)
        {
            if (!EOMemory.IsLoading)
            {
                EOMemory.ShopMenuItemIndex = (int)KIPKFLDGMFM;
                __result = EO1.GetNewPrice(__result);
            }
        }

        public static bool Item_GetSellPrice_Prefix(out uint __result, ItemNoEnum.ITEM_NO KIPKFLDGMFM)
        {
            if (!EOMemory.IsLoading)
            {
                uint price = EOMemory.GetItem(KIPKFLDGMFM).SellPrice;
                if (!EOMemory.GetItem(KIPKFLDGMFM).Material)
                    price = EO1.GetNewPrice(price);
                __result = price;
                return false;
            }
            __result = 0;
            return true;
        }

        // Replace price in list
        public static void Item_ShowSellPrice_Postfix(List<ShopItemListController.Data> __result, ShopBuyMenu __instance)
        {
            for (int i = 0; i < __result.Count; ++i)
            {
                ShopItemListController.Data item = __result[i];
                string price = item.ELLNBOJNCPG;
                try
                {
                    if (!EOMemory.GetItem(item.GOGABMGPOND).Material)
                        item.ELLNBOJNCPG = EO1.GetNewPrice(uint.Parse(item.ELLNBOJNCPG)).ToString();
                }
                catch (System.Exception e)
                {
                    item.ELLNBOJNCPG = price;
                    GDebug.LogError(e.Message);
                }
            }
        }

        // Override next time gold is deduced from your inv
        public static void ShopBuyMenu_Buy_Prefix(int GBKKIPAKICJ)
        {
            int itemID = GBKKIPAKICJ;
            GoldItem.GetBasicItemData((ITEM_NO)itemID, out GoldItem.BASIC_ITEM_DATA itemData);
            uint price = EO1.GetNewPrice(itemData.GMDGMCMPOHE);
            EO1.OverrideNextPurchase = (int)price;
        }

        // Recompute total gain accounting including shop price modifier
        public static void ShopSellMenu_SellYesNo_Prefix(List<Il2CppSystem.ValueTuple<ItemNoEnum.ITEM_NO, int>> JPHEOAEEEKH, ref uint LKMFNLBGDAG)
        {
            uint saleSum = 0;
            for (int i = 0; i < JPHEOAEEEKH.Count; ++i)
            {
                ItemNoEnum.ITEM_NO itemIndex = (TupleField.GetValue(JPHEOAEEEKH[0]).Unbox<ItemNoEnum.ITEM_NO>());
                ref ItemCustom customData = ref EOMemory.GetItem(itemIndex);
                uint price = customData.SellPrice;
                if (!customData.Material)
                    price = EO1.GetNewPrice(price);
                saleSum += price;
            }
            LKMFNLBGDAG = saleSum;
        }
    }
}
