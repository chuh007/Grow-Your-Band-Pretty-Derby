using System.Linq;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public abstract class AbstractTraitEffect
    {
        protected ActiveTrait _activeTrait;
        
        public virtual void Initialize(ActiveTrait trait) 
            => _activeTrait = trait;

        protected float GetValue(int index) => _activeTrait.CurrentEffects.ElementAtOrDefault(index);
        
        public abstract bool IsTargetStat(TraitTarget category);
        public abstract float GetAmount(TraitTarget category, object context = null);
    }
}