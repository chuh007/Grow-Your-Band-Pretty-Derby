using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitConditions
{
    /// <summary>
    /// TraitCondition 추상 베이스 클래스
    /// </summary>
    public abstract class TraitCondition : ITraitCondition
    {
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