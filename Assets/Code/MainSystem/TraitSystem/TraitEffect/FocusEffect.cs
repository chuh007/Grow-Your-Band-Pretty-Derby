using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 집중력 특성
    /// </summary>
    public class FocusEffect : AbstractTraitEffect, IPercentageModifier<ISuccessRateStat>, IJudgmentCorrection
    {
        public float Percentage { get; private set; }
        public bool CorrectMissToGood => true;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
        }
    }
}