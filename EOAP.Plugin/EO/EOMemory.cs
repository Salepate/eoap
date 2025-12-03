using Dirt.Common;

namespace EOAP.Plugin.EO
{
    public static class EOMemory
    {
        public static int ShopMenuItemIndex = -1;

        public static ItemCustom Invalid = new ItemCustom() { Checked = true, Hint = "Invalid", Infinite = false };
        public static ItemCustom[] Items;
        public static bool AllowLazyLoad { get; internal set; }

        static EOMemory()
        {
            Items = EnumUtility.CreateArray<ItemCustom, ItemNoEnum.ITEM_NO>();
        }
        public static ref ItemCustom GetItem(int no) => ref GetItem((ItemNoEnum.ITEM_NO)no);
        public static ref ItemCustom GetItem(ItemNoEnum.ITEM_NO no)
        {
            if (no >= 0 && (int) no < Items.Length)
                return ref Items[(int)no];
            return ref Invalid;
        }

        public static bool IsMissingLocation(ItemNoEnum.ITEM_NO no) => !GetItem(no).Checked;
    }
}
