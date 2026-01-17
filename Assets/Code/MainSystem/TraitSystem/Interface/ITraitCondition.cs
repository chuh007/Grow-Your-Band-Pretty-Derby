using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitCondition
    {
        bool IsMet(ITraitHolder holder, ActiveTrait trait);
    }
}