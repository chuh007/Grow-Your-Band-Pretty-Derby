namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IMultiplyModifier<T>
    {
        float Multiplier { get; }
    }
    
    public interface IPercentageModifier<T>
    {
        float Percentage { get; }
    }
    
    public interface IAdditiveModifier<T>
    {
        float AdditiveValue { get; }
    }
}