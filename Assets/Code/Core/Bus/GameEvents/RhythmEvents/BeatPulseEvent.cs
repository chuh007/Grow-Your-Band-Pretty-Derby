namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct BeatPulseEvent : IEvent 
    { 
        public int BeatNumber;
        public BeatPulseEvent(int beatNumber) => BeatNumber = beatNumber;
    }
}