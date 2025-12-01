using EOAP.Plugin.Behaviours;
using EOAP.Plugin.DB;
using System.Collections.Generic;

namespace EOAP.Plugin.AP
{
    public static class EOItems
    {
        public const string DynDBPath = "Bepinex/plugins/EOAPResources/dyndb.json";

        public const string ItemTable = "Items";
        public const string FlagTable = "Flags";

        public static readonly Dictionary<int, string> GameItems = new Dictionary<int, string>() {};
        public static readonly Dictionary<int, string> FlagLocations = new Dictionary<int, string>();

        public static Dictionary<long, System.Action<long>> CustomItems = new Dictionary<long, System.Action<long>>()
        {
            { 1000002, SendVictory },
            { 1000003, EntalReward }, // 500en
            { 1000004, EntalReward }, // 200en
            { 1000005, EntalReward }, // 100en
            { 1000008, EntalReward }, // 50en
            { 1000009, EntalReward }, // 1en
            // Unhandled yet
            //FIRST_STRATUM_CLEARED: 1000001,
            //NIGHT_10TP: 1000006, (note: disregard time)
            //FIRST_CHAR_10HP: 1000007,
        };



        public static void LoadDatabase()
        {
            Builder.Load(DynDBPath);
            List<Entry> itemTbl = Builder.GetTable("Items");
            List<Entry> flagTbl = Builder.GetTable("Flags");
            if (itemTbl == null)
            {
                GDebug.LogError("cannot load item db");
            }
            else
            {
                int curItemCount = GameItems.Count;
                for(int i = 0; i < itemTbl.Count; ++i)
                {
                    Entry entry = itemTbl[i];

                    if (entry.Data == null || entry.Data.Length < 1)
                        continue;

                    if (string.IsNullOrEmpty(entry.Data[0]))
                        continue;

                    if (!GameItems.ContainsKey(entry.ID))
                    {
                        GameItems.Add(entry.ID, entry.Data[0]);
                    }
                }

                int delta = GameItems.Count - curItemCount;
                GDebug.Log($"Registered {delta} items");
            }

            if (flagTbl == null)
            {
                GDebug.LogError("Unable to load flag db");
            }
            else
            {
                for(int i = 0; i< flagTbl.Count; ++i)
                {
                    Entry entry = flagTbl[i];

                    if (entry.Data == null || entry.Data.Length < 1)
                        continue;

                    if (string.IsNullOrEmpty(entry.Data[0]))
                        continue;
                    FlagLocations.Add(entry.ID, entry.Data[0]);
                }
                GDebug.Log($"Registered {FlagLocations.Count} flags");

            }
        }

        private static void SendVictory(long obj)
        {
            APBehaviour.GetSession().SendGoal();
        }

        private static void EntalReward(long value)
        {
            GoldItem.GiveGold((uint)value);
        }
    }
}
