using Code.Core.Bus;

namespace Code.MainSystem.StatSystem.Events
{
    /// <summary>
    /// 팀 스탯을 증가시키는 이벤트
    /// </summary>
    public struct TeamStatIncreaseEvent : IEvent
    {
        public int AddValue;

        public TeamStatIncreaseEvent(int addValue)
        {
            AddValue = addValue;
        }
    }
}