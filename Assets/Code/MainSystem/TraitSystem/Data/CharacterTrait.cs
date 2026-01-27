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
    public class CharacterTrait : MonoBehaviour, ITraitHolder
    {
        [SerializeField] private MemberType memberType;
        [SerializeField] private int maxPoints;
    
        public MemberType MemberType => memberType;
        
        private readonly List<ActiveTrait> _activeTraits = new();
        private readonly List<object> _modifiers = new();

        public int TotalPoint => _activeTraits.Sum(t => t.Data.Point);
        public int MaxPoints => maxPoints;
        public IReadOnlyList<ActiveTrait> ActiveTraits => _activeTraits;
        public bool IsAdjusting { get; private set; }
        public TraitDataSO PendingTrait { get; private set; }

        public void AddTrait(TraitDataSO data)
        {
            if (data is null)
                return;
            
            ActiveTrait newTrait = new ActiveTrait(data);
            _activeTraits.Add(newTrait);
        }

        public void RemoveActiveTrait(ActiveTrait trait)
        {
            if (trait == null)
                return;

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