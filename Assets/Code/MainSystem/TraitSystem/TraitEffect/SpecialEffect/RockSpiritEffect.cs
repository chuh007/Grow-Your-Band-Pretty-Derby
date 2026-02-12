using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 록스피릿 효과
    /// </summary>
    public class RockSpiritEffect : MultiStatModifierEffect, ITrainingSuccessBonus
    {
        public float AddValue { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            AddValue = GetValue(0);
        }
    }
}