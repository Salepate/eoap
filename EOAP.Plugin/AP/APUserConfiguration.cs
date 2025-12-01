using Newtonsoft.Json;
using System.IO;

namespace EOAP.Plugin.AP
{
    [System.Serializable]
    public class APUserConfiguration
    {
        public static string GetFilePath() => "ap_connection.json";

        public string Hostname;
        public string Slotname;
        public string Password;
        public bool DebugUtils;
        public bool FastQuit;

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

        public static APUserConfiguration LoadConnectionFile()
        {
            if (File.Exists(GetFilePath()))
            {
                try
                {
                    string content = File.ReadAllText(GetFilePath());
                    return JsonConvert.DeserializeObject<APUserConfiguration>(content);

                } catch(System.Exception e)
                {
                    GDebug.Log("Could not load AP connection file " + GetFilePath());
                    GDebug.Log(e.Message);
                }
            }

            return new APUserConfiguration()
            {
                Hostname = "localhost",
                Slotname = "Player1",
                Password = "",
                DebugUtils = false
            };
        }

        internal static void CreateDefaultSaveFile()
        {
            if (!File.Exists(GetFilePath()))
            {
                APUserConfiguration file = LoadConnectionFile();
                file.SaveConnectionFile();
            }
        }
    }
}
