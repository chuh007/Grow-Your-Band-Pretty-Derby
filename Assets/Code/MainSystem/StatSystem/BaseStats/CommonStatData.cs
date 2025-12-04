using UnityEngine;

namespace Code.MainSystem.StatSystem.BaseStats
{
    [CreateAssetMenu(fileName = "BaseStatData", menuName = "SO/CommonData", order = 0)]
    public class CommonStatData : ScriptableObject
    {
        public CommonStatType commonStatType;
        public string statName;
        public int currentValue;
        public int minValue;
        public int maxValue;
        public Sprite statIcon;
    }
}