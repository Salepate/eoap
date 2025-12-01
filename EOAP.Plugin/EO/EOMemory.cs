using Dirt.Common;

namespace EOAP.Plugin.EO
{
    public static class EOMemory
    {
        public static int ShopMenuItemIndex = -1;
        public static bool[] ShopLocations = System.Array.Empty<bool>();
        public static string[] ShopHint = System.Array.Empty<string>();

        public static bool AllowLazyLoad { get; internal set; }

        public static void PrepareMemory()
        {
            ShopLocations = EnumUtility.CreateArray<bool, ItemNoEnum.ITEM_NO>();
            ShopHint = EnumUtility.CreateArray<string, ItemNoEnum.ITEM_NO>();
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
