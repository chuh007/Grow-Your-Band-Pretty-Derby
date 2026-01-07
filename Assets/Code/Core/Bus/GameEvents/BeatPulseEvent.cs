namespace Code.Core.Bus.GameEvents
{
    public struct BeatPulseEvent : IEvent 
    { 
        public int BeatNumber;
        public BeatPulseEvent(int beatNumber) => BeatNumber = beatNumber;
    }
}