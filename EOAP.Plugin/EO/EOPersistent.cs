using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.Behaviours;
using System.Collections.Generic;
using System.IO;

namespace EOAP.Plugin.EO
{
    [System.Serializable]
    public class EOPersistent
    {

        public bool IsGoal;
        public int LastIndex = -1;
        public List<long> PendingItems = new List<long>();
        public List<long> CompleteLocations = new List<long>();
        public List<long> DefeatedEnemies = new List<long>(); // until i find how to check save data

        public List<long> ItemsToSkip = new List<long>();

        public static string GetFilePath(string seed, int slot)
        {
            string fileName = $"{seed}_{slot}.json";
            return Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
        }

        public void AddLocation(long locID)
        {
            if (!CompleteLocations.Contains(locID))
                CompleteLocations.Add(locID);
        }


        public void InjectItems()
        {
            int addedItems = 0;

            for (int i = ItemsToSkip.Count - 1; i >= 0; --i)
            {
                long id = ItemsToSkip[i];
                if (id > (long)ItemNoEnum.ITEM_NO.ITEM_NOT && id < (long)ItemNoEnum.ITEM_NO.ITEM_END)
                {
                    GoldItem.DeletePartyItem((ItemNoEnum.ITEM_NO)ItemsToSkip[i]);
                    ItemsToSkip.RemoveAt(i);
                }
                else if (EO1.CustomItemsDespawn.TryGetValue(id, out var despawner))
                {
                    despawner(id);
                    ItemsToSkip.RemoveAt(i);
                }
            }

            while (PendingItems.Count > 0 && GoldItem.GetPartyCarryItemAllNum() < EOConfig.InventoryLimit)
            {
                AddGameItem(PendingItems[0]);
                PendingItems.RemoveAt(0);
                ++addedItems;
            }

            if (addedItems > 0)
            {
                GDebug.Log($"Synced {addedItems} new items");
            }
        }

        public void AddGameItem(long itemIndex)
        {
            if (itemIndex > (long)ItemNoEnum.ITEM_NO.ITEM_NOT && itemIndex < (long)ItemNoEnum.ITEM_NO.ITEM_END)
            {
                GoldItem.AddPartyItem((ItemNoEnum.ITEM_NO)itemIndex);
            }
            else if (EO1.CustomItems.TryGetValue(itemIndex, out var customAddAction))
            {
                customAddAction(itemIndex);
            }
            else
            {
                ItemInfo item = APBehaviour.GetSession().GetItemInfo(itemIndex);
                GDebug.Log($"Unsupported Item {item.ItemDisplayName}");
            }
        }

        public void AddSkipItems(IList<long> itemIDs)
        {
            ItemsToSkip.AddRange(itemIDs);
        }
    }
}
