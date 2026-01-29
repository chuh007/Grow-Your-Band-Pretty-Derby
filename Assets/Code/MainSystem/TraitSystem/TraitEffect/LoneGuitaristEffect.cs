using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 고독한 기타리스트 특성
    /// </summary>
    public class LoneGuitaristEffect : AbstractTraitEffect, IPracticeStat, IConditionStat, IPercentageModifier<IPracticeStat>, IPercentageModifier<IConditionStat>
    {
        float IPercentageModifier<IPracticeStat>.Percentage => _practicePercentage;
        float IPercentageModifier<IConditionStat>.Percentage => _conditionPercentage;
        
        private float _practicePercentage;
        private float _conditionPercentage;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            
            _practicePercentage = N1(trait); 
            _conditionPercentage = N2(trait);
        }
    }
}