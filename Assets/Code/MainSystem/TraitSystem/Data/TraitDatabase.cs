using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Interface;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Data
{
    public class TraitDatabase : MonoBehaviour, ITraitDatabase
    {
        [SerializeField] private List<TraitDataSO> traits;

        private Dictionary<TraitType, TraitDataSO> _map;

        private void Awake()
        {
            _map = new Dictionary<TraitType, TraitDataSO>();
            foreach (var trait in traits)
                _map[trait.TraitType] = trait;
        }

        public TraitDataSO Get(TraitType traitType)
        {
            return _map[traitType];
        }
    }
}