using EOAP.Plugin.AP;
using System.Collections.Generic;

namespace EOAP.Plugin
{
    public static class EO1
    {
        public const string WorldName = "Etrian Odyssey HD";
        // Locations
        public static readonly Dictionary<int, string> ItemIDToName = new Dictionary<int, string>()
        {
            // Weapon: Swords
            { 1, "Knife" },
            { 2, "Scramasax" },
            { 3, "Dagger" },
            // Weapon: Staves
            { 40, "Wand"},
            { 54, "Staff" },
            // Weapon: Axes
            { 50, "Hatchet" },
            // Weapon: Bows
            { 88, "Wood Bow" },
            // Weapon: Whips
            { 112, "Light Whip" },
            // Armor: Chests
            { 1001, "Tweed" },
            // Armor: Shields
            { 2038, "Targe" },
            // Armor: Headgears
            { 2001, "Hairpin" },
            // Armor: Gloves
            { 2018, "Knit Glove" },
            // Armor: Footgears
            { 2056, "Leaf Boot" },
            // Armor: Accessories
            { 3001, "Hide Belt" },
            // Consumable - Shop
            { 4319, "Ward Chime" },
            { 4374, "Ariadne Thread"}
        };

        public static readonly Dictionary<uint, string> TreasureBox = new Dictionary<uint, string>()
        {
            { 0, "B1F East - North Chest" },
            { 1, "B1F Main - A3 Top Chest" },
            { 2, "B1F Clear Crystal Room - Chest" },
            { 3, "B1F Main - A3 Middle Chest" },
            { 4, "B1F Main - A3 Bottom Chest" },
            { 5, "B1F East - Ragelope Top Chest" },
            { 6, "B1F East - Ragelope Middle Chest" },
            { 7, "B1F Violet Crystal Room - Top Chest"},
            { 8, "B1F Violet Crystal Room - Bottom Chest"},
            { 9 , "B1F East - Ragelope Bottom Chest" }
        };

        // API
        public static void PlaySFX(APCanvasRipper.SFX sfx)
        {
            string sfxName = APCanvasRipper.SFXPath[(int)sfx];
            SoundManager.playSE(sfxName);
        }
        public static string GetShopLocation(int itemID)
        {
            if (ItemIDToName.TryGetValue(itemID, out string itemName))
                return string.Format("Shop - {0}", itemName);
            return string.Empty;
        }

        public static bool TryGetTreasureBoxLocation(uint tboxID, out string location) => TreasureBox.TryGetValue(tboxID, out location);
    }
}
