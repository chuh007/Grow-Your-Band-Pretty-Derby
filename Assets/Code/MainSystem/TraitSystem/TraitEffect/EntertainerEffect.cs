using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 만담가 특성
    /// </summary>
    public class EntertainerEffect : AbstractTraitEffect, IAdditiveModifier
    {
        public float AdditiveValue { get; private set; }
        
        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            AdditiveValue = N1(trait);
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
            AdditiveValue = 0f;
        }
    }
}