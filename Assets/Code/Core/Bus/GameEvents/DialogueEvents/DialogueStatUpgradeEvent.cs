using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.Core.Bus.GameEvents
{
    public struct DialogueStatUpgradeEvent : IEvent
    {
        public StatVariation Stat;

        public DialogueStatUpgradeEvent(StatVariation stat)
        {
            Stat = stat;
        }
    }
}