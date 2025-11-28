using System.Collections.Generic;

namespace EOAP.Plugin.AP
{
    public static class EOConfig
    {
        public static int StartingEntal = 1000; // default values
        public static bool ShopSanity = false;

        private static Dictionary<string, System.Action<long>> s_Configurations;

        public static void LoadSessionConfiguration(Dictionary<string,object> slotData)
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
            } catch(System.Exception e)
            {
                GDebug.LogError("Failed to load slot data properly:\n" + e.Message);
            }
        }
        static EOConfig()
        {
            s_Configurations = new Dictionary<string, System.Action<long>>()
            {
                { "starting_money", (v) => StartingEntal = (int) v },
                { "shop_sanity", (v) => ShopSanity = (v != 0) }
            };
        }

    }
}
