using UnityEngine;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitConditions
{
    /// <summary>
    /// TraitCondition 추상 베이스 클래스
    /// </summary>
    public abstract class AbstractTraitCondition : MonoBehaviour, ITraitCondition
    {
        // Expression Tree에서 접근할 수 있도록 protected로 변경
        [SerializeField] protected ConditionType conditionType;

        public ConditionType ConditionType => conditionType;

        public bool IsMet(ITraitHolder holder, ActiveTrait trait)
        {
            var result = CheckCondition(holder, trait);
            
            return result;
        }
        
        /// <summary>
        /// 실제 조건 체크 로직
        /// </summary>
        protected abstract bool CheckCondition(ITraitHolder holder, ActiveTrait trait);
    }
}