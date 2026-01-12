using UnityEngine;

namespace Code.Core.Bus.GameEvents
{
    public struct StatIncreaseDecreaseEvent : IEvent
    {
        public bool increase; // true면 증가 false는 감소
        public string amount;
        public Sprite statIcon;
        public string statName;

        public StatIncreaseDecreaseEvent(bool increase, string amount, Sprite statIcon, string statName)
        {
            this.increase = increase;
            this.amount = amount;
            this.statIcon = statIcon;
            this.statName = statName;
        }
    }
}