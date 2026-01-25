using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class HonedTechniqueEffect : AbstractTraitEffect , IHonedTechniqueModifier
    {
        public float HealthIncreaseRate => _healthIncreaseRate;

        private float _healthIncreaseRate;
        
        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _healthIncreaseRate = N1(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _healthIncreaseRate = 0f;
        }
    }
}