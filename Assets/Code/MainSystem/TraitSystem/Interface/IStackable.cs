namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IStackable
    {
        int StackCount { get; }
        int IncreaseStack { get; }
        int MaxStack { get; }
    }
}