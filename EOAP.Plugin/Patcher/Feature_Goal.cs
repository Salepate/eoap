using Camp;
using EOAP.Plugin.AP;
using EOAP.Plugin.Behaviours;
using HarmonyLib;

namespace EOAP.Plugin.Patcher
{

    [HarmonyPatch(typeof(CampMenuController), nameof(CampMenuController.SetDecideKeepList))]
    public class Feature_Goal_RipUI
    {
        private static bool _ripped = false;
        public static void Prefix()
        {
            if (!_ripped)
            {
                Shinigami.SetupPauseMenuReferences();
                APBehaviour.UI.CreateCheckText();
                _ripped = true;
            }
            APBehaviour.UI.RefreshChecks();
        }
    }

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
                APBehaviour.GetSession().SendGoal();
            }
        }
    }
}
