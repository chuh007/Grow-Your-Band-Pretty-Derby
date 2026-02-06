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
    
    // TODO 연결 작업후 삭제
    internal interface IFeverInputStat
    { }

    internal interface IFeverTimeStat
    { }

    public interface IFeverScoreStat
    { }
    
    internal interface IMentalStat
    { }
}