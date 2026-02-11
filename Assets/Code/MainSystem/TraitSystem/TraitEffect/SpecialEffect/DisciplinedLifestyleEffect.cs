using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 규칙적인 생활 효과
    /// </summary>
    public class DisciplinedLifestyleEffect : MultiStatModifierEffect, IDisciplinedLifestyle
    {
        private StatType _prevStatType = (StatType)(-1);
        
        /// <summary>
        /// 스탯 타입으로 이전 행동과 동일한지 확인합니다.
        /// </summary>
        public bool CheckSameBehavior(StatType statType)
        {
            bool isSame = _prevStatType == statType;
            _prevStatType = statType;
            return isSame;
        }
    }
}