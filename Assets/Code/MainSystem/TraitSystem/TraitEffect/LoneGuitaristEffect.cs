using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class LoneGuitaristEffect : AbstractTraitEffect, ILoneGuitaristModifier
    {
        public float SoloPracticeEffectBonusPercent => _soloPracticeEffectBonusPercent;
        public float SoloPracticeConditionCostReductionPercent => _soloPracticeConditionCostReductionPercent;

        private float _soloPracticeEffectBonusPercent;
        private float _soloPracticeConditionCostReductionPercent;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _soloPracticeEffectBonusPercent = N1(trait);
            _soloPracticeConditionCostReductionPercent = N2(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _soloPracticeEffectBonusPercent = 0f;
            _soloPracticeConditionCostReductionPercent = 0f;
        }
    }
}