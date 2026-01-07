using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamTrainingType : ITrainingType, ITeamStatChangeProvider
    {
        public string GetIdleImageKey()
        {
            return "Concert/Image/Idle";
        }

        public string GetResultImageKey(bool isSuccess)
        {
            return isSuccess ? "Sprites/Guitar/Succse" : "Sprites/Guitar/Faill"; 
        }

        public string GetProgressImageKey()
        {
            return "Concert/Image/Progress";
        }

        public string GetIdlePrefabKey()
        {
            return "Concert/SD/Idle"; 
        }

        public string GetResultUIPrefabKey()
        {
            return "Concert/UI/Result"; 
        }
        
        public Dictionary<UnitDataSO, List<(string name, Sprite icon, int baseValue, int delta)>> GetAllStatChanges(
            List<UnitDataSO> units,
            StatManager statManager,
            bool isSuccess)
        {
            var result = new Dictionary<UnitDataSO, List<(string name, Sprite icon, int baseValue, int delta)>>();

            foreach (var unit in units)
            {
                int baseHarmony = statManager.GetTeamStat(StatType.TeamHarmony).CurrentValue;
                int delta = isSuccess ? 5 : 0;

                Sprite icon = statManager.GetTeamStat(StatType.TeamHarmony).StatIcon;

                result[unit] = new List<(string, Sprite, int, int)>
                {
                    ("하모니", icon, baseHarmony, delta)
                };
            }

            return result;
        }
    }
}