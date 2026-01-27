using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Manager
{
    public static class TraitCalculator
    {
        public static float GetFinalStat<T>(this ITraitHolder holder, float baseValue)
        {
            if (holder == null) return baseValue;
            
            var mods = holder.GetModifiers<object>(); 
        
            float additive = 0;
            float percentSum = 0;
            float multiplier = 1f;

            foreach (var m in mods)
            {
                if (m is IAdditiveModifier<T> a) additive += a.AdditiveValue;
                if (m is IPercentageModifier<T> p) percentSum += p.Percentage;
                if (m is IMultiplyModifier<T> x) multiplier *= x.Multiplier;
            }

            return (baseValue + additive) * (1f + percentSum) * multiplier;
        }
    }
}