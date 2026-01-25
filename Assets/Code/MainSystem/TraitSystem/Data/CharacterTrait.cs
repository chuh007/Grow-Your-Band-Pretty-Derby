using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.Data
{
    public class CharacterTrait : MonoBehaviour, ITraitHolder, IModifierProvider
    {
        [field:SerializeField] public MemberType MemberType { get; private set; }
        [field:SerializeField] public int MaxPoints { get; private set; }
        
        public int TotalPoint { get; private set; }

        public bool IsAdjusting { get; private set; }
        public TraitDataSO PendingTrait { get; private set; }

        public IReadOnlyList<ActiveTrait> ActiveTraits => _activeTraits;
        
        private readonly List<ActiveTrait> _activeTraits = new();
        private readonly List<object> _modifiers = new();

        public void AddTrait(TraitDataSO data)
        {
            var newTrait = new ActiveTrait(data);
            TotalPoint += newTrait.Point;
            _activeTraits.Add(newTrait);
        }
        
        public void RemoveActiveTrait(ActiveTrait trait)
        {
            TotalPoint -= trait.Point;
            if (_activeTraits.Contains(trait))
                _activeTraits.Remove(trait);
        }
        
        public void BeginAdjustment(TraitDataSO pendingTrait)
        {
            IsAdjusting = true;
            PendingTrait = pendingTrait;
        }

        public void EndAdjustment()
        {
            IsAdjusting = false;
            PendingTrait = null;
            Bus<TraitAdjusted>.Raise(new TraitAdjusted());
        }

        public IEnumerable<T> GetModifiers<T>() where T : class
        {
            return _modifiers.OfType<T>();
        }

        public void RegisterModifier(object modifier)
        {
            if (!_modifiers.Contains(modifier))
                _modifiers.Add(modifier);
        }

        public void UnregisterModifier(object modifier)
        {
            _modifiers.Remove(modifier);
        }
    }
}