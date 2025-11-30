using Camp;
using DungeonData;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;
using Master;
using System;
using static Master.Event;

namespace EOAP.Plugin.Patcher
{
    public class Feature_ItemSync
    {
        public static Tuple<Type, string>[] Injectors = new Tuple<Type, string>[]
        {
            Tuple.Create(typeof(CampMenuController), nameof(CampMenuController.SetDecideKeepList)),
            Tuple.Create(typeof(EventCheck), nameof(EventCheck.EventCheckk))
        };

        public static void Patch(Harmony patcher)
        {
            HarmonyMethod injector = new HarmonyMethod(typeof(Feature_ItemSync), nameof(Feature_ItemSync.Prefix));
            for(int i = 0; i < Injectors.Length; ++i)
            {
                Tuple<Type, string> tuple = Injectors[i];
                patcher.Patch(tuple.Item1.GetMethod(tuple.Item2), prefix: injector);
            }
        }
        public static void Prefix()
        {
            APBehaviour.GetPersistent().InjectItems();
        }
    }

    [HarmonyPatch(typeof(GoldItem), nameof(GoldItem.AddPartyItem))]
    public class GoldItem_AddPartyItem
    {
        public static bool Prefix(ItemNoEnum.ITEM_NO GLCJECALNDH)
        {
            EOPersistent persistent = APBehaviour.GetPersistent();
            int idx = persistent.ItemsToSkip.IndexOf((int)GLCJECALNDH);
            bool skipBase = false;
            if (idx != -1)
            {
                skipBase = true;
                persistent.ItemsToSkip.RemoveAt(idx);
            }
            return !skipBase;
        }
    }

    [HarmonyPatch(typeof(GoldItem), nameof(GoldItem.GiveGold))]
    public class GoldItem_GiveGold
    {
        public static bool Prefix(uint HAKGGLOIKMP)
        {
            EOPersistent persistent = APBehaviour.GetPersistent();
            int idx = persistent.EntalToSkip.IndexOf((int)HAKGGLOIKMP);
            bool skipBase = false;
            if (idx != -1)
            {
                skipBase = true;
                persistent.EntalToSkip.RemoveAt(idx);
            }
            return !skipBase;
        }
    }

}
