using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using EOAP.Plugin.EO;
using HarmonyLib;
using Master;

namespace EOAP.Plugin.Patcher
{
    //[HarmonyPatch(typeof(EventFlagTbl), nameof(EventFlagTbl.GetEventFlag))]
    //public class FlagDB
    //{
    //    public static void Postfix(bool __result, int IINBFPLGLMN)
    //    {
    //        Builder.Push("Flags", IINBFPLGLMN);
    //    }
    //}

    [HarmonyPatch(typeof(EventFlagTbl), nameof(EventFlagTbl.SetEventFlag))]
    public class EventFlagTbl_SetEventFlag
    {
        public static void Postfix(int IINBFPLGLMN, bool LHIBOLMPPOI)
        {
            if (LHIBOLMPPOI)
            {

                if (EO1.FlagLocations.TryGetValue(IINBFPLGLMN, out string locStr))
                {
                    APBehaviour.GetPersistent().AddSkipItems(IINBFPLGLMN, EO1.EventRewards);
                    APBehaviour.GetSession().SendLocation(locStr);
                }
                APDebug.PrintActivateFlag(IINBFPLGLMN);
            }
        }
    }
}
