using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.StatSystem.BaseStats
{
    [CreateAssetMenu(fileName = "Stat data", menuName = "SO/Stat/Stat data", order = 0)]
    public class StatData : ScriptableObject
    {
        public StatType statType;
        public string statName;
        public int currentValue;
        public int minValue;
        public int maxValue;
        public AssetReferenceSprite statIcon;
        
        public AssetReferenceT<StatRankTable> rankTable;
    }
}