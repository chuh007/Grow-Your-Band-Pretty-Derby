using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ISuccessGuarantor
    {
        bool ShouldGuarantee();
    }

    public interface IInspirationSystem
    {
        void OnTrainingFailed();
    }

    public interface ITurnProcessListener
    {
        void OnTurnPassed();
    }
    
    public interface IJudgmentCorrection
    {
        bool CorrectMissToGood { get; }
    }

    public interface IDisciplinedLifestyle
    {
        public float BonusValue { get; }
        public void UpdateLastStat(StatType lastType);
    }

    public interface IMultiStatModifier
    {
        public int AddValue { get; }
    }

    public interface IGrooveRestoration
    {
        bool IsBuffered { get; set; }
        float Multiplier { get; }
        void Reset();
    }
    
    public interface IConditionModifier {
        float ConditionCostMultiplier { get; }
        float ConditionRecoveryMultiplier { get; }
    }
    
    public interface IAdditionalActionProvider {
        float AdditionalActionChance { get; }
    }
    
    public interface IConsecutiveActionModifier
    {
        float GetSuccessBonus(string currentActionId);
    }
    
    public interface ITrainingSuccessBonus {
        float AddValue { get; }
        void OnTrainingSuccess(MemberType member);
    }
    
    public interface IRoutineModifier {
        void OnPracticeSuccess();
        void OnRest();
        float GetSuccessBonus(string currentActionId);
        float GetStatMultiplier();
    }
}