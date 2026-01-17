using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitConditions
{
    public class EnsembleMemberCountCondition : ITraitCondition
    {
        public bool IsMet(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }
    }
}