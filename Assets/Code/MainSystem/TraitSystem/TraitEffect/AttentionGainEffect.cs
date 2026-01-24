using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class AttentionGainEffect : AbstractTraitEffect, IFeverDurationModifier
    {
        public float FeverDurationBonus => _bonus;

        private float _bonus;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _bonus = N1(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _bonus = 0f;
        }
    }
}