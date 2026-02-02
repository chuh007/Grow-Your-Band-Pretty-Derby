using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 지나친 열정 특성
    /// </summary>
    public class OverzealousEffect : AbstractTraitEffect, IActionPointBonus, IConditionStat, IPercentageModifier<IConditionStat>
    {
        public float Chance { get; private set; }
        public int Amount { get; private set; }
        public float Percentage { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Chance = N1(trait);
            Amount = (int)N2(trait);
            Percentage = N3(trait);
        }
    }
}