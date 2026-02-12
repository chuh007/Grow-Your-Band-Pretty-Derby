using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Synergy
{
    public class TeamworkEffect : IScoreSynergy
    {
        private readonly float _ratio;
        public TeamworkEffect(float ratio) => _ratio = ratio;
        public float GetExtraScore(float b, float h, float c) => h * _ratio;
    }

    public class SupportEffect : IFeverSynergy
    {
        public float GetFeverDurationBonus(bool isPartChanged) => 2f;
    }

    public class StabilityEffect : IJudgmentSynergy
    {
        public string OverrideJudgment(string j, int c) => j;
        public bool IsPenaltyDisabled(string j) => InGameStatus.IsFever && j == "MISS";
    }

    public class EnergyEffect : IRecoverySynergy
    {
        private readonly int _step;
        public EnergyEffect(int step) => _step = step;

        public void OnComboTick(int combo, float stats)
        {
            if (combo > 0 && combo % _step == 0)
            {
                /* 인기도 += stats / 10f; */
            }
        }

        public void OnPerfectHit(float c)
        {
        }
    }

    public class GeniusEffect : IScoreSynergy, IJudgmentSynergy
    {
        public float GetExtraScore(float b, float h, float c) => b * 0.2f;
        public string OverrideJudgment(string j, int combo) => j;
        public bool IsPenaltyDisabled(string j) => j == "MISS";
    }

    public class SoloEffect : IScoreSynergy, IRecoverySynergy
    {
        public float GetExtraScore(float b, float h, float c) => b * 0.2f;

        public void OnComboTick(int combo, float s)
        {
        }

        public void OnPerfectHit(float c)
        {
            /* 인기도 += c / 100f; */
        }
    }

    public class MasteryEffect : IJudgmentSynergy
    {
        public string OverrideJudgment(string j, int combo)
        {
            if (j is not ("MISS" or "GOOD") || InGameStatus.PreparationStack <= 0)
                return j;
            
            InGameStatus.PreparationStack--;
            return "PERFECT";

        }

        public bool IsPenaltyDisabled(string j) => false;
    }

    public class ImmersionEffect : IJudgmentSynergy
    {
        public string OverrideJudgment(string j, int c) => j;
        public bool IsPenaltyDisabled(string j) => false;
    }

    public class GuitarSoloEffect : IScoreSynergy, IJudgmentSynergy
    {
        public float GetExtraScore(float b, float h, float c) =>
            InGameStatus.CurrentPart == MemberType.Guitar ? b * 0.2f : 0;

        public string OverrideJudgment(string j, int combo) =>
            InGameStatus.CurrentPart == MemberType.Guitar && j == "GOOD" ? "PERFECT" : j;

        public bool IsPenaltyDisabled(string j) => false;
    }
}