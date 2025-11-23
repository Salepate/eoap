using System.Collections.Generic;

namespace EOAP.Plugin
{
    public static class EO1
    {
        public const string WorldName = "Etrian Odyssey HD";
        // DB
        public static Dictionary<int, string> ItemIDToName = new Dictionary<int, string>()
        {
            { 1, "Knife" },
        };

        public static Dictionary<uint, string> TreasureBox = new Dictionary<uint, string>()
        {
            { 0, "B1F East: North Chest" },
            { 1, "B1F Main: A3 Top Chest" },
            { 2, "B1F Clear Crystal Room: Chest" },
            { 3, "B1F Main: A3 Middle Chest" },
            { 4, "B1F Main: A3 Bottom Chest" },
            { 5, "B1F East: Ragelope Top Chest" },
            { 6, "B1F East: Ragelope Middle Chest" },
            { 7, "B1F Violet Crystal Room: Top Chest"},
            { 8, "B1F Violet Crystal Room: Bottom Chest"},
            { 9 , "B1F East: Ragelope Bottom Chest" }
        };


        // API
        public static string GetShopLocation(int itemID)
        {
            if (ItemIDToName.TryGetValue(itemID, out string itemName))
                return string.Format("Shop: {0}", itemName);
            return string.Empty;
        }

        public static bool TryGetTreasureBoxLocation(uint tboxID, out string location) => TreasureBox.TryGetValue(tboxID, out location);
    }
}
