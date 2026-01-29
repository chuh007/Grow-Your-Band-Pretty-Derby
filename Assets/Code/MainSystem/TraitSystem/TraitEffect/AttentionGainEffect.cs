using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 주목도 상승 특성
    /// </summary>
    public class AttentionGainEffect : AbstractTraitEffect, IFeverTimeStat, IPercentageModifier<IFeverTimeStat>
    {
        public float Percentage { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
        }
    }
}