using Code.MainSystem.TraitSystem.Data;
using UnityEngine;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.TraitEffect;

namespace Code.MainSystem.TraitSystem.Manager
{
    public static class TraitCalculator
    {
        // TODO 연결 작업후 삭제
        public static float GetFinalStat<T>(this ITraitHolder holder, float baseValue) where T : class
        {
            return Mathf.Max(0, 1); 
        }
        
        public static float GetCalculatedStat(this ITraitHolder holder, TraitTarget category, float baseValue, object context = null)
        {
            var modifiers = holder.GetModifiers<MultiStatModifierEffect>();
            float additive = 0f;
            float totalMultiplier = 0f;

            foreach (var m in modifiers)
            {
                if (!m.IsTargetStat(category)) continue;

                float amount = m.GetAmount(category, context);

                if (m.GetCalcType(category) == CalculationType.Additive)
                    additive += amount;
                else
                    totalMultiplier += amount * 0.01f;
            }

            return Mathf.Max(0, (baseValue + additive) * (1f + totalMultiplier));
        }
    }
}