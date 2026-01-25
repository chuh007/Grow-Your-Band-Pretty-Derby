using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.TraitConditions;
using Code.MainSystem.TraitSystem.TraitEffect;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.MainSystem.TraitSystem.Runtime
{
    public class ActiveTrait
    {
        public TraitDataSO Data { get; private set; }
        public AbstractTraitEffect TraitEffect { get; private set; }
        public AbstractTraitCondition TraitCondition { get; private set; }
        public int CurrentLevel { get; private set; }
        public List<float> CurrentEffects { get; private set; }

        public ActiveTrait(TraitDataSO data, Transform parent, int initialLevel = 1)
        {
            Data = data;
            CurrentLevel = initialLevel;
            CurrentEffects = new List<float>(data.Effects);
            
            if (Data.TraitEffectSetter == null) Data.InitializeTrait();

            CreateComponents(parent);
        }

        private void CreateComponents(Transform parent)
        {
            var go = new GameObject($"[Trait_{Data.TraitName}]");
            go.transform.SetParent(parent);
            
            if (!string.IsNullOrEmpty(Data.effectName))
            {
                var type = Type.GetType(Data.effectName);
                if (type != null)
                {
                    TraitEffect = go.AddComponent(type) as AbstractTraitEffect;
                    Data.TraitEffectSetter?.Invoke(TraitEffect);
                }
            }
            
            if (!string.IsNullOrEmpty(Data.conditionName))
            {
                var type = Type.GetType(Data.conditionName);
                if (type != null)
                {
                    TraitCondition = go.AddComponent(type) as AbstractTraitCondition;
                    Data.TraitConditionSetter?.Invoke(TraitCondition);
                }
            }
        }

        public void LevelUp()
        {
            if (CurrentLevel >= Data.MaxLevel) return;
        
            CurrentLevel++;
            // 매직 넘버(1.1f)보다는 데이터 기반 증가가 좋지만, 현재 구조 유지 시:
            for (int i = 0; i < CurrentEffects.Count; i++)
                CurrentEffects[i] *= 1.1f;
        }

        public void Dispose()
        {
            if (TraitEffect != null)
                Object.Destroy(TraitEffect.gameObject);
        }
        
        public string GetFormattedDescription()
        {
            if (Data is null || string.IsNullOrEmpty(Data.DescriptionEffect))
                return string.Empty;

            if (CurrentEffects == null || CurrentEffects.Count == 0)
                return Data.DescriptionEffect;

            var args = CurrentEffects
                .Select(FormatValue)
                .Cast<object>()
                .ToArray();

            try
            {
                return string.Format(Data.DescriptionEffect, args);
            }
            catch (FormatException)
            {
                return Data.DescriptionEffect;
            }
        }

        private string FormatValue(float value)
        {
            return Math.Abs(value) < 1f ? 
                (value * 100f).ToString("0.#", CultureInfo.InvariantCulture) : value.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}