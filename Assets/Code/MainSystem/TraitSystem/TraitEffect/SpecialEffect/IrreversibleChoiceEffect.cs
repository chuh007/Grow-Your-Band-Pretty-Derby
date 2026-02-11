using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 되돌릴수 없는 선택 효과
    /// </summary>
    public class IrreversibleChoiceEffect : MultiStatModifierEffect, IConditionModifier
    {
        public float ConditionCostMultiplier { get; private set; }
        public float ConditionRecoveryMultiplier { get; private set; } = 0f;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            ConditionCostMultiplier = GetValue(0);
        }
    }
}