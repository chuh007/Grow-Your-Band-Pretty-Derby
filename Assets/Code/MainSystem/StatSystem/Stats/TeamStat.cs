using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using UnityEngine;

namespace Code.MainSystem.StatSystem.Stats
{
    public class TeamStat : AbstractStats
    {
        [SerializeField] private string statLabel;

        private bool _initialized;

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            Debug.Assert(!string.IsNullOrEmpty(statLabel), $"{gameObject.name}: statLabel이 비어있습니다!");

            try
            {
                List<StatData> statDataList = await GameManager.Instance.LoadAllAddressablesAsync<StatData>(statLabel);
                
                InitializeStats(statDataList);
                _initialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[TeamStat] 초기화 중 오류 발생 ({statLabel}): {e.Message}");
                _initialized = false;
            }
        }

        private void InitializeStats(List<StatData> list)
        {
            foreach (var data in list.Where(data => !Stats.ContainsKey(data.statType)))
            {
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
