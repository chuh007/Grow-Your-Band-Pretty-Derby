using Code.MainSystem.StatSystem.BaseStats;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.TeamStats
{
    public class TeamStat : AbstractStats
    {
        public BaseStat GetTeamStat(StatType statType)
        {
            return GetCommonStat(statType);
        }
        
        public void TeamStatUpgrade(StatType statType, float failureRate)
        {
            float randValue = UnityEngine.Random.Range(0f, 101f);
            if (randValue <= failureRate)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
            }
            else
            {
                BaseStat stat = GetCommonStat(statType);
                if (stat != null)
                {
                    stat.PlusValue(100);
                    Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
                    Bus<TeamStatValueChangedEvent>.Raise(
                        new TeamStatValueChangedEvent(statType, 100)
                    );
                }
            }
        }
    }
}