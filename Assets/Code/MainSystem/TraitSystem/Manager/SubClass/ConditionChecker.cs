using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.TraitConditions;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    public class ConditionChecker : MonoBehaviour
    {
        private readonly Dictionary<ConditionType, AbstractTraitCondition> _conditions = new();
    
        public void RegisterCondition(ConditionType type, AbstractTraitCondition condition)
        {
            _conditions[type] = condition;
        }
    
        public bool CheckCondition(ITraitHolder holder, ActiveTrait trait)
        {
            if (trait.Data.conditionType == ConditionType.NoneCondition)
                return true;
            
            return _conditions.TryGetValue(trait.Data.conditionType, out var condition) 
                   && condition.IsMet(holder, trait);
        }
    }
}