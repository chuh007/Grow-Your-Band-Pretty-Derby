using System.Collections.Generic;
using UnityEngine;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.MemberStats
{
    public class MemberStat : AbstractStats
    {
        [SerializeField] protected List<StatData> memberStatData;
        
        protected readonly Dictionary<StatType, BaseStat> MemberStats = new();
        
        protected override void Awake()
        {
            base.Awake();

            foreach (var data in memberStatData)
            {
                BaseStat stat = new BaseStat(data);
                MemberStats[data.statType] = stat;
            }

            Bus<TeamStatValueChangedEvent>.OnEvent += OnTeamStatChanged;
        }

        private void OnDestroy()
        {
            Bus<TeamStatValueChangedEvent>.OnEvent -= OnTeamStatChanged;
        }

        private void OnTeamStatChanged(TeamStatValueChangedEvent evt)
        {
            BaseStat stat = GetStat(evt.StatType);
            if (stat != null)
            {
                stat.PlusValue(evt.AddValue);
            }
        }

        public void MemberStatUpgrade(StatType statType, float failureValue)
        {
            float randValue = Random.Range(0f, 101f);
            if (randValue <= failureValue)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
            }
            else
            {
                BaseStat stat = MemberStats.GetValueOrDefault(statType);
                if (stat != null)
                {
                    stat.PlusValue(100);
                    Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
                }
            }
        }

        public BaseStat GetMemberStat(StatType statType)
        {
            return MemberStats.GetValueOrDefault(statType);  
        }

        public BaseStat GetStat(StatType statType)
        {
            return MemberStats.GetValueOrDefault(statType) 
                   ?? CommonStats.GetValueOrDefault(statType);
        }
    }
}
