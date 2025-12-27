using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.StatSystem.Events
{
    public struct StatIncreaseEvent : IEvent
    {
        public MemberType memberType;
        public StatType statType;
        public float Value;


        public StatIncreaseEvent(MemberType memberType, StatType statType, float value)
        {
            this.memberType = memberType;
            this.statType = statType;
            Value = value;
        }
    }
}