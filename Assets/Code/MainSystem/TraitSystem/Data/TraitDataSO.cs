using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Code.MainSystem.TraitSystem.TraitConditions;
using Code.MainSystem.TraitSystem.TraitEffect;

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
        NoneTrait,              
        Telepathy,              
        LoneGuitarist,          
        ShiningEyes,            
        FailureBreedsSuccess,   
        HonedTechnique,         
        Injury,                 
        Overzealous,            
        Dogmatic,               
        Entertainer             
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

        [HideInInspector] public string conditionTypeName;
        [HideInInspector] public string effectTypeName;

        private Action<AbstractTraitCondition> _traitConditionSetter;
        private Action<AbstractTraitEffect> _traitEffectSetter;

        public bool InitializeTrait()
        {
            return ConditionFactory() && EffectFactory();
        }

        public void ApplyCondition(AbstractTraitCondition condition)
        {
            _traitConditionSetter?.Invoke(condition);
        }

        public void ApplyEffect(AbstractTraitEffect effect)
        {
            _traitEffectSetter?.Invoke(effect);
        }

        private bool ConditionFactory()
        {
            var parentType = typeof(AbstractTraitCondition);
            var traitType = FindType(conditionName);
            if (traitType == null || !parentType.IsAssignableFrom(traitType))
                return false;

            var targetField = traitType.GetField(conditionTypeName, bindingFlags);
            if (targetField == null || targetField.FieldType != typeof(ConditionType))
                return false;

            ParameterExpression param = Expression.Parameter(parentType, "condition");
            UnaryExpression casted = Expression.Convert(param, traitType);
            MemberExpression fieldAccess = Expression.Field(casted, targetField);
            ConstantExpression value = Expression.Constant(conditionType);
            BinaryExpression assign = Expression.Assign(fieldAccess, value);

            _traitConditionSetter = Expression.Lambda<Action<AbstractTraitCondition>>(assign, param).Compile();

            return true;
        }

        private bool EffectFactory()
        {
            var parentType = typeof(AbstractTraitEffect);
            var traitType = FindType(effectName);
            if (traitType == null || !parentType.IsAssignableFrom(traitType))
                return false;

            var targetField = traitType.GetField(effectTypeName, bindingFlags);
            if (targetField == null || targetField.FieldType != typeof(TraitEffectType))
                return false;

            ParameterExpression param = Expression.Parameter(parentType, "effect");
            UnaryExpression casted = Expression.Convert(param, traitType);
            MemberExpression fieldAccess = Expression.Field(casted, targetField);
            ConstantExpression value = Expression.Constant(traitEffectType);
            BinaryExpression assign = Expression.Assign(fieldAccess, value);

            _traitEffectSetter = Expression.Lambda<Action<AbstractTraitEffect>>(assign, param).Compile();

            return true;
        }

        private static Type FindType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == fullName);
        }
    }
}