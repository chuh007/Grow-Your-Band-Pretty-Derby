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
            float totalMultiplier = 0f;

            foreach (var m in modifiers)
            {
                switch (m)
                {
                    case IAdditiveModifier<T> addMod:
                        additive += addMod.AdditiveValue;
                        break;

                    case IPercentageModifier<T> perMod:
                        totalMultiplier += perMod.Percentage * 0.01f;
                        break;

                    case IMultiplyModifier<T> mulMod:
                        totalMultiplier += mulMod.Multiplier - 1f;
                        break;
                }
            }
            
            float result = (baseValue + additive) * (1f + totalMultiplier);

            return Mathf.Max(0, result); 
        }
    }
}