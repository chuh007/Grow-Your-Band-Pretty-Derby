using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class FailureBreedsSuccessEffect : AbstractTraitEffect, IFailureBreedsSuccessModifier
    {
        public int InspirationGainOnFail => _inspirationGainOnFail;
        public int GuaranteedSuccessThreshold => _guaranteedSuccessThreshold;

        private int _inspirationGainOnFail;
        private int _guaranteedSuccessThreshold;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _inspirationGainOnFail = (int)N1(trait);
            _guaranteedSuccessThreshold = (int)N2(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _inspirationGainOnFail = 0;
            _guaranteedSuccessThreshold = 0;
        }
    }
}