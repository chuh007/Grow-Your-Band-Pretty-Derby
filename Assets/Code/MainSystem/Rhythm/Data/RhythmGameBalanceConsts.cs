namespace Code.MainSystem.Rhythm.Data
{
    public static class RhythmGameBalanceConsts
    {
        public const float SCORE_TO_STAT_RATIO = 0.1f;
        public const float MISS_THRESHOLD_SECONDS = 0.2f;
        public const int BASE_STAT_GAIN = 1;
        public const int MIN_LOADING_TIME_MS = 1000;
        public const int TRANSITION_TIMEOUT_MS = 5000;

        // Visual Pulse
        public const float PULSE_SCALE_STRONG = 1.3f;
        public const float PULSE_SCALE_WEAK = 1.05f;
        public const int BEAT_INTERVAL_STRONG = 4;

        // Hit Feedback
        public const float IMPULSE_FORCE_PERFECT = 0.5f;
        public const float COMBO_PUNCH_SCALE = 1.5f;
        public const float COMBO_PUNCH_DURATION = 0.2f;
    }
}