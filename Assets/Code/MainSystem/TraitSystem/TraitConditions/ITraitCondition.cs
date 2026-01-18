using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitConditions
{
    public interface ITraitCondition
    {
        bool IsMet(ITraitHolder holder, ActiveTrait trait);
    }
}