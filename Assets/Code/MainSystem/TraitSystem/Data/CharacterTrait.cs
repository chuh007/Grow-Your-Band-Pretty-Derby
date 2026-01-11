using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Data
{
    public class CharacterTrait : MonoBehaviour, ITraitHolder
    {
        private List<ActiveTrait> _activeTraits = new List<ActiveTrait>();
        
        public IReadOnlyList<ActiveTrait> ActiveTraits => _activeTraits;

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