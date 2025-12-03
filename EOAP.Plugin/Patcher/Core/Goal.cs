using EOAP.Plugin.Behaviours;
using EOAP.Plugin.EO;
using HarmonyLib;

namespace EOAP.Plugin.Patcher.Core
{
    [HarmonyPatch(typeof(Master.Event), nameof(Master.Event.SetEventMissionCompleteNo))]
    public class Goal
    {
        public static void Prefix(int BLMCHKGFPGD)
        {
            EOSession session = APBehaviour.GetSession();
            if (EO1.Missions.TryGetValue((uint)BLMCHKGFPGD, out string loc))
            {
                APBehaviour.GetPersistent().AddSkipItems(BLMCHKGFPGD, EO1.MissionRewards, EO1.MissionEnRewards);
                session.SendLocation(loc);
            }

            if (EOConfig.GoalTypeMission && EOConfig.GoalMission == BLMCHKGFPGD)
            {
                APBehaviour.GetSession().SendGoal();
            }
        }
    }
}
