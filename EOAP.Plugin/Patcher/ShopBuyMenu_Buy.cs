using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
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
            ArchipelagoSession sess = APBehaviour.GetSession();
            ILocationCheckHelper locs = sess.Locations;

            if (!string.IsNullOrEmpty(loc))
            {
                long id = sess.Locations.GetLocationIdFromName("Etrian Odyssey HD", loc);
                if (id > 0)
                {
                    GDebug.Log("Send Check - " + loc);
                    sess.Locations.CompleteLocationChecks(id);
                }
                else
                {
                    GDebug.Log("Unknown Location - " + loc);
                }
            }
        }
    }
}
