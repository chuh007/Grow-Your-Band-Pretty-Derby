using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.StatSystem.Events
{
    public struct StatAllIncreaseEvent : IEvent
    {
        public StatType StatType;
        public float Value;
        
        public StatAllIncreaseEvent(StatType statType, float value)
        {
            StatType = statType;
            Value = value;
        }
    }
}