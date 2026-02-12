using Code.MainSystem.StatSystem.BaseStats;
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
        
        public float CheckPractice(StatType statType)
        {
            if (_prevStatType != (StatType)(-1) && _prevStatType == statType)
            {
                UpdateLastStat(statType);
                return BonusValue;
            }

            UpdateLastStat(statType);
            return 0f;
        }
        
        public void UpdateLastStat(StatType lastType)
        {
            _prevStatType = lastType;
        }
    }
}