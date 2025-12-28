using Code.Core.Bus;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.StatSystem.Events
{
    public struct StatAllMemberStatIncreaseEvent : IEvent
    {
        public MemberType MemberType;
        public float Value;

        public StatAllMemberStatIncreaseEvent(MemberType memberType, float value)
        {
            MemberType = memberType;
            Value = value;
        }
    }
}