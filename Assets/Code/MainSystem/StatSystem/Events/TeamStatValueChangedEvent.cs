using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.StatSystem.Events
{
    public struct TeamStatValueChangedEvent : IEvent
    {
        public StatType StatType;
        public int AddValue;

        public TeamStatValueChangedEvent(StatType statType, int addValue)
        {
            StatType = statType;
            AddValue = addValue;
        }
    }
}