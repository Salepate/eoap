using System.Collections.Generic;
using static ResultManager;

namespace EOAP.Plugin.EO
{
    public static class EOConfig
    {
        // Fixed Configuration?
        public const int InventoryLimit = 60;
        // DBG Configuration
        public static bool PrintActivatedFlags = false;
        public static bool PrintTreasureBoxId = true;
        // YAML Configuration
        public static int GoalMission = 0; //
        public static bool GoalTypeMission = true;

        public static uint XPPercent; // experience modifier (%)
        public static int StartingEntal = 1000; // default values
        public static int PriceOverride = -1;
        public static int PriceScale = 100;
        public static bool UseOverride = false;

        public static bool ShopSanity = false;

        private static Dictionary<string, System.Action<long>> s_Configurations;

        public static void LoadSessionConfiguration(Dictionary<string, object> slotData)
        {
            try
            {
                foreach (KeyValuePair<string, object> entry in slotData)
                {
                    if (s_Configurations.TryGetValue(entry.Key, out var setter))
                    {
                        setter((long)entry.Value);
                    }
                    else
                    {
                        GDebug.LogError("unknown world settings: {0} ({1})", entry.Key, entry.Value);
                    }
                }
            }
            catch (System.Exception e)
            {
                GDebug.LogError("Failed to load slot data properly:\n" + e.Message);
            }
        }
        static EOConfig()
        {
            s_Configurations = new Dictionary<string, System.Action<long>>()
            {
                { "starting_money", (v) => StartingEntal = (int) v },
                { "shop_sanity", (v) => ShopSanity = v != 0 },
                { "price_mode", (v) => UseOverride = (int) v == 1 },
                { "price_percent_value", (v) => PriceScale = (int)v },
                { "price_flat_value", (v) => PriceOverride = (int)v },
                { "goal_mode", (v) => GoalTypeMission = v == 0 },
                { "goal_mission", (v) => GoalMission = (int)v },
                { "exp_percent_value", (v) => XPPercent = (uint)v }
            };
        }
    }
}
