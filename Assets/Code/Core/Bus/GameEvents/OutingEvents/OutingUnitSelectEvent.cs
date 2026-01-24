using Code.MainSystem.MainScreen.MemberData;

namespace Code.Core.Bus.GameEvents.OutingEvents
{
    public struct OutingUnitSelectEvent : IEvent
    {
        public UnitDataSO SelectedUnit;

        public OutingUnitSelectEvent(UnitDataSO selectedUnit)
        {
            SelectedUnit = selectedUnit;
        }
    }
}