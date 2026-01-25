using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class HeartToHeartEffect : AbstractTraitEffect, IHeartToHeartModifier
    {
        public float BaseEnsembleBonusPercent => _baseEnsembleBonusPercent;
        public float BothHaveTraitMultiplier => _bothHaveTraitMultiplier;

        private float _baseEnsembleBonusPercent;
        private float _bothHaveTraitMultiplier;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _baseEnsembleBonusPercent = N1(trait);
            _bothHaveTraitMultiplier = N2(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _baseEnsembleBonusPercent = 0f;
            _bothHaveTraitMultiplier = 0f;
        }
    }
}