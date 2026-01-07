using Code.Core.Bus;

namespace Code.MainSystem.StatSystem.Events
{
    public struct TeamPracticeResultEvent : IEvent
    {
        public bool IsSuccess;

        public TeamPracticeResultEvent(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}