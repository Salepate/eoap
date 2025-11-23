using System.IO;

namespace EOAP.Plugin.AP
{
    [System.Serializable]
    public class EOPersistent
    {
        public static string GetFilePath() => Path.Combine(UnityEngine.Application.persistentDataPath, "eoap.json");

        public int LastIndex = -1;
    }
}
