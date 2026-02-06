using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class FocusEffect : MultiStatModifierEffect , IJudgmentCorrection
    {
        public bool CorrectMissToGood { get; private set; } = true;
    }
}