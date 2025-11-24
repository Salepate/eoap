using Dirt.Common;

namespace EOAP.Plugin.AP
{
    public static class EOMemory
    {
        public static int ShopMenuItemIndex = -1;
        public static bool[] ShopLocations = System.Array.Empty<bool>();

        public static void PrepareMemory()
        {
            ShopLocations = EnumUtility.CreateArray<bool, ItemNoEnum.ITEM_NO>();
        }

        public static bool IsMissingLocation(ItemNoEnum.ITEM_NO no)
        {
            int itemIndex = (int)no;
            if (itemIndex >= 0 && itemIndex < ShopLocations.Length)
                return !ShopLocations[itemIndex];

            return false;
        }
    }
}
