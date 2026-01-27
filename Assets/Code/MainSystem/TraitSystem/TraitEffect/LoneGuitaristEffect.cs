using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 고독한 기타리스트 특성
    /// </summary>
    public class LoneGuitaristEffect : AbstractTraitEffect, IPercentageModifier<IPracticeStat>, IPercentageModifier<IConditionStat>
    {
        public float Percentage { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
        }
    }
}