using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Interface;
using UnityEngine.Serialization;

namespace Code.MainSystem.TraitSystem.Data
{
    // 효과 적용 시점 정의
    public enum TraitEffectType
    {
        OnTurnStart,
        OnActionSelect,
        OnActionExecute,
        OnActionComplete,
        OnTurnEnd,
        Passive // 항상 적용
    }
    
    public enum ConditionType
    {
        NoneCondition, TurnBased, EventBased, ConditionBased
    }

    public enum TraitType
    {
        NoneTrait,                   // 특성 없음
        Telepathy,              // 이심전심
        LoneGuitarist,          // 고독한 기타리스트
        ShiningEyes,            // 반짝이는 눈
        FailureBreedsSuccess,   // 실패는 성공의 어머니
        HonedTechnique,         // 단련기술
        Injury,                 // 부상
        Overzealous,            // 지나친 열정
        Dogmatic,               // 독선적
        Entertainer             // 만담가
    }
    
    [CreateAssetMenu(fileName = "Trait data", menuName = "SO/Trait/Trait data", order = 0)]
    public class TraitDataSO : ScriptableObject
    {
        public int TraitID;
        public TraitType TraitType;
        public string TraitName;
        public Sprite TraitIcon;
        
        public TraitEffectType traitEffectType;

        public int Level;
        public int MaxLevel;
        public int Point;

        public bool IsRemovable;
        public ConditionType conditionType;

        public List<float> Effects;
        
        [TextArea] public string DescriptionEffect;
        
        [SerializeReference] public ITraitCondition Condition;
        [SerializeReference] public ITraitEffect Effect;
    }
}