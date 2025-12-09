using UnityEngine;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.BaseStats
{
    public abstract class AbstractStats : MonoBehaviour
    {
        [SerializeField] protected List<StatData> commonStatData;

        protected readonly Dictionary<StatType, BaseStat> CommonStats = new();

        protected virtual void Awake()
        {
            foreach (var data in commonStatData)
            {
                BaseStat stat = new BaseStat(data);
                CommonStats[data.statType] = stat;
            }
        }

        public void CommonStatUpgrade(StatType statType, float successRate,float value)
        {
            float randValue = Random.Range(0f, 101f);
            if (randValue >= successRate)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
            }
            else
            {
                BaseStat stat = CommonStats.GetValueOrDefault(statType);
                if (stat != null)
                {
                    stat.PlusValue((int)value);
                    Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
                }
            }
        }

        public BaseStat GetCommonStat(StatType statType)
        {
            return CommonStats.GetValueOrDefault(statType);  
        }
    }
}