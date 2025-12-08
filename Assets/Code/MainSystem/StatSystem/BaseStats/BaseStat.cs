using UnityEngine;

namespace Code.MainSystem.StatSystem.BaseStats
{
    public class BaseStat : IModifierStat
    {
        public StatType StatType { get; private set; }
        public string StatName { get; private set; }
        public int CurrentValue { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public Sprite StatIcon { get; private set; }

        public BaseStat(StatData data)
        {
            StatType = data.statType;
            CurrentValue = data.currentValue;
            StatName = data.statName;
            StatIcon = data.statIcon;
            MinValue = data.minValue;
            MaxValue = data.maxValue;
        }

        public void PlusValue(int value) 
            => CurrentValue = Mathf.Clamp(CurrentValue + value, MinValue, MaxValue);

        public void MultiplyValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue * value, MinValue, MaxValue);

        public void SubtractValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue - value, MinValue, MaxValue);

        public void PlusPercentValue(int value)
        {
            int addValue = Mathf.RoundToInt(CurrentValue * (value / 100f));
            CurrentValue = Mathf.Clamp(CurrentValue + addValue, MinValue, MaxValue);
        }

        public void MinusPercentValue(int value)
        {
            int subtractValue = Mathf.RoundToInt(CurrentValue * (value / 100f));
            CurrentValue = Mathf.Clamp(CurrentValue - subtractValue, MinValue, MaxValue);
        }
    }
}