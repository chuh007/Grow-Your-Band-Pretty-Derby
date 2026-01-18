using UnityEngine;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// TraitEffect 추상 베이스 클래스
    /// </summary>
    public abstract class AbstractTraitEffect : MonoBehaviour, ITraitEffect
    {
        public TraitEffectType EffectType { get; private set; }
        
        public void Apply(ITraitHolder holder, ActiveTrait trait)
        {
            if (!CanApply(holder, trait))
                return;
            
            ApplyEffect(holder, trait);
        }
        
        public void Remove(ITraitHolder holder, ActiveTrait trait)
        {
            RemoveEffect(holder, trait);
        }
        
        public abstract bool CanApply(ITraitHolder holder, ActiveTrait trait);
        
        /// <summary>
        /// 실제 효과 적용 로직 (하위 클래스에서 구현)
        /// </summary>
        protected abstract void ApplyEffect(ITraitHolder holder, ActiveTrait trait);
        
        /// <summary>
        /// 실제 효과 제거 로직 (하위 클래스에서 구현)
        /// </summary>
        protected abstract void RemoveEffect(ITraitHolder holder, ActiveTrait trait);
        
        // ===== 효과 수치 헬퍼 메서드 =====
        protected float GetEffectValue(ActiveTrait trait, int index)
        {
            if (trait.Data.Effects == null || index < 0 || index >= trait.Data.Effects.Count)
            {
                return 0f;
            }
            
            return trait.Data.Effects[index];
        }
        
        /// <summary>효과 수치 1 (N1)</summary>
        protected float N1(ActiveTrait trait) => GetEffectValue(trait, 0);
        
        /// <summary>효과 수치 2 (N2)</summary>
        protected float N2(ActiveTrait trait) => GetEffectValue(trait, 1);
        
        /// <summary>효과 수치 3 (N3)</summary>
        protected float N3(ActiveTrait trait) => GetEffectValue(trait, 2);
    }
}