using System.Linq;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.Runtime
{
    public class TraitGroupStatus
    {
        public TraitGroupDataSO GroupData { get; }

        public IReadOnlyList<TraitDataSO> RequiredTraits { get; }
        public IReadOnlyList<TraitDataSO> ContributedTraits { get; }

        public int CurrentCount => ContributedTraits.Count;
        public int MaxCount => RequiredTraits.Count;

        public TraitGroupStatus(
            TraitGroupDataSO groupData,
            List<TraitDataSO> requiredTraits,
            List<TraitDataSO> contributedTraits)
        {
            GroupData = groupData;
            RequiredTraits = requiredTraits;
            ContributedTraits = contributedTraits;
        }

        public bool IsTraitContributed(TraitDataSO trait)
        {
            return ContributedTraits.Contains(trait);
        }
    }
}