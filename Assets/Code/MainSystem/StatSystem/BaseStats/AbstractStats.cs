using UnityEngine;
using System.Collections.Generic;

namespace Code.MainSystem.StatSystem.BaseStats
{
    public abstract class AbstractStats : MonoBehaviour
    {
        protected readonly Dictionary<StatType, BaseStat> Stats = new();

        protected virtual void Awake() { }

        public void ApplyStatIncrease(StatType statType, float value)
        {
            BaseStat stat = Stats.GetValueOrDefault(statType);

            stat?.PlusValue((int)value);
        }

        public void ApplyRecover(StatType statType, int recoverValue)
        {
            BaseStat stat = Stats.GetValueOrDefault(statType);

            stat?.PlusValue(recoverValue);
        }

        public BaseStat GetStat(StatType statType)
        {
            return Stats.GetValueOrDefault(statType);
        }
    }
}