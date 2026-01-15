using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct TraitUpgraded : IEvent
    {
        public MemberType MemberType;
        public ActiveTrait Trait;
        public int PrevLevel;

        public TraitUpgraded(MemberType memberType, ActiveTrait trait, int prevLevel)
        {
            MemberType = memberType;
            Trait = trait;
            PrevLevel = prevLevel;
        }
    }
}