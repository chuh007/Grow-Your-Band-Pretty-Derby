using UnityEngine;
using System.Collections.Generic;

namespace Code.MainSystem.TraitSystem.Data
{
    public enum TraitGroupType
    {
        None,
        Good
    }
    
    [CreateAssetMenu(fileName = "Trait group", menuName = "SO/Trait/Trait group", order = 0)]
    public class TraitGroupDataSO : ScriptableObject
    {
        public  TraitGroupType GroupType;
        public string GroupName;
        public List<TraitDataSO> TraitData;
    }
}