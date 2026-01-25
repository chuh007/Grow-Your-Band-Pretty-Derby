namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IExcessivePassionModifier
    {
        float ExtraActionChancePercent { get; }
        int ExtraActionAmount { get; }
        float ConditionCostIncreasePercent { get; }
    }
}