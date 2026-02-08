using System.Threading.Tasks;
using Code.Core.Addressable;
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
        public StatRankTable RankTable { get; private set; } 
        
        

        public StatRankType CurrentRankName =>
            RankTable != null ? RankTable.GetRankName(CurrentValue) : StatRankType.None;

        public Sprite CurrentRankIcon =>
            RankTable?.GetRankIcon(CurrentValue) is { } iconRef && iconRef.RuntimeKeyIsValid()
                ? GameResourceManager.Instance.Load<Sprite>(iconRef.RuntimeKey.ToString()) : null;

        public BaseStat(StatData data)
        {
            StatType = data.statType;
            CurrentValue = data.currentValue;
            StatName = data.statName;
            MinValue = data.minValue;
            MaxValue = data.maxValue;
        }

        public async Task InitializeAssetsAsync(StatData data)
        {
            var rm = GameResourceManager.Instance;
            if (data.statIcon.RuntimeKeyIsValid()) 
                StatIcon = await rm.LoadAsync<Sprite>(data.statIcon.RuntimeKey.ToString());
            
            if (data.rankTable.RuntimeKeyIsValid())
            {
                RankTable = await rm.LoadAsync<StatRankTable>(data.rankTable.RuntimeKey.ToString());
                if (RankTable != null) await RankTable.LoadAllRankIconsAsync();
            }
        }

        public void PlusValue(int value) 
            => SetValue(CurrentValue + value);
        
        public void MultiplyValue(int value)
            => SetValue(CurrentValue * value);
        
        public void SubtractValue(int value)
            => SetValue(CurrentValue - value);
        
        public void PlusPercentValue(int value) 
            => SetValue(CurrentValue + Mathf.RoundToInt(CurrentValue * (value / 100f)));
        
        public void MinusPercentValue(int value)
            => SetValue(CurrentValue - Mathf.RoundToInt(CurrentValue * (value / 100f)));

        private void SetValue(int newValue) 
            => CurrentValue = Mathf.Clamp(newValue, MinValue, MaxValue);
    }
}