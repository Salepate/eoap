using System;
using System.Collections.Generic;
using System.Text;

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

        internal void TrimNonUTF8()
        {
            foreach(var tbl in Tables)
            {
                for(int j = 0; j < tbl.Value.Count; ++j)
                {
                    var entry = tbl.Value[j];
                    bool trim = false;
                    var str = entry.LocationName;
                    trim = string.IsNullOrEmpty(str) || str.Length != Encoding.UTF8.GetByteCount(str);
                    if (trim)
                    {
                        tbl.Value.RemoveAt(j--);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct Entry
    {
        public int ID; // game id
        public long Location;
        public string LocationName;
        public long[] Items;
        public string Info;
        public bool HasItem => Items != null && Items.Length > 0;
        public bool IsValid
        {
            get
            {
                return ID > 0 && Location > 0;
            }
        }
    }
}
