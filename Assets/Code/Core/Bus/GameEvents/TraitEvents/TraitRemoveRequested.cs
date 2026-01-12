using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct TraitRemoveRequested : IEvent
    {
        public readonly MemberType MemberType;
        public readonly ActiveTrait TargetTrait;

        public TraitRemoveRequested(MemberType memberType, ActiveTrait targetTrait)
        {
            MemberType = memberType;
            TargetTrait = targetTrait;
        }
    }
}