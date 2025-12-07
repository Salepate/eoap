using Common;
using EOAP.Plugin.Behaviours;
using EOAP.Plugin.DB;
using EOAP.Plugin.EO;
using HarmonyLib;
using System.Collections.Generic;

namespace EOAP.Plugin.Patcher.Core
{
    public class Goal
    {
        public static void Patch(Harmony patcher)
        {
            patcher.Patch(typeof(Master.Event).GetMethod(nameof(Master.Event.SetEventMissionCompleteNo)),
                prefix: new HarmonyMethod(typeof(Goal), nameof(Event_SetEventMissionCompleteNo)));

            patcher.Patch(typeof(DungeonUtil).GetMethod(nameof(DungeonUtil.SetDiscoveryEnemy)),
                prefix: new HarmonyMethod(typeof(Goal), nameof(DungeonUtil_SetDiscoveryEnemy)));
        }
        public static void Event_SetEventMissionCompleteNo(int BLMCHKGFPGD)
        {
            EOSession session = APBehaviour.GetSession();
            if (EO1.Missions.TryGetValue((uint)BLMCHKGFPGD, out string loc))
            {
                if (EO1.MissionRewards.TryGetValue(BLMCHKGFPGD, out List<long> rewards))
                {
                    APBehaviour.GetPersistent().AddSkipItems(rewards);
                }
                session.SendLocation(loc);
            }

            if (EOConfig.GoalTypeMission && EOConfig.GoalMission == BLMCHKGFPGD)
            {
                APBehaviour.GetSession().SendGoal();
            }
        }

        public static void DungeonUtil_SetDiscoveryEnemy(EnemyNoEnum.ENEMY_NO BBFHNCKBEFJ)
        {
            int enemyIndex = (int)BBFHNCKBEFJ;
            EOPersistent persistent = APBehaviour.GetPersistent();
            if (EO1.GameEnemies.TryGetValue((int)BBFHNCKBEFJ, out Entry enemyEntry))
            {
                //string enemyName = enemyEntry.LocationName;
                if (!persistent.DefeatedEnemies.Contains(enemyIndex))
                {
                    persistent.DefeatedEnemies.Add(enemyIndex);
                }

                if (EOConfig.Goals.Contains(enemyEntry.LocationName))
                    APBehaviour.GetSession().CheckGoal();
            }
        }
    }
}
