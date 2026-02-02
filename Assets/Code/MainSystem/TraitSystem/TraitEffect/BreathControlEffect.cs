using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 호흡 조절 특성
    /// </summary>
    public class BreathControlEffect : AbstractTraitEffect, 
        IConditionStat, IFeverInputStat,
        IPercentageModifier<IConditionStat>, IAdditiveModifier<IFeverInputStat>
    {
        public float Percentage { get; private set; }
        public float AdditiveValue { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
            AdditiveValue = (int)N2(trait);
        }
    }
}