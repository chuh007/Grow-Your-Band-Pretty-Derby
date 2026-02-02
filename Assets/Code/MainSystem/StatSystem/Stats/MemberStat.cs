using System;
using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.StatSystem.Stats
{
    public class MemberStat : AbstractStats
    {
        [SerializeField] private MemberType memberType;
        [SerializeField] private string statDataLabel;

        private readonly List<StatData> _statDataList = new();
        
        public MemberType MemberType => memberType;

        protected override async void Awake()
        {
            Debug.Assert(!string.IsNullOrEmpty(statDataLabel), $"{gameObject.name}: statDataLabel이 비어있습니다!");
            
            try
            {
                base.Awake();

                List<StatData> statDataList = await GameManager.Instance.LoadAllAddressablesAsync<StatData>(statDataLabel);
                _statDataList.AddRange(statDataList);

                InitializeStats(_statDataList);
                Bus<TeamStatValueChangedEvent>.OnEvent += OnTeamStatChanged;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MemberStat] 초기화 실패: {e.Message}");
                InitializeStats(new List<StatData>());
            }
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