using Code.Core.Bus;
using Code.MainSystem.MainScreen.MemberData;

namespace Code.MainSystem.StatSystem.Events
{
    public struct ConfirmRestEvent : IEvent
    {
        public UnitDataSO Unit { get; }

        public ConfirmRestEvent(UnitDataSO unit)
        {
            Unit = unit;
        }
    }
}