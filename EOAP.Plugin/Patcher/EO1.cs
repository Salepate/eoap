using System.Collections.Generic;

namespace EOAP.Plugin.Patcher
{
    public static class EO1
    {
        // DB
        public static Dictionary<int, string> ItemIDToName = new Dictionary<int, string>()
        {
            { 1, "Knife" }
        };


        // API
        public static string GetShopLocation(int itemID)
        {
            if (ItemIDToName.TryGetValue(itemID, out string itemName))
                return string.Format("Shop: {0}", itemName);
            return string.Empty;
        }
    }
}
