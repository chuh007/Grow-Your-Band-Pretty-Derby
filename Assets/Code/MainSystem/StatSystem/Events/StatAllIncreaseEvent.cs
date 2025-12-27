using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.StatSystem.Events
{
    /// <summary>
    /// 모든 멤버의 지정된 스탯을 증가시키는 이벤트
    /// </summary>
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