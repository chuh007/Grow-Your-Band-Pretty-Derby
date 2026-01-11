using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitHolder
    {
        void AddTrait(TraitDataSO newTrait);
        void RemoveActiveTrait(ActiveTrait trait);
    }
}