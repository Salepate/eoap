using System;

namespace EOAP.Plugin
{
    public static class GDebug
    {
        public static void Log(string message, params object[] data)
        {
            Plugin.Log.LogInfo(string.Format(message, data));
        }

        public static void LogError(string message, params object[] data)
        {
            Plugin.Log.LogError(string.Format(message, data));
        }
    }
}
