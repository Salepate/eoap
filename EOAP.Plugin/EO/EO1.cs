using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using EOAP.Plugin.DB;
using Master;
using System.Collections.Generic;

namespace EOAP.Plugin.EO
{
    public static class EO1
    {
        public const string WorldName = "Etrian Odyssey HD";
        public const string DynDBPath = "Bepinex/plugins/Resources/dyndb.json";
        public const string ItemTable = "Items";
        public const string FlagTable = "Flags";
        public const string EnemyTable = "Enemies";


        public static readonly HashSet<int> AutoFlags = new HashSet<int>()
        {
            3072,   // Town first time visit
            3073,   // The Rooster Inn first time visit,
            608,    // The Rooster Inn first time dialogue
            3076,   // Explorer Guild first time visit
            656,    // Ceft Apothecary first time dialogue
            3074,   // Ceft Apothecary first time visit
            672,    // Golden Deer Pub first time dialogue
            3077,   // Golden Deer Pub first time visit
            624,    // Shilleka's good first time dialogue
            3075,   // Shilleka's good first time visit
            3079,   // Forest Entrance first time visit
            3078,   // Radha Hall first time visit
            592,    // Radha Hall first time dialogue
            112,    // forest warning
            640,    // Some other dialogue
            643,    // guild name set
            644,    // going yolo?
            120,    // Adventurer initiation flag
            116,    // soldier awaiting you
        };

        public static readonly Dictionary<int, Entry> GameItems = new Dictionary<int, Entry>() { };
        public static readonly Dictionary<int, Entry> GameEnemies = new Dictionary<int, Entry>();
        public static readonly Dictionary<int, Entry> FlagLocations = new Dictionary<int, Entry>();
        public static Dictionary<long, System.Action<long>> CustomItems = new Dictionary<long, System.Action<long>>()
        {
            { 1000002, SendVictory },
            { 1000003, (v) => EntalReward(500) }, // 500en
            { 1000004, (v) => EntalReward(200) }, // 200en
            { 1000005, (v) => EntalReward(100) }, // 100en
            { 1000006, (v) => EntalReward(50) }, // 50en
            { 1000007, (v) => EntalReward(1) }, // 1en
            { 1000008, (v) => EntalReward(400) }, // 400en

            // Unhandled yet
            //FIRST_STRATUM_CLEARED: 1000001,
            //NIGHT_10TP: 1000006, (note: disregard time)
            //FIRST_CHAR_10HP: 1000007,
        };

        public static Dictionary<uint, long> EntalItems = new Dictionary<uint, long>()
        {
            { 500, 1000003 },
            { 200, 1000004 },
            { 100, 1000005 },
            { 50, 1000006 },
            { 1, 1000007 },
            { 400, 1000008 },
        };

        public static Dictionary<long, System.Action<long>> CustomItemsDespawn = new Dictionary<long, System.Action<long>>()
        {
            { 1000003, (v) => EntalPenalty(500) }, 
            { 1000004, (v) => EntalPenalty(200) },
            { 1000005, (v) => EntalPenalty(100) },
            { 1000006, (v) => EntalPenalty(50) },
            { 1000007, (v) => EntalPenalty(1) },
            { 1000008, (v) => EntalPenalty(400) },
        };

        // Locations
        public static readonly Dictionary<uint, string> TreasureBox = new Dictionary<uint, string>() {};

        public static readonly Dictionary<uint, string> Missions = new Dictionary<uint, string>()
        {
            {0, "Adventurers Initiation - Complete" }
        };

        public static Dictionary<int, List<long>> MissionRewards = new Dictionary<int, List<long>>()
        {
            {0, [4373, 4373, 1000003] } // Radha's Note (x2 dont know why)
        };

        public static void LoadDatabase()
        {
            Builder.Load(DynDBPath);
            List<Entry> itemTbl = Builder.GetTable(ItemTable);
            List<Entry> flagTbl = Builder.GetTable(FlagTable);
            List<Entry> enemyTable = Builder.GetTable(EnemyTable);

            if (itemTbl == null)
            {
                GDebug.LogError("cannot load item db");
            }
            else
            {
                int curItemCount = GameItems.Count;
                for (int i = 0; i < itemTbl.Count; ++i)
                {
                    Entry entry = itemTbl[i];

                    if (!entry.IsValid)
                        continue;

                    if (!GameItems.ContainsKey(entry.ID))
                    {
                        GameItems.Add(entry.ID, entry);
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
                for (int i = 0; i < flagTbl.Count; ++i)
                {
                    Entry entry = flagTbl[i];

                    if (!entry.IsValid)
                        continue;

                    FlagLocations.Add(entry.ID, entry);
                }
                GDebug.Log($"Registered {FlagLocations.Count} flags");
            }

            if (enemyTable == null)
            {
                GDebug.LogError("Unable to load enemy db");
            }
            else
            {
                for (int i = 0; i < enemyTable.Count; ++i)
                {
                    Entry entry = enemyTable[i];
                    if (string.IsNullOrEmpty(entry.LocationName))
                        continue;
                    GameEnemies.Add(entry.ID, entry);
                }
                GDebug.Log($"Registered {GameEnemies.Count} enemies");
            }
        }



        // API
        public static int OverrideNextPurchase = -1;
        public static uint GetNewPrice(uint currentPrice)
        {
            if (EOConfig.UseOverride)
            {
                if (EOConfig.PriceOverride >= 0)
                    return (uint)EOConfig.PriceOverride;
                return currentPrice;
            }
            else
            {
                long newPrice = (currentPrice * EOConfig.PriceScale) / 100;
                return (uint)newPrice;
            }
        }

        public static void LoadAutoFlags()
        {
            foreach(var autoflag in AutoFlags)
            {
                EventFlagTbl.SetEventFlag(autoflag, true);
            }
        }
        public static void PlaySFX(Shinigami.SFX sfx)
        {
            string sfxName = Shinigami.SFXPath[(int)sfx];
            SoundManager.playSE(sfxName);
        }
        // Callbacks
        private static void SendVictory(long obj)
        {
            APBehaviour.GetSession().SendGoal();
        }

        private static void EntalReward(long value)
        {
            GoldItem.GiveGold((uint)value);
        }

        private static void EntalPenalty(long value)
        {
            GoldItem.PayGold_WithoutSEPlay((uint)value);
        }
    }
}
