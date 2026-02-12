using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.MainScreen.MemberData;

namespace Code.MainSystem.StatSystem.Events
{
    public struct TeamPracticeEvent : IEvent
    {
        public List<UnitDataSO> UnitDataSos;

        public TeamPracticeEvent(List<UnitDataSO> unitDataSos)
        {
            UnitDataSos = unitDataSos;
        }
    }
}