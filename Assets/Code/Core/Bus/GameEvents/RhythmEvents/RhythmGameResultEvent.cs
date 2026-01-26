using Code.MainSystem.Rhythm.Data;

namespace Code.Core.Bus.GameEvents.RhythmEvents
{
    public struct RhythmGameResultEvent : IEvent
    {
        public readonly int FinalScore;
        public readonly int MaxCombo;
        public readonly string Rank; 
        
        public readonly int PerfectCount;
        public readonly int GreatCount;
        public readonly int GoodCount;
        public readonly int MissCount;

        public RhythmGameResultEvent(int score, int maxCombo, string rank, int perfect, int great, int good, int miss)
        {
            FinalScore = score;
            MaxCombo = maxCombo;
            Rank = rank;
            PerfectCount = perfect;
            GreatCount = great;
            GoodCount = good;
            MissCount = miss;
        }
    }
}