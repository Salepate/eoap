using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.AP;
using System.Collections.Generic;

namespace EOAP.Plugin
{
    public static class EO1
    {
        public const string WorldName = "Etrian Odyssey HD";
        // Locations
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

        public static readonly Dictionary<uint, string> Missions = new Dictionary<uint, string>()
        {
            {0, "Adventurers Initiation - Complete" }
        };

        // Event Reward Skips
        public static Dictionary<int, List<long>> EventRewards = new Dictionary<int, List<long>>()
        {
            { 127, [4021] } // B1F Main - Moles Whitestone (Whitestone)
        };

        public static Dictionary<int, List<long>> MissionRewards = new Dictionary<int, List<long>>()
        {
            {0, [4373, 4373] } // Radha's Note (x2 dont know why)
        };
        public static Dictionary<int, long> MissionEnRewards = new Dictionary<int, long>()
        {
            { 0, 500 } // Adventurer's Initiation (500en)
        };

        // API

        public static void PlaySFX(APCanvasRipper.SFX sfx)
        {
            string sfxName = APCanvasRipper.SFXPath[(int)sfx];
            SoundManager.playSE(sfxName);
        }
        public static string GetShopLocation(int itemID)
        {
            if (EOItems.GameItems.TryGetValue(itemID, out string itemName))
            {
                if (Item.IsMedicine((ItemNoEnum.ITEM_NO)itemID))
                    return string.Format("Apothecary - {0}", itemName);
                return string.Format("Shop - {0}", itemName);
            }
            return string.Empty;
        }

        public static bool TryGetTreasureBoxLocation(uint tboxID, out string location) => TreasureBox.TryGetValue(tboxID, out location);
    }
}
