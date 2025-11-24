using System.Collections.Generic;
using System.IO;

namespace EOAP.Plugin.AP
{
    [System.Serializable]
    public class EOPersistent
    {
        public static string GetFilePath() => Path.Combine(UnityEngine.Application.persistentDataPath, "eoap.json");

        public void AddLocation(long locID)
        {
            if (!CompleteLocations.Contains(locID))
                CompleteLocations.Add(locID);
        }

        public int LastIndex = -1;
        public List<long> CompleteLocations = new List<long>();
    }
}
