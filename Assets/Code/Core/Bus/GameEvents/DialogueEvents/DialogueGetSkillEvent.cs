using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.TraitSystem.Data;

namespace Code.Core.Bus.GameEvents.DialogueEvents
{
    public struct DialogueGetSkillEvent : IEvent
    {
        public TraitType TraitType;
        
        public DialogueGetSkillEvent(TraitType traitType)
        {
            TraitType = traitType;
        }
    }
}