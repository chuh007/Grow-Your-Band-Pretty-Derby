using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class StatChangeResult
    {
        public string statName;
        public Sprite leftIcon;   // 스탯 아이콘
        public Sprite rightIcon;  // 등급 아이콘 (F, E 등)
        public float currentValue;
        public float deltaValue;

        public StatChangeResult(string statName, Sprite leftIcon, Sprite rightIcon, float currentValue, float deltaValue)
        {
            this.statName = statName;
            this.leftIcon = leftIcon;
            this.rightIcon = rightIcon;
            this.currentValue = currentValue;
            this.deltaValue = deltaValue;
        }
    }
}