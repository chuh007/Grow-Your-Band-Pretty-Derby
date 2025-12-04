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

        public BaseStat(CommonStatData data)
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

        public void MinusValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue - value, MinValue, MaxValue);

        public void SubtractValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue - value, MinValue, MaxValue);

        public void PlusPerScentValue(int value)
            => CurrentValue = (int)(CurrentValue + CurrentValue * (value / 100.0));

        public void MinusPerScentValue(int value)
            => CurrentValue = (int)(CurrentValue - CurrentValue * (value / 100.0));
    }
}