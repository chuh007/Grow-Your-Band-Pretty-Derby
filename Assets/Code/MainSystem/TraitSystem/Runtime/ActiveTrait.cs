using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.TraitConditions;
using Code.MainSystem.TraitSystem.TraitEffect;

namespace Code.MainSystem.TraitSystem.Runtime
{
    public class ActiveTrait
    {
        public TraitDataSO Data { get; private set; }
        public AbstractTraitEffect TraitEffect { get; private set; }
        public AbstractTraitCondition TraitCondition { get; private set; }
        public int CurrentLevel { get; private set; }
        public List<float> CurrentEffects { get; private set; }

        public ActiveTrait(TraitDataSO data)
        {
            Data = data;
            CurrentLevel = 1;
            CurrentEffects = new List<float>(data.Effects);
            
            TraitEffect = data.CreateEffectInstance();
            TraitCondition = data.CreateConditionInstance();

            TraitEffect.Initialize(this);
        }

        public void LevelUp()
        {
            if (CurrentLevel >= Data.MaxLevel)
                return;
        
            CurrentLevel++;
            for (int i = 0; i < CurrentEffects.Count; i++)
                CurrentEffects[i] *= 1.1f;
        }
        
        public string GetFormattedDescription()
        {
            if (Data is null || string.IsNullOrEmpty(Data.DescriptionEffect))
                return string.Empty;

            if (CurrentEffects == null || CurrentEffects.Count == 0)
                return Data.DescriptionEffect;

            object[] args = CurrentEffects
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