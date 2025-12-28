using System.Collections.Generic;
using System.Threading.Tasks;
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

        private bool _initialized;

        protected override void Awake()
        {
            base.Awake();
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            if (string.IsNullOrEmpty(statLabel))
            {
                return;
            }

            var statDataList = new List<StatData>();

            var handle = Addressables.LoadAssetsAsync<StatData>(
                statLabel,
                data => statDataList.Add(data)
            );

            await handle.Task;
            InitializeStats(statDataList);
            _initialized = true;
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
            return _initialized ? GetStat(statType) : null;
        }

        public void ApplyTeamStatIncrease(float value)
        {
            if (!_initialized)
            {
                return;
            }

            BaseStat stat = GetStat(StatType.TeamHarmony);
            if (stat == null)
                return;

            stat.PlusValue((int)value);

            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
            Bus<TeamStatValueChangedEvent>.Raise(
                new TeamStatValueChangedEvent(StatType.TeamHarmony, (int)value)
            );
        }
    }
}
