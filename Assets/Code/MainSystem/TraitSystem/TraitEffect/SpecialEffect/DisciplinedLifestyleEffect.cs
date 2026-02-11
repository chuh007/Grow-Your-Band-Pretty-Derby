using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 규칙적인 생활 효과
    /// </summary>
    public class DisciplinedLifestyleEffect : MultiStatModifierEffect, IDisciplinedLifestyle
    {
        private StatType _prevStatType = (StatType)(-1);

        public float BonusValue { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            BonusValue = GetValue(0);
        }
        
        public override float GetAmount(TraitTarget target, object context = null)
        {
            if (context is not StatType currentStatType)
                return 0f;
            
            if (_prevStatType != (StatType)(-1) && _prevStatType == currentStatType)
                return BonusValue;
            
            return 0f;
        }
        
        public void UpdateLastStat(StatType lastType)
        {
            _prevStatType = lastType;
        }
    }
}