using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 고위험 루틴 효과
    /// </summary>
    public class HighRiskRoutineEffect : MultiStatModifierEffect, IRoutineModifier
    {
        private float _addValue;
        private int _highRiskStack = 0; 

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            _addValue = GetValue(0);
        }

        public void OnPracticeSuccess() {
            if (_highRiskStack < 7) 
                _highRiskStack++; 
        }
        
        public void OnRest() {
            _highRiskStack = 0;
        }
        
        public float GetStatMultiplier() 
            => 1f + (_highRiskStack * 0.1f);
    }
}