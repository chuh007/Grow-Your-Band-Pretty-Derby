using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 서로서로 도와요 효과
    /// </summary>
    public class MutualAssistanceEffect : MultiStatModifierEffect, IMultiStatModifier
    {
        public int AddValue { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            AddValue = (int)GetValue(0);
        }
    }
}