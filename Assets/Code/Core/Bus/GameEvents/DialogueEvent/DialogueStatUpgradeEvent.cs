using Code.MainSystem.StatSystem.BaseStats;

namespace Code.Core.Bus.GameEvents
{
    public struct DialogueStatUpgradeEvent : IEvent
    {
        public StatType StatType;
        public int StatValue;

        public DialogueStatUpgradeEvent(StatType statType, int statValue)
        {
            StatType = statType;
            StatValue = statValue;
        }
    }
}