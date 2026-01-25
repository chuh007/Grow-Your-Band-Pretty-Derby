using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class ExcessivePassionEffect : AbstractTraitEffect, IExcessivePassionModifier
    {
        public float ExtraActionChancePercent => _extraActionChancePercent;
        public int ExtraActionAmount => _extraActionAmount;
        public float ConditionCostIncreasePercent => _conditionCostIncreasePercent;

        private float _extraActionChancePercent;
        private int _extraActionAmount;
        private float _conditionCostIncreasePercent;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _extraActionChancePercent = N1(trait);
            _extraActionAmount = (int)N2(trait);
            _conditionCostIncreasePercent = N3(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _extraActionChancePercent = 0f;
            _extraActionAmount = 0;
            _conditionCostIncreasePercent = 0f;
        }
    }
}