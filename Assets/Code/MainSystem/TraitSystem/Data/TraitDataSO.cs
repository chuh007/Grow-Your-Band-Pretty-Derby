using System;
using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.TraitConditions;
using Code.MainSystem.TraitSystem.TraitEffect;

namespace Code.MainSystem.TraitSystem.Data
{
    /// <summary>
    /// 어떨때 실행이 되는지
    /// </summary>
    public enum TraitEffectType
    {
        OnTurnStart,
        OnActionSelect,
        OnActionExecute,
        OnActionComplete,
        OnTurnEnd,
        Passive
    }

    public enum TraitType
    {
        NoneTrait,              // 특성 없음
        Telepathy,              // 이심전심
        LoneGuitarist,          // 고독한 기타리스트
        ShiningEyes,            // 반짝이는 눈
        FailureBreedsSuccess,   // 실패는 성공의 어머니
        HonedTechnique,         // 단련기술
        Injury,                 // 부상
        Overzealous,            // 지나친 열정
        Dogmatic,               // 독선적
        Entertainer,            // 만담가
        Focus,                  // 집중력
        HighlightBoost,         // 하이라이트 강화
        AttentionGain,          // 주목도 상승
        BreathControl,          // 호흡 조절
    }

    [CreateAssetMenu(fileName = "Trait data", menuName = "SO/Trait/Trait data")]
    public class TraitDataSO : ScriptableObject
    {
        public int TraitID;
        public TraitType TraitType;
        public string TraitName;
        public Sprite TraitIcon;
        public TraitEffectType traitEffectType;
        public int MaxLevel;
        public int Point;
        public bool IsRemovable = true;
        public List<float> Effects = new();
        [TextArea] public string DescriptionEffect;
        
        [HideInInspector] public string conditionName;
        [HideInInspector] public string effectName;
        
        public AbstractTraitCondition CreateConditionInstance()
        {
            if (string.IsNullOrEmpty(conditionName))
                return null;
            Type t = Type.GetType(conditionName);
            return t != null ? Activator.CreateInstance(t) as AbstractTraitCondition : null;
        }

        public AbstractTraitEffect CreateEffectInstance()
        {
            if (string.IsNullOrEmpty(effectName)) 
                return null;
            Type t = Type.GetType(effectName);
            return t != null ? Activator.CreateInstance(t) as AbstractTraitEffect : null;
        }
    }
}