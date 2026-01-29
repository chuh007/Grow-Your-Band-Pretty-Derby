using UnityEngine;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Manager
{
    public static class TraitCalculator
    {
        public static float GetFinalStat<T>(this ITraitHolder holder, float baseValue) where T : class
        {
            if (holder == null) return baseValue;
            
            var modifiers = holder.GetModifiers<T>();
            if (modifiers == null) return baseValue;

            float additive = 0f;
            float percent = 0f;
            float multiplier = 1f;

            foreach (var m in modifiers)
            {
                switch (m)
                {
                    case IAdditiveModifier<T> addMod:
                        additive += addMod.AdditiveValue;
                        break;
                    case IPercentageModifier<T> perMod:
                        percent += perMod.Percentage;
                        break;
                    case IMultiplyModifier<T> mulMod:
                        multiplier *= mulMod.Multiplier;
                        break;
                }
            }

            float result = (baseValue + additive) * (1f + percent) * multiplier;

            return Mathf.Max(0, result); 
        }
    }
}