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
    
    internal interface IJudgmentCorrection
    {
        bool CorrectMissToGood { get; }
    }
}