using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 집중력 특성
    /// </summary>
    public class FocusEffect : AbstractTraitEffect, 
        ISuccessRateStat, 
        IPercentageModifier<ISuccessRateStat>, 
        IJudgmentCorrection
    {
        // 필드 이름을 명확히 구분
        private float _successRateBonus;
        public bool CorrectMissToGood => true;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            _successRateBonus = N1(trait);
        }

        // 명시적으로 IPercentageModifier<ISuccessRateStat>의 Percentage임을 정의
        float IPercentageModifier<ISuccessRateStat>.Percentage => _successRateBonus;
    }
}