using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 연습 루틴 효과
    /// </summary>
    public class PracticeRoutineEffect : MultiStatModifierEffect, IConsecutiveActionModifier
    {
        private string _lastActionId;
        
        public float GetSuccessBonus(string currentActionId)
        {
            float bonus = (_lastActionId == currentActionId) ? 10f : 0f;
            _lastActionId = currentActionId;
            return bonus;
        }
    }
}