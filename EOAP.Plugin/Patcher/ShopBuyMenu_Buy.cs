using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Town;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(ShopBuyMenu), nameof(ShopBuyMenu.Buy))]
    public class ShopBuyMenu_Buy
    {
        public static void Prefix(int GBKKIPAKICJ)
        {
            int itemID = GBKKIPAKICJ;
            string loc = EO1.GetShopLocation(itemID);
            if (!string.IsNullOrEmpty(loc))
                APBehaviour.GetSession().SendLocation(loc);
        }
    }
}
