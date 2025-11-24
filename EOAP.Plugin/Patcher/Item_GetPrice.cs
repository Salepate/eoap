using EOAP.Plugin.AP;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(Item), nameof(Item.GetPrice), [])]
    public class Item_GetPrice
    {
        public static void Postfix(Item __instance)
        {
            EOMemory.ShopMenuItemIndex = (int)__instance.LCBEOKALFBN;
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.GetPrice), [typeof(ItemNoEnum.ITEM_NO)])]
    public class Item_GetPrice2
    {
        public static void Postfix(Item __instance, ItemNoEnum.ITEM_NO KIPKFLDGMFM)
        {
            EOMemory.ShopMenuItemIndex = (int)KIPKFLDGMFM;
        }
    }
}
