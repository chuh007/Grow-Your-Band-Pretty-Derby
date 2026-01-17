using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitEffect
    {
        void Apply(ITraitHolder holder, ActiveTrait trait);
        void Remove(ITraitHolder holder, ActiveTrait trait);
        bool CanApply(ITraitHolder holder, ActiveTrait trait);
    }
}