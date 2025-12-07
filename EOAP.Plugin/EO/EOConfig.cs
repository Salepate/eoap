using Newtonsoft.Json.Linq;
using StableNameDotNet;
using System;
using System.Collections.Generic;

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
        public static bool GoalTypeMission = false;

        public static uint XPPercent = 100; // experience modifier (%)
        public static int StartingEntal = 1000; // default values
        public static int PriceOverride = -1;
        public static int PriceScale = 100;
        public static bool UseOverride = false;
        public static List<string> Goals = new List<string>();

        public static bool ShopSanity = false;

        private static Dictionary<string, System.Action<long>> s_LongParameters;
        private static Dictionary<string, Action<string>> s_StringParameters;

        public static void LoadSessionConfiguration(Dictionary<string, object> slotData)
        {
            Goals.Clear();
            try
            {
                foreach (KeyValuePair<string, object> entry in slotData)
                {
                    if (entry.Value is JArray jarray)
                    {
                        for(int i = 0; i < jarray.Count; ++i)
                        {
                            HandleSingleParameter(entry.Key, jarray[i].ToObject(typeof(object)));
                        }
                    }
                    else
                    {
                        HandleSingleParameter(entry.Key, entry.Value);
                    }
                }
            }
            catch (System.Exception e)
            {
                GDebug.LogError("Failed to load slot data properly:\n" + e.Message);
            }
            if (Goals.Count == 0)
            {
                GDebug.LogError($"No valid goal detected, please check your configuration");
            }
            else
            {
                GDebug.Log($"Player has {Goals.Count} goal{(Goals.Count > 1 ? "s" : "")}: {Goals.Join(",")} ");
            }
        }

        private static void HandleSingleParameter(string paramName, object data)
        {
            if (s_LongParameters.TryGetValue(paramName, out var setterLong))
            {
                setterLong((long)data);
            }
            else if (s_StringParameters.TryGetValue(paramName, out var setterStr))
            {
                setterStr((string)data);
            }
            else
            {
                GDebug.LogError("unknown world settings: {0} ({1})", paramName, data);
            }
        }
        static EOConfig()
        {
            s_LongParameters = new Dictionary<string, System.Action<long>>()
            {
                { "starting_money", (v) => StartingEntal = (int) v },
                { "shop_sanity", (v) => ShopSanity = v != 0 },
                { "price_mode", (v) => UseOverride = (int) v == 1 },
                { "price_percent_value", (v) => PriceScale = (int)v },
                { "price_flat_value", (v) => PriceOverride = (int)v },
                { "exp_modifier", (v) => XPPercent = (uint)v },
            };

            s_StringParameters = new Dictionary<string, System.Action<string>>()
            {
                { "goal", Goals.Add }
            };
        }
    }
}
