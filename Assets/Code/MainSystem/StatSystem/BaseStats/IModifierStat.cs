namespace Code.MainSystem.StatSystem.BaseStats
{
    public interface IModifierStat
    {
        void PlusValue(int value);
        void MultiplyValue(int value);
        void SubtractValue(int value);
        void PlusPercentValue(int value);
        void MinusPercentValue(int value);
    }
}