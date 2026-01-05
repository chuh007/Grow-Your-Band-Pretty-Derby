using UnityEngine;
using Code.Core.Bus;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.StatSystem.MemberStats
{
    public class MemberStat : AbstractStats
    {
        [SerializeField] private MemberType memberType;
        [SerializeField] private string statDataLabel;

        private readonly List<StatData> _statDataList = new();
        
        public MemberType MemberType => memberType;

        protected override async void Awake()
        {
            base.Awake();

            var handle = Addressables.LoadAssetsAsync<StatData>(
                statDataLabel,
                data => _statDataList.Add(data)
            );

            await handle.Task;

            InitializeStats(_statDataList);

            Bus<TeamStatValueChangedEvent>.OnEvent += OnTeamStatChanged;
        }

        private void InitializeStats(List<StatData> list)
        {
            foreach (var data in list)
            {
                BaseStat stat = new BaseStat(data);
                Stats[data.statType] = stat;
            }
        }

        private void OnDestroy()
        {
            Bus<TeamStatValueChangedEvent>.OnEvent -= OnTeamStatChanged;
        }

        private void OnTeamStatChanged(TeamStatValueChangedEvent evt)
        {
            BaseStat stat = GetStat(evt.StatType);
            stat?.PlusValue(evt.AddValue);
        }

        public void ApplyAllStatIncrease(float value)
        {
            foreach (var stat in Stats.Values)
                stat.PlusValue((int)value);
        }
    }
}