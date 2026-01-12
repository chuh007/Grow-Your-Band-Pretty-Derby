using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.StatSystem.Manager;

namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct TraitAddRequested : IEvent
    {
        public readonly MemberType MemberType;
        public readonly TraitDataSO NewTrait;

        public TraitAddRequested(MemberType memberType, TraitDataSO newTrait)
        {
            MemberType = memberType;
            NewTrait = newTrait;
        }
    }
}