using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 규칙적인 생활 효과
    /// </summary>
    public class DisciplinedLifestyleEffect : MultiStatModifierEffect
    {
        private StatType _prevStatType = (StatType)(-1);

        public override float QueryValue(TraitTrigger trigger, object context = null)
        {
            if (trigger != TraitTrigger.CalcSuccessRateBonus || context is not StatType currentType) 
                return 0f;
            
            bool isConsecutive = _prevStatType != (StatType)(-1) && _prevStatType == currentType;
              
            _prevStatType = currentType;

            return isConsecutive ? GetValue(0) : 0f;
        }

        public override void OnTrigger(TraitTrigger trigger, object context = null)
        {
            if (trigger == TraitTrigger.OnRestStarted)
                _prevStatType = (StatType)(-1);
        }
    }
}