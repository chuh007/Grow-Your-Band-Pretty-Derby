using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 호흡 조절 특성
    /// </summary>
    public class BreathControlEffect : AbstractTraitEffect, IPercentageModifier, IAdditiveModifier
    {
        public float Percentage { get; private set; }
        public float AdditiveValue { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
            AdditiveValue = (int)N2(trait);
        }

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            holder?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            holder?.UnregisterModifier(this);
            Percentage = 1f;
            AdditiveValue = 0;
        }
    }
}