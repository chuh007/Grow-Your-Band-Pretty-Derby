using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.TraitEffect;
using Code.MainSystem.TraitSystem.TraitConditions;

namespace Code.MainSystem.TraitSystem.Data
{
    public enum TraitEffectType
    {
        OnTurnStart,
        OnActionSelect,
        OnActionExecute,
        OnActionComplete,
        OnTurnEnd,
        Passive
    }

    public enum ConditionType
    {
        NoneCondition,
        TurnBased,
        EventBased,
        ConditionBased
    }

    public enum TraitType
    {
        // NoneTrait,              // 특성 없음
        // Telepathy,              // 이심전심
        // LoneGuitarist,          // 고독한 기타리스트
        // ShiningEyes,            // 반짝이는 눈
        // FailureBreedsSuccess,   // 실패는 성공의 어머니
        // HonedTechnique,         // 단련기술
        // Injury,                 // 부상
        // Overzealous,            // 지나친 열정
        // Dogmatic,               // 독선적
        // Entertainer,            // 만담가
        Synergy,                // 시너지
        Focus,                  // 집중력
        HighlightBoost,         // 하이라이트 강화
        AttentionGain,          // 주목도 상승
        BreathControl,          // 호흡 조절
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

        public BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [HideInInspector] public string conditionName;
        [HideInInspector] public string effectName;
        
        [NonSerialized] public Action<AbstractTraitCondition> TraitConditionSetter;
        
        [NonSerialized] public Action<AbstractTraitEffect> TraitEffectSetter;

        public bool InitializeTrait()
        {
            bool conditionResult = ConditionFactory();
            bool effectResult = EffectFactory();
            
            return conditionResult && effectResult;
        }

        private bool ConditionFactory()
        {
            Type parentType = typeof(AbstractTraitCondition);
            Type traitType = FindType(conditionName);
            
            if (traitType == null || !parentType.IsAssignableFrom(traitType))
                return false;

            FieldInfo targetField = traitType.GetField("conditionType", bindingFlags);
            if (targetField == null || targetField.FieldType != typeof(ConditionType))
                return false;

            ParameterExpression param = Expression.Parameter(parentType, "condition");
            UnaryExpression casted = Expression.Convert(param, traitType);
            MemberExpression fieldAccess = Expression.Field(casted, targetField);
            ConstantExpression value = Expression.Constant(conditionType);
            BinaryExpression assign = Expression.Assign(fieldAccess, value);
            
            TraitConditionSetter = Expression.Lambda<Action<AbstractTraitCondition>>(assign, param).Compile();

            return true;
        }

        private bool EffectFactory()
        {
            Type parentType = typeof(AbstractTraitEffect);
            Type traitType = FindType(effectName);
            
            if (traitType == null || !parentType.IsAssignableFrom(traitType))
                return false;

            FieldInfo targetField = traitType.GetField("effectType", bindingFlags);
            if (targetField == null || targetField.FieldType != typeof(TraitEffectType))
                return false;
            
            ParameterExpression param = Expression.Parameter(parentType, "effect");
            UnaryExpression casted = Expression.Convert(param, traitType);
            MemberExpression fieldAccess = Expression.Field(casted, targetField);
            ConstantExpression value = Expression.Constant(traitEffectType);
            BinaryExpression assign = Expression.Assign(fieldAccess, value);
            
            TraitEffectSetter = Expression.Lambda<Action<AbstractTraitEffect>>(assign, param).Compile();
            
            return true;
        }

        private static Type FindType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == fullName || t.AssemblyQualifiedName == fullName);
        }
    }
}