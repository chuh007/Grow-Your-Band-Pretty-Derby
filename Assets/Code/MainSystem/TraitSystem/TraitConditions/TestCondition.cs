using Code.MainSystem.TraitSystem.Contexts;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitConditions
{
    public class TestCondition : ITraitCondition
    {
        public bool IsMet(GameContext context)
        {
            return true;
        }
    }
}