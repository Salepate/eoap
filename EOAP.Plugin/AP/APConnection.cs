using Newtonsoft.Json;
using System.IO;
using System.Linq.Expressions;

namespace EOAP.Plugin.AP
{
    [System.Serializable]
    public class APConnection
    {
        public static string GetFilePath() => "ap_connection.json";

        public string Hostname;
        public string Slotname;

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
                Slotname = "Player1"
            };
        }
    }
}
