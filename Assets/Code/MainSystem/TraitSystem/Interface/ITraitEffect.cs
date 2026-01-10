using Code.MainSystem.TraitSystem.Contexts;
using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitEffect
    {
        void Apply(CharacterTrait character, GameContext context);
    }
}