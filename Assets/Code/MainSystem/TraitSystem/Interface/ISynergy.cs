namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IScoreSynergy
    {
        float GetExtraScore(float baseScore, float harmony, float concentration);
    }

    public interface IFeverSynergy
    {
        float GetFeverDurationBonus(bool isPartChanged);
    }

    public interface IJudgmentSynergy
    {
        string OverrideJudgment(string currentJudge, int combo);
        bool IsPenaltyDisabled(string judge);
    }

    public interface IRecoverySynergy
    {
        void OnComboTick(int combo, float totalStats);
        void OnPerfectHit(float concentration);
    }
}