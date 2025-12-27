using Code.Core.Bus;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.StatSystem.Events
{
    public struct RestEvent : IEvent
    {
        public MemberType MemberType { get; }

        public RestEvent(MemberType memberType)
        {
            MemberType = memberType;
        }
    }
}