using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.Core
{
    public static class RhythmGameStatCalculator
    {
        public static int CalculateAllStatGain(float finalScore)
        {
            return RhythmGameBalanceConsts.BASE_STAT_GAIN + (int)(finalScore * RhythmGameBalanceConsts.SCORE_TO_STAT_RATIO);
        }

        public static int CalculateHarmonyStatGain(float finalScore, int memberCount)
        {
            return memberCount * (int)(finalScore * RhythmGameBalanceConsts.SCORE_TO_STAT_RATIO);
        }
    }
}