using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Data
{
    public enum TraitEffectType
    {
        Training, Performance, Etc
    }
    
    public enum ExpirationType
    {
        None, TurnBased, EventBased, ConditionBased
    }

    public enum TraitType
    {
        None,                   // 특성 없음
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
        public ExpirationType ExpirationType;

        public List<float> Effects;
        
        [TextArea] public string DescriptionEffect;
        
        [SerializeReference] public ITraitCondition Condition;
        [SerializeReference] public ITraitEffect Effect;
    }
}