using System.Collections.Generic;
using Code.Core.Bus;

namespace Code.MainSystem.StatSystem.Events
{
    public struct TeamPracticeEvent : IEvent
    {
        public List<float> MemberConditions;

        public TeamPracticeEvent(List<float> memberConditions)
        {
            MemberConditions = memberConditions;
        }
    }
}