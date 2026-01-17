using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public interface ITraitEffect
    {
        void Apply(ITraitHolder holder, ActiveTrait trait);
        void Remove(ITraitHolder holder, ActiveTrait trait);
        bool CanApply(ITraitHolder holder, ActiveTrait trait);
    }
}