using Code.MainSystem.Rhythm;

namespace Code.Core.Bus.GameEvents
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