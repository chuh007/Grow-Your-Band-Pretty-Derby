using System.Collections.Generic;
using Code.Core;
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
        [SerializeField] private string dataPath;
        
        public MemberType MemberType => memberType;
        
        protected override void Awake()
        {
            base.Awake();

            //GameManager.Instance.LoadAsset<List<StatData>>(dataPath);
            
            foreach (var data in memberStatData)
            {
                BaseStat stat = new BaseStat(data);
                Stats[data.statType] = stat;
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
            stat?.PlusValue(evt.AddValue);
        }
    }
}
