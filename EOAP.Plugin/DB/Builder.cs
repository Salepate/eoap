using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace EOAP.Plugin.DB
{
    public static class Builder
    {
        private static DynDB s_DB;
        static Builder()
        {
            s_DB = new DynDB();
        }

        public static List<Entry> GetTable(string tbl)
        {
            if (s_DB != null)
            {
                return s_DB.GetTable(tbl);
            }
            return null;
        }

        public static void Load(string path)
        {
            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                s_DB = JsonConvert.DeserializeObject<DynDB>(content);
                s_DB.TrimNonUTF8();
                GDebug.Log("loaded dyndb");
            }
            else
            {
                GDebug.Log("dyndb not found");
            }
        }
        public static void Dump(string path)
        {
            s_DB.SortByID();
            string jsonDB = JsonConvert.SerializeObject(s_DB, Formatting.Indented);
            File.WriteAllText(path, jsonDB);
            int entries = 0;
            foreach(var tbl in s_DB.Tables)
            {
                entries += tbl.Value.Count;
            }
            GDebug.Log($"saved dyndb / tables : {s_DB.Tables.Count} / totale entries {entries}");
        }

        public static void Push(string db, int id, string additionalData = "")
        {
            List<Entry> tbl = s_DB.GetTable(db);
            if (!DynDB.GetEntry(tbl, id, out int tblIndex))
            {
                tbl.Add(new Entry() { ID = id, Info = additionalData });
            }
        }
    }
}
