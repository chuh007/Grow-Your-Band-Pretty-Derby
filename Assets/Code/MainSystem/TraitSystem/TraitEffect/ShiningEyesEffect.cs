using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 반짝이는 눈
    /// </summary>
    public class ShiningEyesEffect : AbstractTraitEffect, IPercentageModifier, IAdditiveModifier
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
            Percentage = 0f;
            AdditiveValue = 0;
        }
    }
}