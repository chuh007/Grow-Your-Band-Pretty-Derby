using Code.Core.Bus;

namespace Code.MainSystem.Rhythm
{
    public struct SongStartEvent : IEvent { }
    public struct SongEndEvent : IEvent { }
    
    public struct BeatPulseEvent : IEvent 
    { 
        public int BeatNumber;
        public BeatPulseEvent(int beatNumber) => BeatNumber = beatNumber;
    }
}