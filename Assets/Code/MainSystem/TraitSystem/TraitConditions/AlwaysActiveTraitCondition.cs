using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitConditions
{
    public sealed class AlwaysActiveTraitCondition : AbstractTraitCondition
    {
        protected override bool CheckCondition(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }
    }
}