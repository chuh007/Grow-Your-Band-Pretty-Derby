using Code.MainSystem.TraitSystem.Data;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 반짝이는 눈 효과
    /// </summary>
    public class ShiningEyesEffect : MultiStatModifierEffect
    {
        public override float QueryValue(TraitTrigger trigger, object context = null)
        {
            if (trigger != TraitTrigger.CheckAdditionalAction) 
                return base.QueryValue(trigger, context);
            
            if (context is >= 60f)
                return Random.Range(0f, 100f) < GetValue(2) ? 1f : 0f;
            
            return base.QueryValue(trigger, context);
        }
    }
}