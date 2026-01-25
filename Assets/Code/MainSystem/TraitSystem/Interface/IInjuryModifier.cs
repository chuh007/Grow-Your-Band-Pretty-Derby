namespace Code.MainSystem.TraitSystem.Interface
{
    public interface IInjuryModifier
    {
        public float TrainingSuccessPenaltyPercent { get; }
        public int InjuryDurationTurns { get; }
    }
}