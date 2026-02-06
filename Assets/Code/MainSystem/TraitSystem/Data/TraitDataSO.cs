using System;
using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.TraitEffect;

namespace Code.MainSystem.TraitSystem.Data
{
    [Serializable]
    public struct StatImpact
    {
        public TraitTarget Target;      // N번째 효과가 적용될 대상
        public CalculationType CalcType; // 계산 방식
        public string RequiredTag;       // 특정 조건
    }
    
    [CreateAssetMenu(fileName = "Trait data", menuName = "SO/Trait/Trait data")]
    public class TraitDataSO : ScriptableObject
    {
        public TraitType TraitType;
        public string TraitName;
        public Sprite TraitIcon;
        public int MaxLevel;
        public int Point;
        public bool IsRemovable = true;
        public List<StatImpact> Impacts;
        public List<float> Effects = new();
        
        [TextArea] public string DescriptionEffect;
        
        [Header("Special Case")]
        public string SpecialLogicClassName;

        public AbstractTraitEffect CreateEffectInstance()
        {
            if (string.IsNullOrEmpty(SpecialLogicClassName))
                return new MultiStatModifierEffect();

            var type = System.Type.GetType(SpecialLogicClassName);
            return (AbstractTraitEffect)System.Activator.CreateInstance(type);
        }
    }
}