using Code.Core.Bus;

namespace Code.MainSystem.StatSystem.Events
{
    public struct TeamStatIncreaseEvent : IEvent
    {
        public int AddValue;

        public TeamStatIncreaseEvent(int addValue)
        {
            AddValue = addValue;
        }
    }
}