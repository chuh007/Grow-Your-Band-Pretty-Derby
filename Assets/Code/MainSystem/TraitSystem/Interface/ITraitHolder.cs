using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitHolder
    {
        int TotalPoint { get; }
        int MaxPoints { get; }
        IReadOnlyList<ActiveTrait> ActiveTraits { get; }
        
        void BeginAdjustment(TraitDataSO pendingTrait);
        void EndAdjustment();
        
        bool IsAdjusting { get; }
        TraitDataSO PendingTrait { get; }
        
        void AddTrait(TraitDataSO newTrait);
        void RemoveActiveTrait(ActiveTrait trait);
    }
}