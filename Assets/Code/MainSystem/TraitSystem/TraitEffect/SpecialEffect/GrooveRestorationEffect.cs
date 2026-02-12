using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 흐름 되살리기 효과
    /// </summary>
    public class GrooveRestorationEffect : MultiStatModifierEffect, IGrooveRestoration
    {
        public bool IsBuffered { get; set; }
        public float Multiplier { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Multiplier = GetValue(0);
        }

        public void Reset()
        {
            IsBuffered = false;
        }
    }
}