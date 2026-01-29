using System.Linq;
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
            if (modifiers == null || !modifiers.Any()) 
            {
                Debug.Log($"[Trait] {typeof(T).Name} 에 해당하는 Modifiers가 없습니다.");
                return baseValue;
            }

            float additive = 0f;
            float totalMultiplier = 0f;
            
            Debug.Log($"기본 밸류 : {baseValue}");

            foreach (var m in modifiers)
            {
                switch (m)
                {
                    case IAdditiveModifier<T> addMod:
                        additive += addMod.AdditiveValue;
                        Debug.Log($"[Trait] 더하기 적용 성공: {addMod.AdditiveValue}%");
                        break;

                    case IPercentageModifier<T> perMod:
                        totalMultiplier += perMod.Percentage * 0.01f;
                        Debug.Log($"[Trait] 퍼센트 적용 성공: {perMod.Percentage}%");
                        break;

                    case IMultiplyModifier<T> mulMod:
                        totalMultiplier += mulMod.Multiplier - 1f;
                        Debug.Log($"[Trait] 곱하기 적용 성공: {mulMod.Multiplier}%");
                        break;
                }
            }
            
            float result = (baseValue + additive) * (1f + totalMultiplier);
            
            Debug.Log($"최종 밸류 : {result}");

            return Mathf.Max(0, result); 
        }
    }
}