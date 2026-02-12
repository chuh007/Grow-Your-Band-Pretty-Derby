using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 연습 루틴 효과
    /// </summary>
    public class PracticeRoutineEffect : MultiStatModifierEffect, IConsecutiveActionModifier
    {
        public float PecValue { get; private set; }

        private string _lastActionId;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            PecValue = GetValue(0);
            _lastActionId = "";
        }

        public float GetSuccessBonus(string currentActionId)
        {
            float bonus = _lastActionId == currentActionId ? PecValue : 0f;
            _lastActionId = currentActionId;
            return bonus * 0.01f;
        }
    }
}