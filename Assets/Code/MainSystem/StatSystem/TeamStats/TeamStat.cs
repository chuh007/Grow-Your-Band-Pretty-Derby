using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Code.MainSystem.StatSystem.BaseStats;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.TeamStats
{
    public class TeamStat : AbstractStats
    {
        [SerializeField] private string statLabel;

        protected override async void Awake()
        {
            base.Awake();

            var statDataList = new List<StatData>();

            var handle = Addressables.LoadAssetsAsync<StatData>(
                statLabel,
                data => statDataList.Add(data)
            );

            await handle.Task;

            InitializeStats(statDataList);
        }

        private void InitializeStats(List<StatData> list)
        {
            foreach (var data in list)
            {
                if (Stats.ContainsKey(data.statType))
                    continue;

                Stats[data.statType] = new BaseStat(data);
            }
        }

        public BaseStat GetTeamStat(StatType statType)
        {
            return GetStat(statType);
        }

        public void TeamStatUpgrade(StatType statType, float successRate, float value)
        {
            float randValue = Random.Range(0f, 101f);
            if (randValue >= successRate)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
                return;
            }

            BaseStat stat = GetStat(statType);
            if (stat == null)
                return;

            stat.PlusValue((int)value);

            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
            Bus<TeamStatValueChangedEvent>.Raise(
                new TeamStatValueChangedEvent(statType, (int)value)
            );
        }
    }
}