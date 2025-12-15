using Code.MainSystem.StatSystem.BaseStats;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.TeamStats
{
    public class TeamStat : AbstractStats
    {
        public BaseStat GetTeamStat(StatType statType)
        {
            return GetStat(statType);
        }
        
        public void TeamStatUpgrade(StatType statType, float successRate, float value)
        {
            float randValue = UnityEngine.Random.Range(0f, 101f);
            if (randValue >= successRate)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
            }
            else
            {
                BaseStat stat = GetStat(statType);
                if (stat == null) 
                    return;
                
                stat.PlusValue((int)value);
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
                Bus<TeamStatValueChangedEvent>.Raise(
                    new TeamStatValueChangedEvent(statType, 100)
                );
            }
        }
    }
}