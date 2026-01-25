using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class ShiningEyesEffect : AbstractTraitEffect, IShiningEyesModifier
    {
        public float ExtraActionChancePercent => _extraActionChancePercent;
        public int ExtraActionAmount => _extraActionAmount;

        private float _extraActionChancePercent;
        private int _extraActionAmount;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _extraActionChancePercent = N1(trait);
            _extraActionAmount = (int)N2(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _extraActionChancePercent = 0f;
            _extraActionAmount = 0;
        }
    }
}