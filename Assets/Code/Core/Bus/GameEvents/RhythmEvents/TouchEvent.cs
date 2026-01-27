namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct TouchEvent : IEvent
    {
        public int LaneIndex { get; set; }

        public TouchEvent(int laneIndex)
        {
            LaneIndex = laneIndex;
        }
    }
}