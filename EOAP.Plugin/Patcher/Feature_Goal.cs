using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{

    [HarmonyPatch(typeof(Master.Event), nameof(Master.Event.SetEventMissionCompleteNo))]
    public class Event_SetEventMissionComplete
    { 
        public static void Prefix(int BLMCHKGFPGD)
        {
            AP.EOSession session = APBehaviour.GetSession();
            if (EO1.Missions.TryGetValue((uint) BLMCHKGFPGD, out string loc))
            {
                APBehaviour.GetPersistent().AddSkipItems(BLMCHKGFPGD, EO1.MissionRewards, EO1.MissionEnRewards);
                session.SendLocation(loc);
            }

            if (EOConfig.GoalTypeMission && EOConfig.GoalMission == BLMCHKGFPGD)
            {
                APBehaviour.GetSession().Session.SetGoalAchieved();
            }
        }
    }
}
