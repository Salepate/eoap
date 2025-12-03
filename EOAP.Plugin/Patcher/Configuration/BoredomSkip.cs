using HarmonyLib;

namespace EOAP.Plugin.Patcher.Configuration
{
    /// <summary>
    /// Skips dialogues, tutorials 
    /// </summary>
    public class BoredomSkip
    {
        public static void Patch(Harmony patcher)
        {
            patcher.Patch(typeof(CampData).GetMethod(nameof(CampData.CampData_GetFirstEnterFlg)),
                prefix: new HarmonyMethod(typeof(BoredomSkip), nameof(CampData_GetFirstEnterFlg_Prefix)));
        }

        private static bool CampData_GetFirstEnterFlg_Prefix(out bool __result)
        {
            __result = true;
            return false;
        }
    }
}
