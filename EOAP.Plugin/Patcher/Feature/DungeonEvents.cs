using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using EOAP.Plugin.DB;
using EOAP.Plugin.EO;
using HarmonyLib;
using Master;

namespace EOAP.Plugin.Patcher.Feature
{
    [HarmonyPatch(typeof(EventFlagTbl), nameof(EventFlagTbl.SetEventFlag))]
    public class EventFlagTbl_SetEventFlag
    {
        public static void Postfix(int IINBFPLGLMN, bool LHIBOLMPPOI)
        {
            if (LHIBOLMPPOI)
            {
                if (EO1.FlagLocations.TryGetValue(IINBFPLGLMN, out Entry locEntry))
                {
                    APBehaviour.GetPersistent().AddSkipItems(locEntry.Items);
                    APBehaviour.GetSession().SendLocation(locEntry.LocationName);
                }
#if AP_DEBUG
                APDebug.PrintActivateFlag(IINBFPLGLMN);
#endif
            }
        }
    }
}
