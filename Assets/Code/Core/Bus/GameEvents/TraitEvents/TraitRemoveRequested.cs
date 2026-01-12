using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.StatSystem.Manager;

namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct TraitRemoveRequested : IEvent
    {
        public readonly MemberType MemberType;
        public readonly TraitType TraitType;

        public TraitRemoveRequested(MemberType memberType, TraitType traitType)
        {
            MemberType = memberType;
            TraitType = traitType;
        }
    }
}