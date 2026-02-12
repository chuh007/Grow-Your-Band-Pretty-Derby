using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.TraitSystem.Synergy
{
    public static class InGameStatus
    {
        public static MemberType CurrentPart { get; private set; } = MemberType.Team;
        public static bool IsFever { get; set; }
        public static int PreparationStack { get; set; }
        public static float NarrowJudgmentTimer { get; set; }

        public static void Reset()
        {
            CurrentPart = MemberType.Team;
            IsFever = false;
            PreparationStack = 0;
            NarrowJudgmentTimer = 0f;
        }
    }
}