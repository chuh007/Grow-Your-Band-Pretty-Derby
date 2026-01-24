using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Manager
{
    public class TraitGroupManager : MonoBehaviour
    {
        [SerializeField] private List<TraitGroupDataSO> groupDataList;

        public IReadOnlyList<TraitGroupStatus> BuildGroupStatus(IReadOnlyDictionary<MemberType, ITraitHolder> holders)
        {
            var allTraits = holders.Values
                .SelectMany(h => h.ActiveTraits)
                .Select(t => t.Data)
                .ToList();

            return (from @group in groupDataList
                let contributedTraits = allTraits.Where(t => @group.TraitData.Contains(t))
                    .ToList()
                select new TraitGroupStatus(@group, @group.TraitData, contributedTraits)).ToList();
        }
    }
}