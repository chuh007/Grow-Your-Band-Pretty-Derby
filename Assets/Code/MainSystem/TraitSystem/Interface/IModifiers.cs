namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IMultiplyModifier
    {
        float Multiplier { get; }
    }
    
    public interface IPercentageModifier
    {
        float Percentage { get; }
    }
    
    public interface IAdditiveModifier
    {
        float AdditiveValue { get; }
    }
}