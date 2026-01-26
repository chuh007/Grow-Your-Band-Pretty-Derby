using Code.MainSystem.Rhythm.Data;

namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct ScoreUpdateEvent : IEvent
    {
        public int CurrentScore;
        public int CurrentCombo;
        public JudgementType LastJudgement;
        public int LaneIndex;

        public ScoreUpdateEvent(int score, int combo, JudgementType judgement, int laneIndex)
        {
            CurrentScore = score;
            CurrentCombo = combo;
            LastJudgement = judgement;
            LaneIndex = laneIndex;
        }
    }
}