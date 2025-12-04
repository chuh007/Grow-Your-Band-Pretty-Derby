using UnityEngine;
using Code.MainSystem.StatSystem.UI;
using System.Collections.Generic;

namespace Code.MainSystem.StatSystem.BaseStats
{
    public abstract class AbstractStats : MonoBehaviour
    {
        [SerializeField] protected List<CommonStatData> commonStatData;
        [SerializeField] protected StatsUI[] statUI;

        protected readonly Dictionary<CommonStatType, BaseStat> Stats = new();
        protected readonly Dictionary<CommonStatType, StatsUI> StatUIs = new();

        protected virtual void Awake()
        {
            foreach (var data in commonStatData)
            {
                BaseStat stat = new BaseStat(data);
                Stats[data.commonStatType] = stat;
            }
        }

        protected virtual void Start()
        {
            int idx = 0;
            foreach (var baseStat in Stats)
            {
                statUI[idx].EnableFor(baseStat.Value);
                StatUIs[baseStat.Value.CommonStatType] = statUI[idx];
                idx++;
            }
        }

        public void UpdateUI(CommonStatType baseStatType)
        {
            StatUIs[baseStatType].Update();
        }

        public BaseStat GetStat(CommonStatType baseStatType)
        {
            return Stats.GetValueOrDefault(baseStatType);  
        }
    }
}