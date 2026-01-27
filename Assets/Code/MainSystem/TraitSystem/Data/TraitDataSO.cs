using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

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
        public ConditionType conditionType;
        public List<float> Effects = new();
        [TextArea] public string DescriptionEffect;

        [HideInInspector] public string conditionName;
        [HideInInspector] public string effectName;

        public Action<MonoBehaviour> TraitConditionSetter { get; private set; }
        public Action<MonoBehaviour> TraitEffectSetter { get; private set; }

        private static readonly Dictionary<string, Type> TypeCache = new();

        public void InitializeTrait()
        {
            TraitConditionSetter = CreateSetter(conditionName, "conditionType", conditionType);
            TraitEffectSetter = CreateSetter(effectName, "effectType", traitEffectType);
        }

        private Action<MonoBehaviour> CreateSetter<TEnum>(string typeName, string fieldName, TEnum value)
        {
            Type targetType = FindType(typeName);
            if (targetType == null)
                return null;

            FieldInfo field = targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) 
                return null;

            ParameterExpression param = Expression.Parameter(typeof(MonoBehaviour), "obj");
            UnaryExpression casted = Expression.Convert(param, targetType);
            MemberExpression fieldAccess = Expression.Field(casted, field);
            BinaryExpression assign = Expression.Assign(fieldAccess, Expression.Constant(value));

            return Expression.Lambda<Action<MonoBehaviour>>(assign, param).Compile();
        }

        private static Type FindType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) 
                return null;
            
            if (TypeCache.TryGetValue(fullName, out var type)) 
                return type;

            type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == fullName || t.AssemblyQualifiedName == fullName);

            if (type != null) 
                TypeCache[fullName] = type;
            
            return type;
        }
    }
}