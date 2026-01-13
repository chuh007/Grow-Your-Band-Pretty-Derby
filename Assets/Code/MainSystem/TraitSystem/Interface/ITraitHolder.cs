using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitHolder
    {
        int MaxPoints { get; }
        IReadOnlyList<ActiveTrait> ActiveTraits { get; }
        void AddTrait(TraitDataSO newTrait);
        void RemoveActiveTrait(ActiveTrait trait);
    }
}