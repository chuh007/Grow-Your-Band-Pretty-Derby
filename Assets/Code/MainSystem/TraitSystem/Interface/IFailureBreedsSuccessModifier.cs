namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IFailureBreedsSuccessModifier
    {
        int InspirationGainOnFail { get; }
        int GuaranteedSuccessThreshold { get; }
    }
}