using EOAP.Plugin.AP;
using HarmonyLib;
using Town;

namespace EOAP.Plugin.Patcher
{
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
