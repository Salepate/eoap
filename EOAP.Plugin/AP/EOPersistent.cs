using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.Behaviours;
using System.Collections.Generic;
using System.IO;

namespace EOAP.Plugin.AP
{
    [System.Serializable]
    public class EOPersistent
    {

        public bool IsGoal;
        public int LastIndex = -1;
        public List<long> PendingItems = new List<long>();
        public List<long> CompleteLocations = new List<long>();

        public List<long> ItemsToSkip = new List<long>();
        public List<int> EntalToSkip = new List<int>();

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
                if (GoldItem.GetPartyCarrySameItem((ItemNoEnum.ITEM_NO)ItemsToSkip[i]) > 0)
                {
                    GoldItem.DeletePartyItem((ItemNoEnum.ITEM_NO)ItemsToSkip[i]);
                    ItemsToSkip.RemoveAt(i);
                }
            }

            while(PendingItems.Count > 0 && GoldItem.GetPartyCarryItemAllNum() < EOConfig.InventoryLimit)
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
            else if (EOItems.CustomItems.TryGetValue(itemIndex, out var customAddAction))
            {
                customAddAction(itemIndex);
            }
            else 
            {
                ItemInfo item = APBehaviour.GetSession().GetItemInfo(itemIndex);
                GDebug.Log($"Unsupported Item {item.ItemDisplayName}");
            }
        }

        public void AddSkipItems(int index, Dictionary<int, List<long>> itemDic = null, Dictionary<int, long> enDic = null)
        {
            if (itemDic != null && itemDic.TryGetValue(index, out List<long> itemRewards))
            {
                ItemsToSkip.AddRange(itemRewards);
            }

            if (enDic != null && enDic.TryGetValue(index, out long entalValue))
            {
                EntalToSkip.Add((int)entalValue);
            }
        }
    }
}
