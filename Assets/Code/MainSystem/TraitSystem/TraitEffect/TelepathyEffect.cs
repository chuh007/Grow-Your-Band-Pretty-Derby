using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class TelepathyEffect : AbstractTraitEffect
    {
        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return false;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            
        }
    }
}