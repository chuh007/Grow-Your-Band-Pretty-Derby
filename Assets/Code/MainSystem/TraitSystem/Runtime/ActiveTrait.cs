using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.TraitConditions;
using Code.MainSystem.TraitSystem.TraitEffect;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Runtime
{
    public class ActiveTrait
    {
        public TraitDataSO Data { get; private set; }

        public AbstractTraitEffect TraitEffect { get; private set; }
        public AbstractTraitCondition TraitCondition { get; private set; }
        
        public int CurrentLevel { get; private set; }
        public bool IsActive { get; private set; }
        public int RemainingTurns { get; private set; }
        
        public List<float> CurrentEffects { get; private set; }
        
        public int Point => Data.Point;
        public string Name => Data.TraitName;
        public TraitType Type => Data.TraitType;
        
        public ActiveTrait(TraitDataSO data, int initialLevel = 1)
        {
            Data = data;
            CurrentLevel = initialLevel;
            IsActive = false;
            RemainingTurns = -1;
            CurrentEffects = new List<float>(data.Effects);
            
            // 런타임에 Effect와 Condition 인스턴스 생성
            CreateEffectInstance();
            CreateConditionInstance();
        }
        
        private void CreateEffectInstance()
        {
            if (string.IsNullOrEmpty(Data.effectName))
            {
                Debug.LogWarning($"Trait {Data.TraitName} has no effect name");
                return;
            }

            var effectType = System.Type.GetType(Data.effectName);
            if (effectType == null)
            {
                Debug.LogError($"Cannot find effect type: {Data.effectName}");
                return;
            }

            // GameObject를 생성하고 컴포넌트 추가
            var go = new GameObject($"{Data.TraitName}_Effect");
            go.hideFlags = HideFlags.HideInHierarchy;
            
            TraitEffect = go.AddComponent(effectType) as AbstractTraitEffect;
            
            if (TraitEffect != null && Data.TraitEffectSetter != null)
            {
                Data.TraitEffectSetter.Invoke(TraitEffect);
            }
        }
        
        private void CreateConditionInstance()
        {
            if (string.IsNullOrEmpty(Data.conditionName))
            {
                Debug.LogWarning($"Trait {Data.TraitName} has no condition name");
                return;
            }

            var conditionType = System.Type.GetType(Data.conditionName);
            if (conditionType == null)
            {
                Debug.LogError($"Cannot find condition type: {Data.conditionName}");
                return;
            }

            // GameObject를 생성하고 컴포넌트 추가
            var go = new GameObject($"{Data.TraitName}_Condition");
            go.hideFlags = HideFlags.HideInHierarchy;
            
            TraitCondition = go.AddComponent(conditionType) as AbstractTraitCondition;
            
            if (TraitCondition != null && Data.TraitConditionSetter != null)
            {
                Data.TraitConditionSetter.Invoke(TraitCondition);
            }
        }
        
        public void LevelUp()
        {
            CurrentLevel++;
            
            for (int i = 0; i < CurrentEffects.Count; i++)
                CurrentEffects[i] *= 1.1f;
        }
    }
}