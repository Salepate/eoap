using Newtonsoft.Json;
using System;
using System.IO;

namespace EOAP.Plugin.AP
{
    [System.Serializable]
    public class APConnection
    {
        public static string GetFilePath() => "ap_connection.json";

        public string Hostname;
        public string Slotname;
        public string Password;

        public static bool FileExists { get; internal set; }

        public void SaveConnectionFile()
        {
            if (!File.Exists(GetFilePath()))
            {
                try
                {
                    string content = JsonConvert.SerializeObject(this);
                    File.WriteAllText(GetFilePath(), content);

                }
                catch (System.Exception e)
                {
                    GDebug.Log("Could not load AP connection file " + GetFilePath());
                    GDebug.Log(e.Message);
                }
            }
        }

        public static APConnection LoadConnectionFile()
        {
            if (File.Exists(GetFilePath()))
            {
                try
                {
                    string content = File.ReadAllText(GetFilePath());
                    return JsonConvert.DeserializeObject<APConnection>(content);

                } catch(System.Exception e)
                {
                    GDebug.Log("Could not load AP connection file " + GetFilePath());
                    GDebug.Log(e.Message);
                }
            }

            return new APConnection()
            {
                Hostname = "localhost",
                Slotname = "Player1",
                Password = ""
            };
        }

        internal static void CreateDefaultSaveFile()
        {
            if (!File.Exists(GetFilePath()))
            {
                APConnection file = LoadConnectionFile();
                file.SaveConnectionFile();
            }
        }
    }
}
