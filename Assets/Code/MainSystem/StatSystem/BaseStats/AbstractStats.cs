using UnityEngine;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.BaseStats
{
    public abstract class AbstractStats : MonoBehaviour
    {
        protected readonly Dictionary<StatType, BaseStat> Stats = new();

        protected virtual void Awake() { }

        public void StatUpgrade(StatType statType, float successRate,float value)
        {
            BaseStat stat = Stats.GetValueOrDefault(statType);
            
            float randValue = Random.Range(0f, 100f);
            bool success = randValue < successRate;

            if (!success)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
                return;
            }

            stat.PlusValue((int)value);
            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
        }

        public BaseStat GetStat(StatType statType)
        {
            return Stats.GetValueOrDefault(statType);  
        }
    }
}