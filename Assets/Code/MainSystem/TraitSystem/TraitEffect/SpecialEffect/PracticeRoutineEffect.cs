using Code.MainSystem.TraitSystem.Data;


namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 연습 루틴 효과
    /// </summary>
    public class PracticeRoutineEffect : MultiStatModifierEffect
    {
        private string _lastActionId = string.Empty;

        public override float QueryValue(TraitTrigger trigger, object context = null)
        {
            if (trigger != TraitTrigger.CalcSuccessRateBonus || context is not string currentActionId)
                return base.QueryValue(trigger, context);

            float bonus = 0f;

            if (!string.IsNullOrEmpty(_lastActionId) && _lastActionId == currentActionId)
                bonus = GetValue(0);

            _lastActionId = currentActionId;
            return bonus;

        }

        public override void OnTrigger(TraitTrigger trigger, object context = null)
        {
            if (trigger == TraitTrigger.OnRestStarted)
                _lastActionId = string.Empty;
        }
    }
}