using EOAP.Plugin.AP;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(Item), nameof(Item.GetPrice), [])]
    public class Item_GetPrice
    {
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

        public static void Postfix(ref uint __result, Item __instance)
        {
            EOMemory.ShopMenuItemIndex = (int)__instance.LCBEOKALFBN;
            __result = GetNewPrice(__result);
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.GetPrice), [typeof(ItemNoEnum.ITEM_NO)])]
    public class Item_GetPrice2
    {
        public static void Postfix(ref uint __result, Item __instance, ItemNoEnum.ITEM_NO KIPKFLDGMFM)
        {
            EOMemory.ShopMenuItemIndex = (int)KIPKFLDGMFM;
            __result = Item_GetPrice.GetNewPrice(__result);
        }
    }
}
