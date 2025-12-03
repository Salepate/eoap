using EOAP.Plugin.EO;
using HarmonyLib;

namespace EOAP.Plugin.Patcher.Configuration
{
    /// <summary>
    /// Prevent items from being consumed when used
    /// Bug: item will disappear from the list but on reopening it will be restored
    /// </summary>
    [HarmonyPatch(typeof(GoldItem), nameof(GoldItem.DeletePartyItem))]
    public class InfiniteFlyWing
    {
        public static readonly ItemNoEnum.ITEM_NO[] InfiniteItems = [
            ItemNoEnum.ITEM_NO.ITEM_OTHER_319, // Ward Chime
            ItemNoEnum.ITEM_NO.ITEM_OTHER_320, // Gold Chime
            ItemNoEnum.ITEM_NO.ITEM_OTHER_374, // Ariadne thread
        ]; 

        public static void Patch(Harmony patcher)
        {
            for(int i = 0; i < InfiniteItems.Length; ++i)
            {
                ref ItemCustom itemData = ref EOMemory.GetItem(InfiniteItems[i]);
                itemData.Infinite = true;
            }

            patcher.Patch(typeof(GoldItem).GetMethod(nameof(GoldItem.DeletePartyItem)),
                prefix: new HarmonyMethod(typeof(InfiniteFlyWing), nameof(DeletePartyItem_Prefix)));
        }

        public static bool DeletePartyItem_Prefix(ItemNoEnum.ITEM_NO GLCJECALNDH)
        {
            ref ItemCustom itemData = ref EOMemory.GetItem(GLCJECALNDH);
            return !itemData.Infinite;
        }
    }
}