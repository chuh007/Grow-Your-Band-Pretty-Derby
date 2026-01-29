using Code.MainSystem.StatSystem.Manager;

namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct ActionPointGainEvent : IEvent
    {
        public MemberType MemberType;
        public int Amount;

        public ActionPointGainEvent(MemberType memberType, int amount)
        {
            MemberType = memberType;
            Amount = amount;
        }
    }
}