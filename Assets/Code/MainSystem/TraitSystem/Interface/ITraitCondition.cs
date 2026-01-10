using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Contexts;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitCondition
    {
        bool IsMet(CharacterTrait character, GameContext context);
    }
}