using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.TraitExtensionMethod;

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

        private void Start()
        {
            TraitManager.Instance.RegisterHolder(this);
        }

        public void AddTrait(TraitDataSO data)
        {
            if (data is null) return;
    
            ActiveTrait newTrait = new ActiveTrait(data, memberType);
            _activeTraits.Add(newTrait);
            
            if (newTrait.TraitEffect != null)
                RegisterModifier(newTrait.TraitEffect);
        }

        public void RemoveActiveTrait(ActiveTrait trait)
        {
            if (trait == null) return;
            
            if (trait.TraitEffect != null)
                UnregisterModifier(trait.TraitEffect);

            _activeTraits.Remove(trait);
        }
        
        public void ExecuteTrigger(TraitTrigger trigger, object context = null)
        {
            foreach (var trait in _activeTraits)
                trait.TraitEffect?.OnTrigger(trigger, context);
        }

        public float QueryTriggerValue(TraitTrigger trigger, object context = null)
        {
            bool isMultiplier = trigger.ToString().Contains("Multiplier");
            float result = isMultiplier ? 1f : 0f;

            foreach (var trait in _activeTraits)
            {
                float val = trait.TraitEffect?.QueryValue(trigger, context) ?? 0f;
                if (isMultiplier) result *= (1f + val); 
            }
            return result;
        }

        public bool CheckTriggerCondition(TraitTrigger trigger, object context = null)
        {
            return _activeTraits.Any(t => t.TraitEffect != null && t.TraitEffect.CheckCondition(trigger, context));
        }

        public void RestoreTraits(IEnumerable<ActiveTrait> traits)
        {
            if (traits == null)
                return;
            
            _activeTraits.Clear();
            _modifiers.Clear();
            
            foreach (var trait in traits)
            {
                _activeTraits.Add(trait);
                
                if (trait.TraitEffect != null)
                    RegisterModifier(trait.TraitEffect);
            }
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

        public float GetCalculatedStat(TraitTarget category, float baseValue, object context = null)
        {
            return TraitCalculator.GetCalculatedStat(this, category, baseValue, context);
        }

        public IEnumerable<T> GetModifiers<T>() where T : class
        {
            return _modifiers.OfType<T>();
        }

        private void RegisterModifier(object modifier)
        {
            if (!_modifiers.Contains(modifier))
                _modifiers.Add(modifier);
        }

        private void UnregisterModifier(object modifier)
        {
            _modifiers.Remove(modifier);
        }
    }
}