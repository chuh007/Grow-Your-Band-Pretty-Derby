using System.Linq;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public abstract class AbstractTraitEffect
    {
        protected ActiveTrait ActiveTrait;
        
        public virtual void Initialize(ActiveTrait trait)
        {
            ActiveTrait = trait;
        }
        
        /// <summary>
        /// 이 Effect를 적용할 수 있는지 확인
        /// </summary>
        public abstract bool CanApply(ITraitHolder holder, ActiveTrait trait);
        
        /// <summary>
        /// Effect 적용
        /// </summary>
        public void Apply(ITraitHolder holder, ActiveTrait trait)
        {
            if (trait.TraitCondition != null && !trait.TraitCondition.IsMet(holder, trait))
                return;
                
            ApplyEffect(holder, trait);
        }
        
        /// <summary>
        /// Effect 제거
        /// </summary>
        public void Remove(ITraitHolder holder, ActiveTrait trait)
        {
            RemoveEffect(holder, trait);
        }
        
        protected abstract void ApplyEffect(ITraitHolder holder, ActiveTrait trait);
        protected abstract void RemoveEffect(ITraitHolder holder, ActiveTrait trait);
        
        // 헬퍼 메서드들
        protected float N1(ActiveTrait trait) => trait.CurrentEffects.ElementAtOrDefault(0);
        protected float N2(ActiveTrait trait) => trait.CurrentEffects.ElementAtOrDefault(1);
        protected float N3(ActiveTrait trait) => trait.CurrentEffects.ElementAtOrDefault(2);
    }
}