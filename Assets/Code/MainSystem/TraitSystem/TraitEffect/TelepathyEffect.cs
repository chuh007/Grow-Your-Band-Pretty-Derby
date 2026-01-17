using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class TelepathyEffect : ITraitEffect
    {
        public void Apply(ITraitHolder holder, ActiveTrait trait)
        {
            
        }

        public void Remove(ITraitHolder holder, ActiveTrait trait)
        {
            
        }

        public bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }
    }
}