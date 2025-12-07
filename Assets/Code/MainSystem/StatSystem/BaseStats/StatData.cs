using UnityEngine;

namespace Code.MainSystem.StatSystem.BaseStats
{
    [CreateAssetMenu(fileName = "Stat data", menuName = "SO/Stat data", order = 0)]
    public class StatData : ScriptableObject
    {
        public StatType statType;
        public string statName;
        public int currentValue;
        public int minValue;
        public int maxValue;
        public Sprite statIcon;
    }
}