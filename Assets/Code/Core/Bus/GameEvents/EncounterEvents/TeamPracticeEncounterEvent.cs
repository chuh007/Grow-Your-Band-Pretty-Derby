using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;

namespace Code.Core.Bus.GameEvents.EncounterEvents
{
    public struct TeamPracticeEncounterEvent : IEvent
    {
        public List<UnitDataSO> AllUnits;

        public TeamPracticeEncounterEvent(List<UnitDataSO> allUnits)
        {
            AllUnits = allUnits;
        }
    }
}