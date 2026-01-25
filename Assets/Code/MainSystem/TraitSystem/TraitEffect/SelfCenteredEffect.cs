using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class SelfCenteredEffect : AbstractTraitEffect, ISelfCenteredModifier
    {
        public float EnsembleEffectPenaltyPercent => _ensembleEffectPenaltyPercent;

        private float _ensembleEffectPenaltyPercent;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _ensembleEffectPenaltyPercent = 50f;
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _ensembleEffectPenaltyPercent = 0f;
        }
    }
}