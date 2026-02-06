namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct BeatPulseEvent : IEvent 
    { 
        public int BeatIndex;
        public BeatPulseEvent(int beatIndex) => BeatIndex = beatIndex;
    }
}