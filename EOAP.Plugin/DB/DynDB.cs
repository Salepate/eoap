using System;
using System.Collections.Generic;
using static Master.MasterManager;

namespace EOAP.Plugin.DB
{
    [System.Serializable]
    public class DynDB
    {
        public Dictionary<string, List<Entry>> Tables;

        public DynDB()
        {
            Tables = new Dictionary<string, List<Entry>>();
        }

        public List<Entry> GetTable(string tbl)
        {
            if (!Tables.TryGetValue(tbl, out var entries))
            {
                entries = new List<Entry>();
                Tables.Add(tbl, entries);
            }
            return entries;
        }


        public static bool GetEntry(in List<Entry> table, int entryID, out int tableIndex)
        {
            for(int i = 0; i < table.Count; ++i)
            {
                if (table[i].ID == entryID)
                {
                    tableIndex = i;
                    return true;
                }
            }
            tableIndex = -1;
            return false;
        }

        internal void SortByID()
        {
            var oldTables = Tables;
            Tables = new Dictionary<string, List<Entry>>();
            foreach(var table in oldTables)
            {
                List<Entry> copyEntries = new List<Entry>();
                copyEntries.AddRange(table.Value);
                copyEntries.Sort((a, b) => a.ID - b.ID);
                Tables.Add(table.Key, copyEntries);
            }
        }
    }


    [System.Serializable]
    public struct Entry
    {
        public int ID;
        public string[] Data;
    }
}
