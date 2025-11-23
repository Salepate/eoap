using Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace EOAP.Plugin.DB
{
    public static class Builder
    {
        private static DynDB m_DB;
        private  static JsonConverter m_Converter;
        static Builder()
        {
            m_DB = new DynDB();
        }

        public static void Load(string path)
        {
            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                m_DB = JsonConvert.DeserializeObject<DynDB>(content);
                GDebug.Log("loaded dyndb");
            }
            else
            {
                GDebug.Log("dyndb not found");
            }
        }
        public static void Dump(string path)
        {
            m_DB.SortByID();
            string jsonDB = JsonConvert.SerializeObject(m_DB, Formatting.Indented);
            File.WriteAllText(path, jsonDB);
            int entries = 0;
            foreach(var tbl in m_DB.Tables)
            {
                entries += tbl.Value.Count;
            }
            GDebug.Log($"saved dyndb / tables : {m_DB.Tables.Count} / totale entries {entries}");
        }

        public static void Push(string db, int id, params string[] additionalData)
        {
            List<Entry> tbl = m_DB.GetTable(db);
            if (!DynDB.GetEntry(tbl, id, out int tblIndex))
            {
                tbl.Add(new Entry() { ID = id, Data = additionalData });
            }
        }
    }
}
