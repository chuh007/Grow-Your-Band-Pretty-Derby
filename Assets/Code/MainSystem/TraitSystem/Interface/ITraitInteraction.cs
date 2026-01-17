using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitInteraction
    {
        bool CanInteract(ActiveTrait traitA, ActiveTrait traitB);
        void ProcessInteraction(ITraitHolder holder, ActiveTrait traitA, ActiveTrait traitB);
    }
}