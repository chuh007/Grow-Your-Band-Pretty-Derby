using Code.MainSystem.Rhythm;

namespace Code.Core.Bus.GameEvents
{
    public struct NoteHitEvent : IEvent
    {
        public JudgementType Judgement;
        public int LaneIndex;
        public int TrackIndex;

        public NoteHitEvent(JudgementType type, int lane, int trackIndex = 0) 
        { 
            Judgement = type; 
            LaneIndex = lane; 
            TrackIndex = trackIndex;
        }
    }
}