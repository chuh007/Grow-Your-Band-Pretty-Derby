using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.TraitSystem.Data;

namespace Code.Core.Bus.GameEvents.DialogueEvents
{
    public struct DialogueGetSkillEvent : IEvent
    {
        public TraitVariation TraitType;
        
        public DialogueGetSkillEvent(TraitVariation traitType)
        {
            TraitType = traitType;
        }
    }
}