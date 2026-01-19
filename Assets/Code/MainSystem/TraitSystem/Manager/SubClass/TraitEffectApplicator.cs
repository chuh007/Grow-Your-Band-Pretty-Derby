using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.TraitEffect;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    public class TraitEffectApplicator : MonoBehaviour
    {
        private readonly Dictionary<TraitEffectType, ITraitEffect> _effects = new();
        private ConditionChecker _conditionChecker; 
    
        private void Awake()
        {
            _conditionChecker = GetComponent<ConditionChecker>();
        }
    
        public void RegisterEffect(TraitEffectType type, ITraitEffect effect)
        {
            _effects[type] = effect;
        }
    
        public void ApplyEffects(ITraitHolder holder, TraitEffectType timing)
        {
            foreach (var trait in holder.ActiveTraits)
            {
                if (trait.Data.traitEffectType != timing) continue;
                if (!_effects.TryGetValue(trait.Data.traitEffectType, out var effect)) 
                    continue;
                
                if (!_conditionChecker.CheckCondition(holder, trait))
                    continue;
            
                if (effect.CanApply(holder, trait))
                    effect.Apply(holder, trait);
            }
        }
    }
}