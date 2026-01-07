using Code.MainSystem.Rhythm;

namespace Code.Core.Bus.GameEvents
{
    public struct NoteHitEvent : IEvent
    {
        public JudgementType Judgement;
        public int LaneIndex;
        public NoteHitEvent(JudgementType type, int lane) { Judgement = type; LaneIndex = lane; }
    }
}