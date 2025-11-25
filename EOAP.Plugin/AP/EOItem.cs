namespace EOAP.Plugin.AP
{
    public struct EOItem
    {
        public static EOItem GameItem(long uid, uint amount = 1) => new EOItem() { ID = uid, Amount = amount, Type = EOItemType.GameItem };
        public static EOItem Ental(uint amount = 1) => new EOItem() { ID = 0, Amount = amount, Type = EOItemType.Ental };
        public static EOItem TBox(long uid) => new EOItem() { ID = uid, Type = EOItemType.TreasureBoxFlag };

        public long ID;
        public uint Amount;
        public EOItemType Type;
    }
}
