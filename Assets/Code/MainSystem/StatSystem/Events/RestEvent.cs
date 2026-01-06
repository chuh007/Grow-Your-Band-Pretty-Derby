using Code.Core.Bus;
using Code.MainSystem.MainScreen.MemberData;

namespace Code.MainSystem.StatSystem.Events
{
    public struct RestEvent :IEvent
    {
        public UnitDataSO Unit { get; }

        public RestEvent(UnitDataSO unit)
        {
            Unit = unit;
        }
    }
}