using Code.MainSystem.StatSystem.BaseStats;

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
         float BonusValue { get; }
        float CheckPractice(StatType statType);
        void UpdateLastStat(StatType lastType);
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
    
    public interface IOverzealous {
        float ConditionCostMultiplier { get; }
        float ConditionRecoveryMultiplier { get; }

        bool CheckAction();
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
    }
    
    public interface IRoutineModifier {
        void OnPracticeSuccess();
        void OnRest();
        float GetStatMultiplier();
    }
}