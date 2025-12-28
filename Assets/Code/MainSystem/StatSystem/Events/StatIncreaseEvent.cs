using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.StatSystem.Events
{
    /// <summary>
    /// 특정 멤버의 지정된 스탯을 증가시키는 이벤트
    /// </summary>
    public struct StatIncreaseEvent : IEvent
    {
        public MemberType MemberType;
        public StatType StatType;
        public float Value;
        
        public StatIncreaseEvent(MemberType memberType, StatType statType, float value)
        {
            MemberType = memberType;
            StatType = statType;
            Value = value;
        }
    }
}