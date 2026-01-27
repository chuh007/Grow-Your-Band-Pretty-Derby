using Code.MainSystem.TraitSystem.Data;

namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct ActionApplyEvent : IEvent
    {
        public TraitEffectType TraitEffectType { get; }
        
        public ActionApplyEvent(TraitEffectType traitEffectType)
        {
            TraitEffectType = traitEffectType;
        }
    }
}