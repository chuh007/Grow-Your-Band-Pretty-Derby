using UnityEngine;

namespace Code.MainSystem.StatSystem.BaseStats
{
    public class BaseStat
    {
        public StatType StatType { get; private set; }
        public string StatName { get; private set; }
        public int CurrentValue { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public Sprite StatIcon { get; private set; }
        
        public string CurrentRankName => RankTable != null ? RankTable.GetRankName(CurrentValue) : "N/A";
        public Sprite CurrentRankIcon => RankTable != null ? RankTable.GetRankIcon(CurrentValue) : null;
        
        private StatRankTable RankTable { get; set; }

        public BaseStat(StatData data)
        {
            StatType = data.statType;
            CurrentValue = data.currentValue;
            StatName = data.statName;
            StatIcon = data.statIcon;
            MinValue = data.minValue;
            MaxValue = data.maxValue;
            RankTable = data.rankTable;
        }

        public void PlusValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue + value, MinValue, MaxValue);

        public void MultiplyValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue * value, MinValue, MaxValue);

        public void SubtractValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue - value, MinValue, MaxValue);

        public void PlusPercentValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue + Mathf.RoundToInt(CurrentValue * (value / 100f)), MinValue,
                MaxValue);
        
        public void MinusPercentValue(int value)
            => CurrentValue = Mathf.Clamp(CurrentValue - Mathf.RoundToInt(CurrentValue * (value / 100f)), MinValue,
                MaxValue);
    }
}