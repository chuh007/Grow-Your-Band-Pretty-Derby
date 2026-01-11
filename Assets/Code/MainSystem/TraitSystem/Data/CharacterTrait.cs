using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Data
{
    public class CharacterTrait : MonoBehaviour, ITraitHolder
    {
        [SerializeField] private List<TraitDataSO> startingTraits;
        
        private List<ActiveTrait> _activeTraits = new List<ActiveTrait>();
        
        public IReadOnlyList<ActiveTrait> ActiveTraits => _activeTraits;

        private void Awake()
        {
            InitializeTraits();
        }

        private void InitializeTraits()
        {
            foreach (var data in startingTraits)
                AddTrait(data);
        }

        public void AddTrait(TraitDataSO data)
        {
            var newTrait = new ActiveTrait(data);
            _activeTraits.Add(newTrait);
        }
        
        public void RemoveActiveTrait(ActiveTrait trait)
        {
            if (_activeTraits.Contains(trait))
                _activeTraits.Remove(trait);
        }
    }
}