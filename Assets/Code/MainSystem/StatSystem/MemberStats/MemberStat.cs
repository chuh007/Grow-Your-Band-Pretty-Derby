using System.Collections.Generic;
using UnityEngine;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.StatSystem.MemberStats
{
    public class MemberStat : AbstractStats
    {
        [SerializeField] private MemberType memberType;
        [SerializeField] protected List<StatData> memberStatData;
        
        protected readonly Dictionary<StatType, BaseStat> MemberStats = new();
        
        public MemberType MemberType => memberType;
        
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

        public void MemberStatUpgrade(StatType statType, float successRate, float value)
        {
            float randValue = Random.Range(0f, 101f);
            if (randValue >= successRate)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
            }
            else
            {
                BaseStat stat = MemberStats.GetValueOrDefault(statType);
                if (stat != null)
                {
                    stat.PlusValue((int)value);
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
