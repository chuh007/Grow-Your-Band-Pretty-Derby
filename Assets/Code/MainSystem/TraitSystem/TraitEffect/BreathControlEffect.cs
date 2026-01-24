using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class BreathControlEffect : AbstractTraitEffect, IConditionChangeModifier, IFeverInputModifier
    {
        public float ConditionChangeMultiplier => _conditionMultiplier;
        public int FeverInputReduce => _feverInputReduce;

        private float _conditionMultiplier = 1f;
        private int _feverInputReduce;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _conditionMultiplier = N1(trait);
            _feverInputReduce = (int)N2(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _conditionMultiplier = 1f;
            _feverInputReduce = 0;
        }
    }
}