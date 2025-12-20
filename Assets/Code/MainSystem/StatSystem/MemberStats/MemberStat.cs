using System.Collections.Generic;
using Code.Core;
using UnityEngine;
using Code.Core.Bus;
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
        [SerializeField] private string dataPath;
        
        public MemberType MemberType => memberType;
        
        protected override async void Awake()
        {
            base.Awake();

            var handle = Addressables.LoadAssetsAsync<StatData>(
                statDataLabel,
                OnStatDataLoaded
            );

            await handle.Task;

            Bus<TeamStatValueChangedEvent>.OnEvent += OnTeamStatChanged;
        }

        private void OnDestroy()
        {
            Bus<TeamStatValueChangedEvent>.OnEvent -= OnTeamStatChanged;
        }

        private void OnStatDataLoaded(StatData data)
        {
            if (Stats.ContainsKey(data.statType))
                return;

            BaseStat stat = new BaseStat(data);
            Stats[data.statType] = stat;
        }

        private void OnTeamStatChanged(TeamStatValueChangedEvent evt)
        {
            BaseStat stat = GetStat(evt.StatType);
            stat?.PlusValue(evt.AddValue);
        }
    }
}
