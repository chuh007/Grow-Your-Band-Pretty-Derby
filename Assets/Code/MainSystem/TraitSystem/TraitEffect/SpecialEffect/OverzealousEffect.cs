using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 지나친 열정 효과
    /// </summary>
    public class OverzealousEffect : MultiStatModifierEffect, IOverzealous, IAdditionalActionProvider
    {
        public float ConditionCostMultiplier { get; private set; }
        public float ConditionRecoveryMultiplier { get; private set; }
        public float AdditionalActionChance { get; private set; }

        private float _currentAction;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            ConditionCostMultiplier = GetValue(0);
            ConditionRecoveryMultiplier = GetValue(1);
            AdditionalActionChance = GetValue(2);
        }
        
        public bool CheckAction()
        {
            if (_currentAction <= 0)
                return false;
            
            _currentAction -= 1;
            return true;
        }
    }
}