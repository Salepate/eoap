using Archipelago.MultiClient.Net.Models;
using EOAP.Plugin.Behaviours;
using System.Collections.Generic;
using System.IO;

namespace EOAP.Plugin.AP
{
    [System.Serializable]
    public class EOPersistent
    {
        public static string GetFilePath() => Path.Combine(UnityEngine.Application.persistentDataPath, "eoap.json");

        public void AddLocation(long locID)
        {
            if (!CompleteLocations.Contains(locID))
                CompleteLocations.Add(locID);
        }


        public void ProcessPendingItems()
        {
            if (PendingItems.Count > 0)
                GDebug.Log($"Adding {PendingItems.Count} items from AP");
            for(int i = 0; i < PendingItems.Count; ++i)
            {
                AddGameItem(PendingItems[i]);
            }
            PendingItems.Clear();
        }

        public void AddGameItem(long itemIndex)
        {
            if (itemIndex > (long)ItemNoEnum.ITEM_NO.ITEM_NOT && itemIndex < (long)ItemNoEnum.ITEM_NO.ITEM_END)
            {
                GoldItem.AddPartyItem((ItemNoEnum.ITEM_NO)itemIndex);
            }
            else
            {
                ItemInfo item = APBehaviour.GetSession().GetItemInfo(itemIndex);
                GDebug.Log($"Unsupported Item {item.ItemDisplayName}");
            }
        }


        public int LastIndex = -1;
        public List<long> PendingItems = new List<long>();
        public List<long> CompleteLocations = new List<long>();
    }
}
